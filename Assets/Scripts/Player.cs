using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Coroutine movementCoroutine;
    private bool isTeleporting;

    [SerializeField] private List<Vector2> _keyToPress = new List<Vector2>();
    private Vector2 _currentKey;
    public float _gonnaFall = 3;
    public Vector2 _targetToReach;
    public Transform _spawn;

    public GameObject _canva;
    public Image _image;
    public Sprite _upKeyUi;
    public Sprite _downKeyUi;
    public Sprite _leftKeyUi;
    public Sprite _rightKeyUi;


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

    [Header("Collision")]
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private LayerMask movementObstacleMask;
    private readonly RaycastHit2D[] movementHits = new RaycastHit2D[8];

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
                _canva.SetActive(false);
                animator.SetBool("OnRope", false);
                transform.position = _targetToReach;
                onRope = false;
                StartCoroutine(WaitBeforeWalk());
            }
        }
        else
        {
            StartCoroutine(Falling());
        }
    }

    IEnumerator Falling()
    {
        _canva.SetActive(false);
        yield return new WaitForSeconds(0.5f);
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

            // Vérifie si le déplacement est possible.
            if (!CanMove(direction))
            {
                canMove = true;
                return;
            }

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
            Vector2 destination = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y);

            //RaycastHit2D hit = Physics2D.Raycast(transform.position, LookingDirection, 1f, movementObstacleMask);
            //Debug.DrawRay(transform.position, transform.TransformDirection(LookingDirection) * 1f, Color.green);

            //if (hit.collider == null)
            //{
                movementCoroutine = StartCoroutine(SlidingRoutine(0.25f, destination));
            //}
            //else
            //{
            //    canMove = true;
            //}
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

    IEnumerator WaitBeforeWalk()
    {
        canMove = false;
        yield return new WaitForSeconds(1.5f);
        canMove = true;
    }
    public void KeyLoader()
    {
        _currentKey = _keyToPress[UnityEngine.Random.Range(0, _keyToPress.Count)];
        if (_currentKey == new Vector2(1, 0))
        {
            _image.sprite = _rightKeyUi;
            Debug.Log("droite");
        }
        if (_currentKey == new Vector2(-1, 0))
        {
            _image.sprite = _leftKeyUi;
            Debug.Log("gauche");
        }
        if (_currentKey == new Vector2(0, 1))
        {
            _image.sprite = _upKeyUi;
            Debug.Log("haut");
        }
        if (_currentKey == new Vector2(0, -1))
        {
            _image.sprite = _downKeyUi;
            Debug.Log("bas");
        }
        _canva.GetComponent<Animator>().SetTrigger("Next");
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



    private bool CanMove(Vector2 displacement)
    {
        if (playerCollider == null)
        {
            Debug.LogError("PlayerCollider manquant !");
            return false;
        }

        if (displacement == Vector2.zero)
            return false;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(movementObstacleMask);
        filter.useTriggers = false;

        Vector2 direction = displacement.normalized;
        float distance = 0.75f;


        int hitCount = playerCollider.Cast(
            direction,
            filter,
            movementHits,
            distance
        );

        Debug.DrawRay(playerCollider.bounds.center, direction * distance, Color.red, 0.75f);


        for (int i = 0; i < hitCount; i++)
        {
            if (movementHits[i].collider != null)
                return false;
        }

        return true;
    }


    void OnDrawGizmos()
    {
        //Vector2 startPosition = new Vector2(transform.position.x, transform.position.y) + Vector2.one * LookingDirection;
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(startPosition, 100f);
    }
}
