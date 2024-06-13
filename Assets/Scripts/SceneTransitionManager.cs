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
        // PlayerController.Instance.StopAllCoroutines();
        lastScene = SceneManager.GetActiveScene().name;
        PlayerController.Instance.transform.position = new Vector3(0.00f, 0.77f, -5.47f);
        SceneManager.LoadScene(sceneName);
        Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa");
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
