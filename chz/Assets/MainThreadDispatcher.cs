using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;

    // 다른 스레드에서 메인 스레드로 메시지를 보내기 위한 큐
    private readonly Queue<Action> executionQueue = new Queue<Action>();

    // 속성을 통해 MainThreadDispatcher에 접근할 수 있도록 함
    public static MainThreadDispatcher Instance
    {
        get
        {
            // 인스턴스가 없으면 새 인스턴스를 만듦
            if (instance == null)
            {
                GameObject gameObject = new GameObject("MainThreadDispatcher");
                instance = gameObject.AddComponent<MainThreadDispatcher>();
            }
            return instance;
        }
    }

    // Awake 함수에서 인스턴스를 설정
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

    // 메시지를 큐에 추가하는 Enqueue 메서드
    public void Enqueue(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }

    // 실행 큐의 모든 작업을 실행하는 Update 함수
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
