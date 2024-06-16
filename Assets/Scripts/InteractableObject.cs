using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue ObjectText;

    private InventoryManager inventoryManager;

    public void Awake()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    public void Interact()
    {
        if (gameObject.name == "altar")
        {
            AddTotemToAltar();
        }

        StartCoroutine(DialogueManager.Instance.ShowDialogue(ObjectText));
    }

    private void AddTotemToAltar()
    {
        // Debug.Log("Adding totem to altar.");

        for (int i = 0; i < inventoryManager.inventory.Count; i++)
        {
            string gameObjectName = inventoryManager.inventory[i];
            SceneObjectsManager.Instance.SetObjectActiveState("MainSquare", gameObjectName, true);
            SceneObjectsManager.Instance.UpdateStates();
        }
    }
}
