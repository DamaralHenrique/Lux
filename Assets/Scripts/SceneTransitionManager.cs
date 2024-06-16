using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    private string lastScene;

    void Awake()
    {
        Debug.Log("SceneTransitionManager Awake");
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

    public void LoadScene(string sceneName)
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;;
    }

    public string GetLastScene()
    {
        return lastScene;
    }
}
