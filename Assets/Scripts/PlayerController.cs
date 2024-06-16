using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 0.01f;
    public LayerMask solidObjectLayer;
    public LayerMask interactableLayer;
    public LayerMask floorLayer;
    public LayerMask disappearOnPuzzleCompleteLayer;
    public LayerMask totemLayer;
    public LayerMask portalLayer;

    public float collisionRadius = 0.2f;
    public float interactRadius = 0.2f;
    public float portalRadius = 0.1f;

    public Vector3 startSpawnPosition = Vector3.zero;

    private bool isMoving;
    private Vector2 facingDirection;

    private Animator animator;
    private InventoryManager inventoryManager;

    private Coroutine moveCoroutine;

    public static PlayerController Instance;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        isMoving = false;
        facingDirection = Vector2.zero;

        // Reset da animação para evitar o bug do personagem preso no estado de movimento
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 0);
        }

        SetPlayerPositionOnSceneLoad();
    }

    void Awake()
    {
        Debug.Log("PlayerController Awake");
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        animator = GetComponent<Animator>();
        inventoryManager = FindObjectOfType<InventoryManager>();
        inventoryManager.InitIventory();

        SetPlayerPositionOnSceneLoad();
    }

    void SetPlayerPositionOnSceneLoad()
    {
        Vector3 spawnPosition;
        string lastScene = SceneTransitionManager.Instance.GetLastScene();

        if (lastScene == null)
        {
            Debug.Log("First scene");
            spawnPosition = startSpawnPosition;
        }
        else
        {
            Debug.Log("Last scene: " + lastScene);
            spawnPosition = GameObject.Find("SpawnPoint" + lastScene).transform.position;
        }

        transform.position = spawnPosition;
        Debug.Log("Player position: " + transform.position);
    }

    // private void Start()
    // {
    //     Debug.Log("PlayerController Start");
    // }

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

                    if (moveCoroutine != null)
                    {
                        StopCoroutine(moveCoroutine);
                    }
                    moveCoroutine = StartCoroutine(Move(targetPos));
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
        moveCoroutine = null;
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
        float characterHeight = 0.4f;
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
