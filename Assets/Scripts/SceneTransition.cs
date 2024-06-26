using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public string spawnPointSuffix;

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("OnTriggerEnter");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trocando para a cena " + sceneToLoad + "...");
            SceneTransitionManager.Instance.LoadScene(sceneToLoad, spawnPointSuffix);
            // SceneManager.LoadSceneAsync(sceneToLoad);
        }
    }
}
