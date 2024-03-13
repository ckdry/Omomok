using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using UnityEngine.UI;
using TMPro;

public class ChzzkUnity : MonoBehaviour
{
    private MainThreadDispatcher dispatcher;
    [SerializeField]
    public TMP_Text chatBoxText;
    [SerializeField]
    private string chatText;
    [SerializeField]
    string liveId ="";
    public StreamerInfo streamerData;
    public LiveStatus liveData;


    private enum SslProtocolsHack
    {
        Tls = 192,
        Tls11 = 768,
        Tls12 = 3072
    }

    string cid;
    string token;
    string channel;

    WebSocket socket = null;
    string wsURL = "wss://kr-ss3.chat.naver.com/chat";

    float timer = 0f;
    bool running = false;

    string heartbeatRequest = "{\"ver\":\"2\",\"cmd\":0}";
    string heartbeatResponse = "{\"ver\":\"2\",\"cmd\":10000}";

    public Profile prf;


    //public Action<Profile, string, DonationExtras> onDonation = (profile, str, extra)

    Action<Profile, string> onMessage;
    Action<Profile, string, DonationExtras> onDonation;

    [SerializeField]
    Concave concaveGame;
    [SerializeField]
    ChzzkChatVote chzzkChatVote;

    [SerializeField]
    TMP_InputField idInputfiled;
    [SerializeField]
    Button connectButton;
    [SerializeField]
    RawImage profileImg;
    [SerializeField]
    TMP_Text streamerNameText;
    private void Awake()
    {
        liveId = "";

    }

    // Start is called before the first frame update
    void Start()
    {
        concaveGame = GameObject.FindObjectOfType<Concave>();
        chzzkChatVote = GameObject.FindObjectOfType<ChzzkChatVote>();
        dispatcher = MainThreadDispatcher.Instance;

        connectButton.onClick.AddListener(() => StartListening(idInputfiled.text));


        onMessage = (profile, str) => {
            // 채팅이 오면 할 행동
            dispatcher.Enqueue(() => {
                chatText += "<#000000>" + profile.nickname + ": </color>" + str + "\n";
                
                int lineCount = chatText.Split('\n').Length;
                if (lineCount>20)
                    chatText = chatText.Substring(chatText.IndexOf("\n") + 1);

                ChatText(chatText);

                chzzkChatVote.CheckUserVote(profile.nickname, str);
                //chzzkChatVote.ChatCheck(str);
            });
        };

        onDonation = (profile, str, extra) => {
            // 후원이 오면 할 행동
             dispatcher.Enqueue(() => {/*
                chatText += "<#000000>" + profile.nickname + "님의 " + extra.payAmount+ "원 치즈 후원: </color>" + str + "\n";
                
                int lineCount = chatText.Split('\n').Length;
                if (lineCount>15)
                    chatText = chatText.Substring(chatText.IndexOf("\n") + 1);
                chatBoxText.text = chatText;*/


            });

        };
        //Debug.Log("시작");
        //StartListening(liveId);
    }

    public void ChatText(string str = null)
    {
        if(str == null)
            chatText = "";

        chatBoxText.text = str;
    }

    public void removeAllOnMessageListener() 
    {
        onMessage = (profile, str) => { };
    }

    public void removeAllOnDonationListener()
    {
        onMessage = (profile, str) => { };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (running)
        {
            timer += Time.unscaledDeltaTime;
            if (timer > 15)
            {
                socket.Send(heartbeatRequest);
                timer = 0;
            }
        }
    }

    IEnumerator Connect()
    {
        string url = $"https://api.chzzk.naver.com/service/v1/channels/" + liveId;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);

            StreamerInfo streamerInfo = JsonUtility.FromJson<StreamerInfo>(request.downloadHandler.text);
            streamerData = streamerInfo;

