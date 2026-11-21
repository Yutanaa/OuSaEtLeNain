using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] public SeauEnMain itemTenu;
    public Vector2 lastRespawnPosition;
    public Animator animator;

    public PlayerInput _playerInput;
    public float movingDistance;
    public LayerMask layerMask;

    public GameObject sprite;

    public Vector2 LookingDirection;

    public bool canMove;

    public event Action<Vector2> OnStepCompleted;

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        if (_playerInput == null)
        {
            Debug.Log("Player input missing");
            return;
        }
        _playerInput.actions["Move"].performed += OnMove;
        _playerInput.actions["Move"].canceled += OnRelease;
        _playerInput.actions["Interaction"].performed += Interact;
    }

    void Start()
    {
        LookingDirection = -transform.right;
    }

    public void SetRespawnPoint(Vector2 pos)
    {
        lastRespawnPosition = pos;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();

        if (canMove)
        {
            canMove = false;
            if (direction.x != 0)
            {
                sprite.transform.localScale = new Vector3(-direction.x, 1, 1);
            }
            // transform.Translate(direction);
            StartCoroutine(SlidingRoutine(0.25f, new Vector2(transform.position.x + direction.x, transform.position.y + direction.y)));

            LookingDirection = direction;
        }
    }

    public IEnumerator SlidingRoutine(float duree, Vector2 targetPos)
    {
        float elapsed_time = 0;
        Vector2 currentPos = transform.position;

        Vector2 previousPosition = transform.position;
        animator.SetTrigger("Walking");

        while (elapsed_time <= duree)
        {
            currentPos = Vector2.Lerp(currentPos, targetPos, elapsed_time / duree);

            transform.localPosition = currentPos;

            if (currentPos == targetPos)
            {
                break;
            }

            yield return null;

            elapsed_time += Time.deltaTime;
        }

        canMove = true;
        OnStepCompleted?.Invoke(previousPosition);
        yield return null;
    }

    void OnRelease(InputAction.CallbackContext context)
    {

    }

    public void Respawn()
    {
        transform.position = lastRespawnPosition;
    }

    public void Interact(InputAction.CallbackContext context)
    {

        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y) + Vector2.one * LookingDirection;

        if (itemTenu != null)
        {
            print("interact item");
            print(itemTenu);
            itemTenu.GetComponent<SeauEnMain>().ShootDirt(startPosition, LookingDirection);
            itemTenu = null;
        }

        else
        {
            RaycastHit2D hit = Physics2D.Raycast(startPosition, LookingDirection, 1f, layerMask);

            // Does the ray intersect any objects excluding the player layer
            if (hit)
            {

                Debug.DrawRay(startPosition, transform.TransformDirection(LookingDirection) * hit.distance, Color.red);

                if (hit.collider.GetComponent<SeauAuSol>())
                {
                    AddItem(hit.collider.GetComponent<SeauAuSol>().GiveMeThatBucket(transform));
                }
                else
                {
                    hit.collider?.GetComponent<IInteractible>().Interaction();
                }
            }
            else
            {
                Debug.DrawRay(startPosition, transform.TransformDirection(LookingDirection) * 1f, Color.yellow);
                // Debug.Log("Did not Hit");
            }
        }
    }


    public void AddItem(SeauEnMain gameo)
    {
        itemTenu = gameo;
        itemTenu.transform.SetParent(transform);
        itemTenu.transform.position = transform.position;
        print(gameo);
    }
    void OnDrawGizmos()
    {
        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y) + Vector2.one * LookingDirection;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPosition, 100f);
    }
}
