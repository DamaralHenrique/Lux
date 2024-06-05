using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue initialDialogue;
    [SerializeField] Dialogue puzzleCompleteDialogue;
    [SerializeField] float moveSpeed = 0.05f;

    private Dialogue currentDialogue;

    private void Start()
    {
        currentDialogue = initialDialogue;
    }

    public void Interact()
    {
        GameController.Instance.SetCurrentNPC(this);
        StartCoroutine(DialogueManager.Instance.ShowDialogue(currentDialogue));
    }

    public void ChangeToPuzzleCompleteDialogue()
    {
        currentDialogue = puzzleCompleteDialogue;
    }

    public void MoveNPC(Vector3 displacement)
    {
        Vector3 targetPos = transform.position + displacement;
        StartCoroutine(Move(targetPos));
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
