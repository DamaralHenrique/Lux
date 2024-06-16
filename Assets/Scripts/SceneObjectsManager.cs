using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class ObjectState
{
    public string objectName;
    public string sceneName;
    // public Vector3 position;
    public bool isActive;

    public ObjectState(string sceneName, string objectName, bool isActive)
    {
        this.sceneName = sceneName;
        this.objectName = objectName;
        // this.position = position;
        this.isActive = isActive;
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
        new ObjectState("BlueTemple", "Door_frame.001", true),
        new ObjectState("BlueTemple", "Door_frame.002", true),
        new ObjectState("BlueTemple", "BlueTotem", true),
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

    // void Start()
    // {
    //     UpdateStates();
    // }

    void PrintObjectStates()
    {
        foreach (var state in objectStates)
        {
            Debug.Log($"Scene: {state.sceneName}, Object: {state.objectName}, Active: {state.isActive}");
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

            // Old 
            // GameObject statefulObjects = GameObject.Find("StatefulObjects");
            // GameObject obj = statefulObjects.transform.Find(state.objectName).gameObject;
            // if (obj != null)
            // {
            //     // obj.transform.position = state.position;
            //     obj.SetActive(state.isActive);
            //     Debug.Log($"Scene: {state.sceneName}. Object: {state.objectName}. State: {state.isActive}.");
            // }
            // else
            // {
            //     Debug.LogWarning($"Object {state.objectName} not found in the scene.");
            // }

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
                    obj.SetActive(state.isActive);
                }
            }
        }
    }

    public void SetObjectState(string sceneName, string objectName, bool isActive)
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateStates();
    }
}
