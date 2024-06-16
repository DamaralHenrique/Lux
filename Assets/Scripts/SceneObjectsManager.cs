using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class ObjectState
{
    public string objectName;
    public string sceneName;
    public bool isActive;
    public bool useInitialDialogue;
    public Vector3? position; // ? = nullable

    public ObjectState(
        string sceneName, 
        string objectName, 
        bool isActive, 
        bool useInitialDialogue=true,
        Vector3? position=null
    )
    {
        this.sceneName = sceneName;
        this.objectName = objectName;
        this.isActive = isActive;
        this.useInitialDialogue = useInitialDialogue;
        this.position = position;
    }
}

public class SceneObjectsManager : MonoBehaviour
{
    // Listar aqui todos os objetos que mudam de estado ao longo do jogo 
    // (ex: mudam de posição, são ativados/desativados, etc.)
    // Obs: adicionar na lista o estado inicial deles
    private List<ObjectState> objectStates = new List<ObjectState>
    {
        new ObjectState("MainSquare", "BlueTotem", false),
        new ObjectState("MainSquare", "RedTotem", false),
        new ObjectState("MainSquare", "GreenTotem", false),
        new ObjectState("MainSquare", "NPC RED - Guarda", true),
        new ObjectState("MainSquare", "Door", true),
        new ObjectState("BlueTemple", "Door_frame.001", true),
        new ObjectState("BlueTemple", "Door_frame.002", true),
        new ObjectState("BlueTemple", "BlueTotem", true),
        new ObjectState("BlueTemple", "NPC BLUE", true),
        new ObjectState("RedTemple", "NPC RED", true),
        new ObjectState("RedTemple", "RedTotem", true),
        new ObjectState("RedTemple", "Door", true),
        new ObjectState("LivingRoom", "NPC GREEN", true),
    };

    public static SceneObjectsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void PrintObjectStates()
    {
        foreach (var state in objectStates)
        {
            Debug.Log($"Scene: {state.sceneName}, Object: {state.objectName}, Active: {state.isActive}, useInitialDialogue: {state.useInitialDialogue}");
        }
    }

    public void UpdateStates()
    {
        // Debug.Log("Updating object states...");
        // PrintObjectStates();
        foreach (var state in objectStates)
        {
            if (state.sceneName != SceneManager.GetActiveScene().name)
            {
                continue;
            }

            int totemLayer = LayerMask.NameToLayer("Totem");
            int npcLayer = LayerMask.NameToLayer("NPC");
            int disappearOnPuzzleCompleteLayer = LayerMask.NameToLayer("DisappearOnPuzzleComplete");
            List<int> layers = new List<int> { totemLayer, npcLayer, disappearOnPuzzleCompleteLayer };

            // Obs: GameObject.FindObjectsOfType<GameObject>() só encontra objetos ativos.
            // Para também encontrar objetos inativos, usar a linha abaixo.
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (layers.Contains(obj.layer) && obj.name == state.objectName)
                {
                    // Debug.Log(obj.name);
                    // Debug.Log(state.isActive);

                    // Update active state
                    obj.SetActive(state.isActive);
                    if (obj.layer == npcLayer)
                    {
                        // Update dialogue
                        obj.GetComponent<NPCController>().ChangeDialogue(state.useInitialDialogue);
                        
                        // Update position
                        if (state.position != null)
                        {
                            obj.GetComponent<NPCController>().TeleportNPC((Vector3)state.position);
                        }
                    }
                }
            }
        }
    }

    public void SetObjectActiveState(string sceneName, string objectName, bool isActive)
    {
        foreach (var state in objectStates)
        {
            if (state.sceneName == sceneName && state.objectName == objectName)
            {
                state.isActive = isActive;
                break;
            }
        }
    }

    public void SetObjectDialogueState(string sceneName, string objectName, bool useInitialDialogue)
    {
        foreach (var state in objectStates)
        {
            if (state.sceneName == sceneName && state.objectName == objectName)
            {
                state.useInitialDialogue = useInitialDialogue;
                break;
            }
        }
    }

    public void SetObjectPositionState(string sceneName, string objectName, Vector3 newPosition)
    {
        foreach (var state in objectStates)
        {
            if (state.sceneName == sceneName && state.objectName == objectName)
            {
                state.position = newPosition;
                break;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateStates();
    }
}
