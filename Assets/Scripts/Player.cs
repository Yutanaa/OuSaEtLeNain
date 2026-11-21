using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 lastRespawnPosition;
    public Animator animator;

    public PlayerInput _playerInput;
    public float movingDistance;
    public GameObject itemTenu;
    public LayerMask layerMask;

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

    public void SetRespawnPoint(Vector2 pos)
    {
        lastRespawnPosition = pos;
    }

    void Update()
    {

    }

    void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        transform.Translate(direction);
        animator.SetTrigger("Walking");
    }

    void OnRelease(InputAction.CallbackContext context)
    {

    }

    public void Respawn()
    {

    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (itemTenu)
        {
            print("interact");
        }

        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, 100f, layerMask);
            
            // Does the ray intersect any objects excluding the player layer
            if (hit)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(-transform.right) * hit.distance, Color.red);
                Debug.Log("Did Hit");
                hit.collider?.GetComponent<IInteractible>().Interaction();
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(-transform.right) * 10000000, Color.yellow);
                Debug.Log("Did not Hit");
            }
        }
    }


    public void AddItem(GameObject gameo)
    {

    }
}
