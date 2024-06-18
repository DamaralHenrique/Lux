using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if (gameObject.name == "altar_pilar")
        {
            AddTotemToAltar();
            GameController.Instance.CheckGameClear();
        }

        StartCoroutine(DialogueManager.Instance.ShowDialogue(ObjectText));
    }

    private void AddTotemToAltar()
    {
        int addedTotems = 0;
        ObjectText.ClearLines();

        for (int i = 0; i < inventoryManager.inventory.Count; i++)
        {
            string gameObjectName = inventoryManager.inventory[i];
            if (!SceneObjectsManager.Instance.IsObjectActive("MainSquare", gameObjectName))
            {
                SceneObjectsManager.Instance.SetObjectActiveState("MainSquare", gameObjectName, true);
                addedTotems++;
            }
        }
        SceneObjectsManager.Instance.UpdateStates();

        string lines;
        if (addedTotems > 0)
        {
            lines = "Totens adicionados no altar.";
        }
        else
        {
            lines = "Não há totens para serem adicionados.";
        }

        int totalActiveTotems = CountActiveTotems();
        int missingTotems = 3 - (totalActiveTotems);

        lines += $" Quantidade de totens faltantes: {missingTotems}.";

        ObjectText.AddLine(lines);
    }

    private int CountActiveTotems()
    {
        int count = 0;
        if (SceneObjectsManager.Instance.IsObjectActive("MainSquare", "BlueTotem")) count++;
        if (SceneObjectsManager.Instance.IsObjectActive("MainSquare", "RedTotem")) count++;
        if (SceneObjectsManager.Instance.IsObjectActive("MainSquare", "GreenTotem")) count++;
        return count;
    }
}