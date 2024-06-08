using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue ObjectText;

    public void Interact()
    {
        StartCoroutine(DialogueManager.Instance.ShowDialogue(ObjectText));
    }
}
