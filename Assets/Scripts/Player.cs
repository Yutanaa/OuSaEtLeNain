using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Coroutine movementCoroutine;
    private bool isTeleporting;

    [SerializeField] private List<Vector2> _keyToPress = new List<Vector2>();
    [SerializeField] private int _maxQTE = 3;
    private Vector2 _currentKey;
    public float _gonnaFall = 5;
    public Vector2 _targetToReach;
    public Transform _spawn;


    [SerializeField] public SeauEnMain itemTenu;
    public Vector2 lastRespawnPosition;
    public Animator animator;

    public PlayerInput _playerInput;
    public float movingDistance;
    public LayerMask layerMask;

    public GameObject sprite;

    public Vector2 LookingDirection;

    public bool canMove;
    public bool onRope = false;
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

    private void Update()
    {
        if (!onRope) return;
        animator.SetBool("OnRope", true);
        if (_gonnaFall > 0)
        {
            _gonnaFall -= Time.deltaTime;
            animator.SetFloat("Fall", _gonnaFall);
            transform.position = Vector2.MoveTowards(transform.position, _targetToReach, 0.4f * Time.deltaTime);
            if (Vector2.Distance(transform.position, _targetToReach) < 1)
            {
                animator.SetBool("OnRope", false);
                transform.position = _targetToReach;
                canMove = true;
                onRope = false;
            }
        }
        else
        {
            StartCoroutine(Falling());
        }
    }

    IEnumerator Falling()
    {
        yield return new WaitForSeconds(1);
        animator.SetBool("OnRope", false);
        transform.position = _spawn.position;
        canMove = true;
        onRope = false;
        _gonnaFall = 5;
    }

    public void SetRespawnPoint(Vector2 pos)
    {
        lastRespawnPosition = pos;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        if ((!canMove || isTeleporting)  && !onRope)
            return;

        Vector2 direction = context.ReadValue<Vector2>();


        if (canMove)
        {
            canMove = false;
            if (direction.x != 0)
            {
                sprite.transform.localScale = new Vector3(-direction.x, 1, 1);
            }
            // transform.Translate(direction);
            if (direction.y > 0)
            {
                animator.SetTrigger("WalkingUp");
            }
            else if (direction.y < 0)
            {
                animator.SetTrigger("WalkingDown");
            }
            else
            {

                animator.SetTrigger("Walking");
            }

            movementCoroutine = StartCoroutine(SlidingRoutine(0.25f, new Vector2(transform.position.x + direction.x, transform.position.y + direction.y)));

            LookingDirection = direction; 
        }


        if (onRope)
        {
            if (_currentKey == direction)
            {
                Debug.Log("GG");
                _gonnaFall += 1.5f;
                KeyLoader();
            }
            else
            {
                _gonnaFall -= 1.5f;
                Debug.Log("nique toi");
            }
        }
    }

    public void KeyLoader()
    {
        _currentKey = _keyToPress[UnityEngine.Random.Range(0, _keyToPress.Count)];
        if (_currentKey == new Vector2(1, 0))
        {
            Debug.Log("droite");
        }
        if (_currentKey == new Vector2(-1, 0))
        {
            Debug.Log("gauche");
        }
        if (_currentKey == new Vector2(0, 1))
        {
            Debug.Log("haut");
        }
        if (_currentKey == new Vector2(0, -1))
        {
            Debug.Log("bas");
        }
    }



    public IEnumerator SlidingRoutine(float duree, Vector2 targetPos)
    {
        float elapsed_time = 0;
        Vector2 currentPos = transform.position;
        Vector2 previousPosition = transform.position;

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


        OnStepCompleted?.Invoke(previousPosition);

        movementCoroutine = null;
        canMove = true;
    }
    void OnRelease(InputAction.CallbackContext context)
    {

    }

    public void Respawn()
    {
        transform.position = lastRespawnPosition;
    }

    public void Teleport(Vector2 targetPosition)
    {

        isTeleporting = true;
        canMove = false;

        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        transform.position = targetPosition;


        isTeleporting = false;
        canMove = true;
    }

    public void Interact(InputAction.CallbackContext context)
    {

        Vector2 startPosition = new Vector2(transform.position.x, transform.position.y) + Vector2.one * LookingDirection;

        if (itemTenu != null)
        {
            print("interact item");
            print(itemTenu);
            itemTenu.GetComponent<SeauEnMain>().ShootDirt(LookingDirection, startPosition);
            //itemTenu = null;
            RemoveItem();
        }

        else
        {
            RaycastHit2D hit = Physics2D.Raycast(startPosition, LookingDirection, 1f, layerMask);

            Debug.DrawRay(startPosition, transform.TransformDirection(LookingDirection) * hit.distance, Color.red);
            // Does the ray intersect any objects excluding the player layer
            if (hit)
            {
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

        animator.SetBool("HoldingBucket", true);
        //print(gameo);
    }

    public void RemoveItem()
    {
        animator.SetBool("HoldingBucket", false);
        Destroy(itemTenu.gameObject);
        itemTenu = null;

    }
    void OnDrawGizmos()
    {
        //Vector2 startPosition = new Vector2(transform.position.x, transform.position.y) + Vector2.one * LookingDirection;
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(startPosition, 100f);
    }
}
