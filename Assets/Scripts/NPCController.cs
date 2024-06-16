using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue initialDialogue;
    [SerializeField] Dialogue puzzleCompleteDialogue;
    [SerializeField] float moveSpeed = 0.05f;
    [SerializeField] List<int> showInputAtLine = new List<int> {};
    [SerializeField] List<int> showChoiceAtLine = new List<int> {};
    [SerializeField] List<ActionLine> doActionAtLine = new List<ActionLine> {};
    [SerializeField] public bool interactOnScreenLoad = false;

    public bool hasInteractOnScreenLoad = false;

    public bool isInitialDialog = true;

    private Dialogue currentDialogue;

    private void Awake()
    {
        currentDialogue = initialDialogue;
    }

    public void InteractOnScreenLoad()
    {
        hasInteractOnScreenLoad = true;
        this.Interact();
    }

    public void Interact()
    {
        GameController.Instance.SetCurrentNPC(this);
        StartCoroutine(DialogueManager.Instance.ShowDialogue(currentDialogue, showInputAtLine, showChoiceAtLine, doActionAtLine));
    }

    public void ChangeToPuzzleCompleteDialogue()
    {
        currentDialogue = puzzleCompleteDialogue;
    }

    public void ChangeDialogue(bool useInitialDialogue)
    {

        hasInteractOnScreenLoad = !useInitialDialogue;
        isInitialDialog = useInitialDialogue;
        currentDialogue = useInitialDialogue ? initialDialogue : puzzleCompleteDialogue;
    }

    public void MoveNPC(Vector3 displacement)
    {
        Vector3 targetPos = transform.position + displacement;
        StartCoroutine(Move(targetPos));
    }

    public void TeleportNPC(Vector3 position)
    {
        transform.position = position;
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
    }
}
