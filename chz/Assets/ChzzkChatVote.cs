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


    [Header("��ǥ Ȯ�� �ۼ�Ʈ")]
    [SerializeField]
    Transform votePercentCanvas;
    [SerializeField]
    List<TMP_Text> votePercentText;
    private IEnumerator Start()
    {
        concaveGame = GameObject.FindObjectOfType<Concave>();
        ChzzkUserClear();
        CheckUserVote("a","a10");
        CheckUserVote("�𸣴»��", "b1");
        CheckUserVote("�𸣴»��123", "a10");
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
    /// �ߺ� ��ǥ�� ���� �ʵ��� �г����� üũ�ϴ� �Լ�
    /// </summary>
    public void CheckUserVote(string nickName, string str)
    {
        if (concaveGame.isStreamerTurn) //�������� ��ǥ �ð��� �ƴѰ��
            return;

        if (chzzkUserList.Contains(nickName)) //�г������� �̹� ��ǥ�� �� ���
            return;
        else
        {
            ChatCheck(nickName,str);
        }
    }

    public void ChatCheck(string nickName, string str)
    {
        // ������ ��ġ�� 3���ڸ� �Ѿ�� ����
        if (str.Length > 3)
            return;
        char firstChar = str[0];

        //��ҹ��ڸ� ���� �ҹ��ڷ�
        int asciiValue = char.ToLower(firstChar);
        str = str.ToLower();
        // 'a'���� �� ũ������� ASCII �� ����
        int minAscii = (int)'a';
        int maxAscii = minAscii + concaveGame.boardSize - 1; // �� ����� �°�

        // ù ���ڰ� �� �ȿ� �ִ� ��ġ�� ���
        if (asciiValue >= minAscii && asciiValue <= maxAscii)
        {
            //������ ���� �ٵ��ͼ� �������� Ȯ��
            string col = str.Substring(1, str.Length - 1);
            int.TryParse(col, out int colNum);

            //������ ���ڰ� �� �ȿ� �ִ� ��ġ�� ���
            if (colNum >= 1 && colNum <= concaveGame.boardSize)
            {

                chzzkUserList.Add(nickName);
                //�ƽ�Ű�ڵ� �� ��� �� �迭 ��ġ���� 0,0 �� �ǵ��� ����
                asciiValue -= 97;
                colNum -= 1;

                CellPosVote cellUpdate = cellVoteList.Find(cellvote => cellvote.position == str);
                if(cellUpdate == null)
                {
                    //��ҹ��ڸ� ���� �ҹ��ڷ�
                    cellUpdate = new CellPosVote { position = str, voteNum = 1, row = asciiValue, col = colNum };
                    cellVoteList.Add(cellUpdate);
                }
                else
                {
                    cellUpdate.voteNum += 1;
                }
                //cellPositionList.Add(new List<int> { 1, 1 });
                //�� ����
                //ChatPlaceStone(asciiValue , colNum);
            }
        }

        return;
    }

    /// <summary>
    /// ���� ��ǥ ����
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
            voteTimeText.text = (int)time + "�� ���ҽ��ϴ�";
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
            //�ִ��� ã��
            int maxVoteNum = cellVoteList.Max(cellvote => cellvote.voteNum);


            //Ȥ�� 2ǥ:2ǥ �� ��ǥ���� �������
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
        CheckUserVote("�𸣴»��", "b1");
        CheckUserVote("�𸣴»��123", "d10");

        int totalSum = CalculateTotalSum(cellVoteList);
        /*Vector3 Position = concaveGame.cells[3, 3].transform.position;
        votePercentText.transform.position = Camera.main.WorldToScreenPoint(Position);*/
        // �� ���� Ȯ�� ��� �� ���
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

    // ����Ʈ�� ���� ���� ���� ����ϴ� �޼���
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