            yield return GetImage(streamerData.content.channelImageUrl);
            streamerNameText.text = streamerData.content.channelName;

        }

        string URL = $"https://api.chzzk.naver.com/polling/v2/channels/"+ liveId+"/live-status";
        request = UnityWebRequest.Get(URL);
        Debug.Log(URL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
            LiveStatus status = JsonUtility.FromJson<LiveStatus>(request.downloadHandler.text);
            Debug.Log(status);
            liveData = status;
            cid = status.content.chatChannelId;
            //cidOutput.text = cid;
            URL = $"https://comm-api.game.naver.com/nng_main/v1/chats/access-token?channelId={cid}&chatType=STREAMING";
            Debug.Log(URL);
            request = UnityWebRequest.Get(URL);
            //Debug.Log(URL);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
                AccessTokenResult tokenResult = JsonUtility.FromJson<AccessTokenResult>(request.downloadHandler.text);
                token = tokenResult.content.accessToken;
                socket = new WebSocket(wsURL);

                var sslProtocolHack = (System.Security.Authentication.SslProtocols)(SslProtocolsHack.Tls12 | SslProtocolsHack.Tls11 | SslProtocolsHack.Tls);
                socket.SslConfiguration.EnabledSslProtocols = sslProtocolHack;
                socket.OnMessage += Recv;
                socket.OnClose += CloseConnect;
                socket.OnOpen += OnStartChat;
                socket.Connect();

                concaveGame.gameStartBtn.interactable = true;
                concaveGame.SettingPnlOpen();
            }
            else
            {
                Debug.Log($"ERROR On get token : {request.result} : {request.error}");
                concaveGame.gameStartBtn.interactable = false;
            }
        }
        else
        {
            // 올바른 ID를 입력하지 않았을 때
            Debug.Log($"ERROR On get cid : {request.result} : {request.error}");
            concaveGame.gameStartBtn.interactable = false;
        }
    }

    IEnumerator GetImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            profileImg.texture = myTexture;
        }
    }

    void Recv(object sender, MessageEventArgs e)
    {
        
        try
        {
            //kills.Add(JsonUtility.FromJson<Kill>(e.Data));            
            IDictionary<string, object> data = JsonConvert.DeserializeObject<IDictionary<string, object>>(e.Data);

            StringBuilder sb = new StringBuilder();
            foreach(string key in data.Keys)
            {
                if (data[key] != null) 
                {
                    sb.Append("\n" + key + ":" + data[key].ToString());
                }
                else
                {
                    sb.Append("\n" + key + ": NULL");
                }
                
            }
            //Debug.Log(data["cmd"].GetType());

            

            switch ((long)data["cmd"])
            {
                case 0://HeartBeat Request
                    socket.Send(heartbeatResponse);
                    timer = 0;
                    break;
                case 93101://Chat
                    //Debug.Log(data["bdy"].GetType());
                    JArray bdy = (JArray)data["bdy"];
                    //Debug.Log(bdy[0].GetType());
                    JObject bdyObject = (JObject)bdy[0];

                    string profileText = bdyObject["profile"].ToString();
                    profileText = profileText.Replace("\\", "");                        
                    Profile profile = JsonUtility.FromJson<Profile>(profileText);

                    Debug.Log(profile.nickname + " : " + bdyObject["msg"].ToString());              
                    //debugText.Append($"{profile.nickname}({profile.userIdHash}) : {bdyObject["msg"]}\n");
                    onMessage(profile, bdyObject["msg"].ToString());

                    //chatText.text += profile.nickname + ": " + bdyObject["msg"].ToString() + "\n";
                    //SetText(bdyObject["msg"].ToString());
                    break;

                case 93006: //Blind Chat
                    bdy = (JArray)data["bdy"];
                    //Debug.Log(bdy[0].GetType());
                    bdyObject = (JObject)bdy[0];

                    profileText = bdyObject["profile"].ToString();
                    profileText = profileText.Replace("\\", "");
                    //Debug.Log(profileText);                                      
                    profile = JsonUtility.FromJson<Profile>(profileText);

                    onMessage(profile, bdyObject["msg"].ToString());
                    break;
                case 93102://Donation
                    Debug.Log("치즈후원");
                    bdy = (JArray)data["bdy"];
                    bdyObject = (JObject)bdy[0];
                    profileText = bdyObject["profile"].ToString();
                    profileText = profileText.Replace("\\", "");
                    profile = JsonUtility.FromJson<Profile>(profileText);

                    string extraText = bdyObject["extras"].ToString();
                    extraText = extraText.Replace("\\", "");
                    Debug.Log(extraText);
                    DonationExtras extras = JsonUtility.FromJson<DonationExtras>(extraText);
                    Debug.Log(extraText);
                    onDonation(profile, bdyObject["msg"].ToString(),extras);
                    //Debug.Log(data["cmd"]);
                    //Debug.Log(e.Data);
                    break;
                case 94008://Blocked Message(CleanBot)
                case 94201://Member Sync
                case 10000://HeartBeat Response
                case 10100://Token ACC
                    break;//Nothing to do
                default:
                    Debug.LogError(data["cmd"]);
                    Debug.LogError(e.Data);
                    break;
            }
        }
        
        catch (Exception er)
        {
            Debug.Log(er.ToString());
        }
    }

    void CloseConnect(object sender, CloseEventArgs e)
    {
        Debug.Log(e.Reason);
        Debug.Log(e.Code);
        Debug.Log(e);

        try
        {
            if (socket == null) return;

            if (socket.IsAlive) socket.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.StackTrace);
        }
    }

    void OnStartChat(object sender, EventArgs e)
    {
        Debug.Log("OPENED");
        string message = $"{{\"ver\":\"2\",\"cmd\":100,\"svcid\":\"game\",\"cid\":\"{cid}\",\"bdy\":{{\"uid\":null,\"devType\":2001,\"accTkn\":\"{token}\",\"auth\":\"READ\"}},\"tid\":1}}";
        timer = 0;
        running = true;
        socket.Send(message);
    }


    public void StartListening(string channelId)
    {
        chzzkChatVote.ChzzkUserClear();
        ChatText();
        if (socket!=null && socket.IsAlive)
        {
            socket.Close();
            socket = null;
        }
        liveId = channelId;
        StartCoroutine(Connect());
    }

    public void StopListening()
    {

        chzzkChatVote.ChzzkUserClear();
        ChatText();
        if (socket != null)
        {
            liveData = null;
            socket.Close();
        }
    }

    
}
