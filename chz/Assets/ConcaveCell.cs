using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcaveCell : MonoBehaviour
{
    private bool isOccupied = false; // 셀이 돌에 점유되었는지 여부를 나타냄
    private bool isBlackStone; // 점유된 돌이 흑돌인지를 나타냄

    [SerializeField]
    private GameObject onMouseEft;
    [SerializeField]
    public int rowPosition;
    [SerializeField]
    public int colPosition;
    // 셀을 돌로 점유하는 함수
    private void Start()
    {
        onMouseEft.SetActive(false);
    }
    public void OccupyCell(bool isBlack)
    {
        isOccupied = true;
        isBlackStone = isBlack;
    }

    // 셀이 점유되었는지 여부를 확인하는 함수
    public bool IsOccupied()
    {
        return isOccupied;
    }

    // 점유된 돌이 흑돌인지 확인하는 함수
    public bool IsBlackStone()
    {
        return isBlackStone;
    }

    public void SetPositionNum(int row, int col)
    {
        rowPosition = row;
        colPosition = col;
    }

    private void OnMouseEnter()
    {
        onMouseEft.SetActive(true);
    }
    private void OnMouseExit()
    {
        onMouseEft.SetActive(false);
    }
}
