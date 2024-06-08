using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Default, Dialogue}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] NPCController npcController;

    GameState state;
    private NPCController currentNPC;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        DialogueManager.Instance.OnPasswordCorrect += () =>
        {
            if (currentNPC != null)
            {
                Vector3 displacement = CalculateDisplacement();
                currentNPC.MoveNPC(displacement);
            }
        };
    }

    public void SetCurrentNPC(NPCController npc)
    {
        currentNPC = npc;
    }

    private Vector3 CalculateDisplacement() 
    {
        if (playerController.transform.position.x > currentNPC.transform.position.x)
        {
            return new Vector3(-1, 0, 0);
        }
        else
        {
            return new Vector3(1, 0, 0);
        }
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
