using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;

    // �ٸ� �����忡�� ���� ������� �޽����� ������ ���� ť
    private readonly Queue<Action> executionQueue = new Queue<Action>();

    // �Ӽ��� ���� MainThreadDispatcher�� ������ �� �ֵ��� ��
    public static MainThreadDispatcher Instance
    {
        get
        {
            // �ν��Ͻ��� ������ �� �ν��Ͻ��� ����
            if (instance == null)
            {
                GameObject gameObject = new GameObject("MainThreadDispatcher");
                instance = gameObject.AddComponent<MainThreadDispatcher>();
            }
            return instance;
        }
    }

    // Awake �Լ����� �ν��Ͻ��� ����
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �޽����� ť�� �߰��ϴ� Enqueue �޼���
    public void Enqueue(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    // ���� ť�� ��� �۾��� �����ϴ� Update �Լ�
    private void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }
}
