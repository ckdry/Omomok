using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Concave : MonoBehaviour
{
    public GameObject omokCellPrefab;
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;

    [SerializeField]
    private ConcaveCell[] cellpos;
    public GameObject[,] cells;
    private GameObject currentStone;
    public bool isBlackTurn = true;

    public bool isStreamerTurn = true;

    [SerializeField]
    private GameObject gameBoardTrf;

    [SerializeField]
    public int boardSize = 10;

    [SerializeField]
    ChzzkChatVote chzzkChatVote;

    public GameObject settingPnl;
    public Button gameStartBtn;
    public GameObject gameBoard;

    public GameObject gameRuleSetPnl;

    public int streamerTurnTime = 15;
    public int userTurnTime = 15;

    [SerializeField]
    Image[] cellColorImgs;

    [SerializeField]
    Button[] timeSetBtns;
    [SerializeField]
    TMP_Text[] timeTexts;

    // Start is called before the first frame update
    void Start()
    {
        chzzkChatVote = GameObject.FindObjectOfType<ChzzkChatVote>();
        gameStartBtn.interactable = false;
        gameRuleSetPnl.SetActive(false);
        gameBoard.SetActive(false);
        cells = new GameObject[boardSize, boardSize];
        foreach (var cell in cellpos)
        {
            cells[cell.rowPosition, cell.colPosition] = cell.gameObject;
        }

        timeTexts[0].text = streamerTurnTime.ToString();
        timeTexts[1].text = userTurnTime.ToString();

        timeSetBtns[0].onClick.AddListener(() => TimerSecBtn(ref streamerTurnTime,  5, timeTexts[0]));
        timeSetBtns[1].onClick.AddListener(() => TimerSecBtn(ref streamerTurnTime, -5, timeTexts[0]));
        timeSetBtns[2].onClick.AddListener(() => TimerSecBtn(ref userTurnTime,      5, timeTexts[1]));
        timeSetBtns[3].onClick.AddListener(() => TimerSecBtn(ref userTurnTime,     -5, timeTexts[1]));

        gameStartBtn.onClick.AddListener(() => GameStart());


        //CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isStreamerTurn)
        {
            PlaceStone();
        }
    }
    public void SettingPnlOpen()
    {

        gameBoard.SetActive(false);
        settingPnl.SetActive(false);
        gameRuleSetPnl.SetActive(true);
    }
    void GameStart()
    {

        //CreateBoard();
        settingPnl.SetActive(false);
        gameRuleSetPnl.SetActive(false);
        gameBoard.SetActive(true);

        isBlackTurn = true;
        if (isStreamerTurn)
            StartCoroutine( chzzkChatVote.TimerUpdate(streamerTurnTime));
        else
            StartCoroutine(chzzkChatVote.VoteStart());
    }
    void CreateBoard()
    {

        float cellSize = omokCellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        Vector2 startPos = gameBoardTrf.GetComponent<Transform>().position;
        startPos = new Vector2((int)(startPos.x - boardSize / 2 * cellSize), (int)(startPos.y - boardSize / 2 * cellSize));
        //new Vector2(-boardSize / 2 * cellSize, -boardSize / 2 * cellSize);

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                GameObject cell = Instantiate(omokCellPrefab, new Vector2(startPos.x + x * cellSize, startPos.y + y * cellSize), Quaternion.identity);
                cell.GetComponent<ConcaveCell>().SetPositionNum(x, y);
                cells[x, y] = cell;
            }
        }
    }

    void PlaceStone()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            ConcaveCell cell = hit.collider.GetComponent<ConcaveCell>();
            if (cell != null && !cell.IsOccupied())
            {
                GameObject stonePrefab = isBlackTurn ? blackStonePrefab : whiteStonePrefab;
                currentStone = Instantiate(stonePrefab, cell.transform.position, Quaternion.identity);
                cell.OccupyCell(isBlackTurn);

                isStreamerTurn = !isStreamerTurn;

                StopAllCoroutines();
                StartCoroutine(chzzkChatVote.VoteStart());


                if (CheckWinCondition(cell.rowPosition, cell.colPosition, isBlackTurn))
                {
                    GameEnd();
                }

                isBlackTurn = !isBlackTurn;
            }
        }
    }

    public void ChatPlaceStone(int row, int col)
    {
        Debug.Log(row);
        Debug.Log(col);
        //ConcaveCell cell = hit.collider.GetComponent<ConcaveCell>();
        ConcaveCell cell = cells[row, col].GetComponent<ConcaveCell>();
        GameObject stonePrefab = isBlackTurn ? blackStonePrefab : whiteStonePrefab;
        currentStone = Instantiate(stonePrefab, cell.transform.position, Quaternion.identity);
        cell.OccupyCell(isBlackTurn);
        isStreamerTurn = !isStreamerTurn;
        if (CheckWinCondition(row, col, isBlackTurn))
        {
            GameEnd();
        }

        isBlackTurn = !isBlackTurn;

    }
    void GameEnd()
    {
        Debug.Log(isBlackTurn ? "Black Player Wins!" : "White Player Wins!");
        chzzkChatVote.StopAllCoroutines();
        StopAllCoroutines();
        SettingPnlOpen();
    }
    

    bool CheckWinCondition(int x, int y, bool isBlack)
    {
        int horizontalCount = 1;
        int verticalCount = 1;
        int diagonalCount1 = 1; // 대각선 방향 \
        int diagonalCount2 = 1; // 대각선 방향 /

        int dx, dy;

        Debug.Log(x);
        Debug.Log(y);
        // 가로 방향 검사
        for (dx = x + 1; dx < boardSize; dx++)
        {
            if (cells[dx, y].GetComponent<ConcaveCell>().IsOccupied() && cells[dx, y].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                horizontalCount++;
            }
            else
            {
                break;
            }
        }
        for (dx = x - 1; dx >= 0; dx--)
        {
            if (cells[dx, y].GetComponent<ConcaveCell>().IsOccupied() && cells[dx, y].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                horizontalCount++;
            }
            else
            {
                break;
            }
        }

        // 세로 방향 검사
        for (dy = y + 1; dy < boardSize; dy++)
        {
            if (cells[x, dy].GetComponent<ConcaveCell>().IsOccupied() && cells[x, dy].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                verticalCount++;
            }
            else
            {
                break;
            }
        }
        for (dy = y - 1; dy >= 0; dy--)
        {
            if (cells[x, dy].GetComponent<ConcaveCell>().IsOccupied() && cells[x, dy].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                verticalCount++;
            }
            else
            {
                break;
            }
        }

        // 대각선 방향 검사 (대각선 \)
        for (dx = x + 1, dy = y + 1; dx < boardSize && dy < boardSize; dx++, dy++)
        {
            if (cells[dx, dy].GetComponent<ConcaveCell>().IsOccupied() && cells[dx, dy].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                diagonalCount1++;
            }
            else
            {
                break;
            }
        }
        for (dx = x - 1, dy = y - 1; dx >= 0 && dy >= 0; dx--, dy--)
        {
            if (cells[dx, dy].GetComponent<ConcaveCell>().IsOccupied() && cells[dx, dy].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                diagonalCount1++;
            }
            else
            {
                break;
            }
        }

        // 대각선 방향 검사 (대각선 /)
        for (dx = x + 1, dy = y - 1; dx < boardSize && dy >= 0; dx++, dy--)
        {
            if (cells[dx, dy].GetComponent<ConcaveCell>().IsOccupied() && cells[dx, dy].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                diagonalCount2++;
            }
            else
            {
                break;
            }
        }
        for (dx = x - 1, dy = y + 1; dx >= 0 && dy < boardSize; dx--, dy++)
        {
            if (cells[dx, dy].GetComponent<ConcaveCell>().IsOccupied() && cells[dx, dy].GetComponent<ConcaveCell>().IsBlackStone() == isBlack)
            {
                diagonalCount2++;
            }
            else
            {
                break;
            }
        }

        // 육목 판별
        if (horizontalCount >= 6 || verticalCount >= 6 || diagonalCount1 >= 6 || diagonalCount2 >= 6)
        {
            return false;
        }

        return (horizontalCount >= 5 || verticalCount >= 5 || diagonalCount1 >= 5 || diagonalCount2 >= 5);
    }



    /// <summary>
    /// 시간 버튼 세팅
    /// </summary>
    /// <param name="whoTime">대상이 될 변수 userTurnTime,streamerTurnTime</param>
    /// <param name="time">추가 혹은 감소할 시간</param>
    void TimerSecBtn(ref int whoTime, int time, TMP_Text text)
    {
        if(whoTime + time > 0)
            whoTime += time;
        text.text = whoTime.ToString();
    }

    public void ChangeCellColor()
    {
        //isBlackTurn = !isBlackTurn;
        isStreamerTurn = !isStreamerTurn;
        SetChangeImage();
    }
    void SetChangeImage()
    {
        if (isStreamerTurn)
        {
            cellColorImgs[0].color = new Color(0, 0, 0);
            cellColorImgs[1].color = new Color(1, 1, 1);
        }
        else
        {
            cellColorImgs[0].color = new Color(1, 1, 1);
            cellColorImgs[1].color = new Color(0, 0, 0);
        }
    }

}
