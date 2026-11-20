using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerInput _playerInput;
    public float movingDistance;
    public Vector2 lastRespawnPosition;
    
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
    }

    void OnRelease(InputAction.CallbackContext context)
    {
        
    }

    public void Respawn()
    {
        
    }

    public void Interact(InputAction.CallbackContext context)
    {
        print("interact");
    }

    public void AddItem(GameObject gameo)
    {
        
    }
}
