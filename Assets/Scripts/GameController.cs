using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Debug.Log("GameController Awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("GameController Start");

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

                // Salva a nova posição do NPC após a resolução do puzzle.
                // Assim, mesmo ao trocar de cenas, a informação do NPC é mantida.
                Vector3 newPosition = currentNPC.transform.position + displacement;
                SceneObjectsManager.Instance.SetObjectPositionState(
                    SceneManager.GetActiveScene().name, 
                    currentNPC.name, 
                    newPosition
                );
            }
        };
        DialogueManager.Instance.OnCubePuzzleComplete += () =>
        {
            Debug.Log("DisappearOnPuzzleComplete");
            int layer = LayerMask.NameToLayer("DisappearOnPuzzleComplete");
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.layer == layer)
                {
                    Debug.Log(obj.name);
                    obj.SetActive(false);
                    SceneObjectsManager.Instance.SetObjectActiveState(
                        SceneManager.GetActiveScene().name, 
                        obj.name, 
                        false
                    ); // Salva o estado de "portas abertas" pós conclusão do puzzle
                }
            }
        };
    }

    public void SetCurrentNPC(NPCController npc)
    {
        currentNPC = npc;
    }

    public NPCController GetCurrentNPC()
    {
        return currentNPC;
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
