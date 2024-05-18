using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    public float speed = .015f;
    private bool isMoving;
    // private Vector2 input;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
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

                StartCoroutine(Move(targetPos));

                // Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);

                // transform.position += moveDirection * speed;
            }
        }

        animator.SetBool("isMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
            // transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }
}
