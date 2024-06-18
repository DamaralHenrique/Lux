using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComplete : MonoBehaviour
{
    public Action OnSkipGameCompleteMessage;
    public static GameComplete Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            OnSkipGameCompleteMessage?.Invoke();
        }
    }
}
