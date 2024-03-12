using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcaveCell : MonoBehaviour
{
    private bool isOccupied = false; // ���� ���� �����Ǿ����� ���θ� ��Ÿ��
    private bool isBlackStone; // ������ ���� �浹������ ��Ÿ��

    [SerializeField]
    private GameObject onMouseEft;
    [SerializeField]
    public int rowPosition;
    [SerializeField]
    public int colPosition;
    // ���� ���� �����ϴ� �Լ�
    private void Start()
    {
        onMouseEft.SetActive(false);
    }
    public void OccupyCell(bool isBlack)
    {
        isOccupied = true;
        isBlackStone = isBlack;
    }

    // ���� �����Ǿ����� ���θ� Ȯ���ϴ� �Լ�
    public bool IsOccupied()
    {
        return isOccupied;
    }

    // ������ ���� �浹���� Ȯ���ϴ� �Լ�
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
