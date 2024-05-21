using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Default, Dialogue}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    GameState state;

    void Start()
    {
        DialogueManager.Instance.OnShowDialogue += () => 
        {
            state = GameState.Dialogue;
        };
        DialogueManager.Instance.OnCloseDialogue += () => 
        {
            if (state == GameState.Dialogue)
            {
                state = GameState.Default;
            }
        };
    }

    private void Update()
    {
        if (state == GameState.Default)
        {
            playerController.HandleUpdate();
        }        
        else if (state == GameState.Dialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }
}
