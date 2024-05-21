using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = .01f;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;

    private bool isMoving;
    // private Vector2 input;

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

            if (xDirection != 0.0f | zDirection != 0.0f)
            {
                animator.SetFloat("moveX", xDirection);
                animator.SetFloat("moveY", zDirection);

                var targetPos = transform.position;
                targetPos.x += xDirection;
                targetPos.z += zDirection;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }

                // Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);

                // transform.position += moveDirection * speed;
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.F))
        {
            // Debug.Log("kEY PRessed: f");
            Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
            // transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime);

            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        Collider [] collisions = Physics.OverlapSphere(targetPos, 0.15f, interactableLayer | solidObjectLayer);
        if (collisions.Length != 0)
        {
            foreach(Collider obj in collisions)
            {
                Debug.Log("Collision with object " + obj.gameObject.name);
            }
            return false;
        }

        return true;
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        // Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);
        var collider = Physics.OverlapSphere(interactPos, 0.15f, interactableLayer);
        if (collider.Length != 0)
        {
            collider[0].GetComponent<Interactable>()?.Interact();
        }
    }
}
