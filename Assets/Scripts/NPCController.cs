using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] float moveSpeed = 1f;

    private bool isMoving;

    public void Interact()
    {
        GameController.Instance.SetCurrentNPC(this);
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
    }

    public void MoveNPC(Vector3 displacement)
    {
        Vector3 targetPos = transform.position + displacement;
        StartCoroutine(Move(targetPos));
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }
}
