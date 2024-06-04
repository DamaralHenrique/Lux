using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = .01f;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    public float collisionRadius = 0.2f;
    public float interactRadius = 0.2f;

    private bool isMoving;
    private Vector2 facingDirection;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            float xDirection = Input.GetAxisRaw("Horizontal");
            float zDirection = Input.GetAxisRaw("Vertical");
            
            // Remove diagonal movement
            if (xDirection != 0.0f) zDirection = 0.0f;

            if (xDirection != 0.0f || zDirection != 0.0f)
            {
                animator.SetFloat("moveX", xDirection);
                animator.SetFloat("moveY", zDirection);

                // Update facing direction
                facingDirection = new Vector2(xDirection, zDirection).normalized;

                var targetPos = transform.position;
                targetPos.x += xDirection;
                targetPos.z += zDirection;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        Collider[] collisions = Physics.OverlapSphere(targetPos, collisionRadius, interactableLayer | solidObjectLayer);
        if (collisions.Length != 0)
        {
            foreach (Collider obj in collisions)
            {
                Debug.Log("Collision with object " + obj.gameObject.name);
            }
            return false;
        }

        return true;
    }

    void Interact()
    {
        var interactPos = transform.position + new Vector3(facingDirection.x, 0, facingDirection.y);

        Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);
        var collider = Physics.OverlapSphere(interactPos, interactRadius, interactableLayer);
        if (collider.Length != 0)
        {
            collider[0].GetComponent<Interactable>()?.Interact();
        }
    }
}
