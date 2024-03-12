using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ChzzkChatVote : MonoBehaviour
{
    [SerializeField]
    Concave concaveGame;
    [SerializeField]
    List<string> chzzkUserList = new List<string>();

    [SerializeField]
    List<CellPosVote> cellVoteList = new List<CellPosVote>();

    [SerializeField]
    Slider voteTimerSlider;
    [SerializeField]
    TMP_Text voteTimeText;


    [Header("투표 확률 퍼센트")]
    [SerializeField]
    Transform votePercentCanvas;
    [SerializeField]
    List<TMP_Text> votePercentText;
    private IEnumerator Start()
    {
        concaveGame = GameObject.FindObjectOfType<Concave>();
        ChzzkUserClear();
        CheckUserVote("a","a10");
        CheckUserVote("모르는사람", "b1");
        CheckUserVote("모르는사람123", "a10");
        CheckUserVote("a", "b1");
        CheckUserVote("a", "C9");
        CheckUserVote("a", "C9");
        CheckUserVote("a", "B5");
        CheckUserVote("a", "B5");

                yield return new WaitForSeconds(3.5f);
        
                SetCellVote();

    }

    public void ChzzkUserClear()
    {
        chzzkUserList.Clear();
        cellVoteList.Clear();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log("click");
            VotePercentUIDraw();
        }
    }
    /// <summary>
    /// 중복 투표가 되지 않도록 닉네임을 체크하는 함수
    /// </summary>
    public void CheckUserVote(string nickName, string str)
    {
        if (concaveGame.isStreamerTurn) //유저들의 투표 시간이 아닌경우
            return;

        if (chzzkUserList.Contains(nickName)) //닉네임으로 이미 투표가 된 경우
            return;
        else
        {
            ChatCheck(nickName,str);
        }
    }

    public void ChatCheck(string nickName, string str)
    {
        // 오목의 위치는 3글자를 넘어가지 않음
        if (str.Length > 3)
            return;
        char firstChar = str[0];

        //대소문자를 전부 소문자로
        int asciiValue = char.ToLower(firstChar);
        str = str.ToLower();
        // 'a'부터 판 크기까지의 ASCII 값 범위
        int minAscii = (int)'a';
        int maxAscii = minAscii + concaveGame.boardSize - 1; // 판 사이즈에 맞게

        // 첫 글자가 판 안에 있는 위치일 경우
        if (asciiValue >= minAscii && asciiValue <= maxAscii)
        {
            //나머지 글자 다따와서 숫자인지 확인
            string col = str.Substring(1, str.Length - 1);
            int.TryParse(col, out int colNum);

            //나머지 글자가 판 안에 있는 위치일 경우
            if (colNum >= 1 && colNum <= concaveGame.boardSize)
            {

                chzzkUserList.Add(nickName);
                //아스키코드 값 고려 등 배열 위치값이 0,0 이 되도록 조정
                asciiValue -= 97;
                colNum -= 1;

                CellPosVote cellUpdate = cellVoteList.Find(cellvote => cellvote.position == str);
                if(cellUpdate == null)
                {
                    //대소문자를 전부 소문자로
                    cellUpdate = new CellPosVote { position = str, voteNum = 1, row = asciiValue, col = colNum };
                    cellVoteList.Add(cellUpdate);
                }
                else
                {
                    cellUpdate.voteNum += 1;
                }
                //cellPositionList.Add(new List<int> { 1, 1 });
                //돌 놓기
                //ChatPlaceStone(asciiValue , colNum);
            }
        }

        return;
    }

    /// <summary>
    /// 유저 투표 시작
    /// </summary>
    /// <returns></returns>
    public IEnumerator VoteStart()
    {
        concaveGame.isStreamerTurn = false;
        ChzzkUserClear();
        yield return VoteTimer();
    }

    IEnumerator VoteTimer()
    {
        float time = concaveGame.userTurnTime;
        yield return TimerUpdate(time);

        yield return SetCellVote();
        concaveGame.isStreamerTurn = true;

    }

    public IEnumerator TimerUpdate(float time)
    {
        float maxTime = time;
        while (time > 0 )
        {
            float lastTime = time / maxTime;
            voteTimerSlider.value = lastTime;
            voteTimeText.text = (int)time + "초 남았습니다";
            yield return null;
            time -= Time.deltaTime;
        }
        Debug.Log("000");
        Debug.Log(concaveGame.isStreamerTurn);
        if (concaveGame.isStreamerTurn)
        {
            Debug.Log("1234");

            concaveGame.isBlackTurn = !concaveGame.isBlackTurn;
            concaveGame.isStreamerTurn = false;

            StopAllCoroutines();
            yield return VoteStart();
        }
    }

    IEnumerator SetCellVote()
    {
        if (cellVoteList.Count == 0)
        {
            concaveGame.isBlackTurn = !concaveGame.isBlackTurn;
        }
        else 
        {
            //최댓값을 찾음
            int maxVoteNum = cellVoteList.Max(cellvote => cellvote.voteNum);


            //혹시 2표:2표 등 투표수가 같은경우
            cellVoteList = cellVoteList.Where(cellvote => cellvote.voteNum == maxVoteNum).ToList();

            int random = Random.Range(0, cellVoteList.Count());
            CellPosVote lastVote = cellVoteList[random];

            concaveGame.ChatPlaceStone(lastVote.row, lastVote.col);
        }

        foreach (var item in votePercentText)
        {
            item.gameObject.SetActive(false);
        }
        
        concaveGame.isStreamerTurn = true;

        yield return TimerUpdate(concaveGame.streamerTurnTime);

    }


    void VotePercentUIDraw()
    {
        

        CheckUserVote("a", "a10");
        CheckUserVote("모르는사람", "b1");
        CheckUserVote("모르는사람123", "d10");

        int totalSum = CalculateTotalSum(cellVoteList);
        /*Vector3 Position = concaveGame.cells[3, 3].transform.position;
        votePercentText.transform.position = Camera.main.WorldToScreenPoint(Position);*/
        // 각 값의 확률 계산 및 출력
        for (int i = 0; i < cellVoteList.Count; i++)
        {
            float probability = (float)cellVoteList[i].voteNum / totalSum;
            Debug.Log("Value " + cellVoteList[i] + " has a probability of " + probability);
            Vector3 cellPosition = concaveGame.cells[cellVoteList[i].row, cellVoteList[i].col].transform.position;
            
            if(i < votePercentText.Count)
                votePercentText[i].transform.position = Camera.main.WorldToScreenPoint(cellPosition);
            else
            {
                Debug.Log(i);
                TMP_Text a = Instantiate(votePercentText[0],votePercentCanvas);
                votePercentText.Add(a);
                votePercentText[i].transform.position = Camera.main.WorldToScreenPoint(cellPosition);
            }
            votePercentText[i].gameObject.SetActive(true);
            votePercentText[i].text = (probability*100).ToString("0") + "%";
        }
    }

    // 리스트의 정수 값의 합을 계산하는 메서드
    int CalculateTotalSum(List<CellPosVote> values)
    {
        int sum = 0;
        foreach (CellPosVote value in values)
        {
            sum += value.voteNum;
        }
        return sum;
    }

}


