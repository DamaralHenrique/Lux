using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnter : MonoBehaviour
{
    [SerializeField] NPCController npcController;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("SceneEnter OnTriggerEnter");
        if(npcController != null){
            Debug.Log("npcController.interactOnScreenLoad: " + npcController.interactOnScreenLoad);
            Debug.Log("npcController.hasInteractOnScreenLoad: " + npcController.hasInteractOnScreenLoad);
        }
        if(npcController != null && npcController.interactOnScreenLoad && !npcController.hasInteractOnScreenLoad){
            Debug.Log("Interact on screen load");
            npcController.InteractOnScreenLoad();
            return;
        }
    }
}