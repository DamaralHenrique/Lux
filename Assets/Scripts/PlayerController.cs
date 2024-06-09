using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = .01f;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    public LayerMask floorLayer;
    public LayerMask disappearOnPuzzleCompleteLayer;
    public LayerMask totemLayer;

    public float collisionRadius = 0.2f;
    public float interactRadius = 0.2f;

    private bool isMoving;
    private Vector2 facingDirection;

    private Animator animator;
    private InventoryManager inventoryManager;

    void Awake()
    {
        animator = GetComponent<Animator>();
        inventoryManager = FindObjectOfType<InventoryManager>();
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

                facingDirection = new Vector2(xDirection, zDirection).normalized;

                var targetPos = transform.position;
                targetPos.x += xDirection;
                targetPos.z += zDirection;

                if (IsWalkable(targetPos))
                {
                    float targetHeight = GetTargetHeight(targetPos);
                    targetPos.y = targetHeight;
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryManager.PrintInventory();
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
        Collider[] collisions = Physics.OverlapSphere(
            targetPos, collisionRadius, interactableLayer | solidObjectLayer | disappearOnPuzzleCompleteLayer
        );
        if (collisions.Length != 0)
        {
            // foreach (Collider obj in collisions)
            // {
            //     Debug.Log("Collision with object " + obj.gameObject.name);
            // }
            return false;
        }

        return true;
    }

    private float GetTargetHeight(Vector3 targetPos)
    {
        RaycastHit hitUp;
        RaycastHit hitDown;

        // Raycast upwards to detect climbable objects
        float characterHeight = 0.77f;
        if (Physics.Raycast(targetPos - Vector3.up * characterHeight, Vector3.up, out hitUp, 1.0f, floorLayer))
        {
            Debug.Log("Climbable object");
            Debug.Log("Hit point: " + hitUp.point.y);
            float ascPosY = hitUp.point.y + characterHeight + 0.35f;
            Debug.Log("New pos-y: " + ascPosY);

            return ascPosY;
        }
        
        // Raycast downwards to detect descendable areas
        Physics.Raycast(targetPos, Vector3.down, out hitDown, 100, floorLayer);
        float floorBoxColisionZ = 0.02f;
        float posY = hitUp.point.y + characterHeight + floorBoxColisionZ;

        return posY;
    }

    void Interact()
    {
        var interactPos = transform.position + new Vector3(facingDirection.x, 0, facingDirection.y);

        var NPCCollider = Physics.OverlapSphere(interactPos, interactRadius, interactableLayer);
        if (NPCCollider.Length != 0)
        {
            NPCCollider[0].GetComponent<Interactable>()?.Interact();
        }

        var TotemCollider = Physics.OverlapSphere(interactPos, interactRadius, totemLayer);
        if (TotemCollider.Length != 0)
        {
            string totemName = TotemCollider[0].gameObject.name;
            inventoryManager.AddItem(totemName);
            TotemCollider[0].gameObject.SetActive(false);

            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "RedTemple")
            {
                int layer = LayerMask.NameToLayer("DisappearOnPuzzleComplete");
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

                foreach (GameObject obj in allObjects)
                {
                    if (obj.layer == layer)
                    {
                        Debug.Log(obj.name);
                        obj.SetActive(false);
                    }
                }
            }
        }
    }
}
