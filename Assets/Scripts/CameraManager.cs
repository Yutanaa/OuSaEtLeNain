
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private Transform cameraTransform;

    [Header("Player Controls")]
    [Tooltip("Scripts du joueur à désactiver pendant la transition.")]
    [SerializeField] private Behaviour[] playerControls;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.4f;

    private Tile currentRoom;
    private bool isTransitioning;

    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Initialize(Tile startingRoom)
    {
        currentRoom = startingRoom;

        if (currentRoom == null)
            Debug.LogError("RoomTransitionManager : salle de départ manquante.");
    }

    public void TryTransition(DoorMask exitDirection)
    {
        if (isTransitioning || currentRoom == null)
        {
            Debug.Log("Transition impossible : soit une transition est déjà en cours, soit la salle actuelle est manquante.");
            return;
        }

        Tile nextRoom = currentRoom.GetNeighbour(exitDirection);

        if (nextRoom == null)
        {
            Debug.Log("Transition impossible : aucune salle voisine dans la direction " + exitDirection);
            return;
        }

        DoorMask entryDirection = Opposite(exitDirection);

        Transform entryPoint = nextRoom.GetEntryPoint(entryDirection);

        if (entryPoint == null)
        {
            Debug.LogError("Point d'arrivée manquant dans la salle suivante.");
            return;
        }

        StartCoroutine(
            TransitionToRoom(nextRoom, entryDirection, entryPoint)
        );
    }

    private IEnumerator TransitionToRoom(
        Tile nextRoom,
        DoorMask entryDirection,
        Transform entryPoint)
    {
        isTransitioning = true;

        bool[] previousStates = new bool[playerControls.Length];

        for (int i = 0; i < playerControls.Length; i++)
        {
            if (playerControls[i] == null)
                continue;

            previousStates[i] = playerControls[i].enabled;
            playerControls[i].enabled = false;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.position = entryPoint.position;
        }
        else
        {
            player.position = entryPoint.position;
        }


        nextRoom.SetEnteredFrom(entryDirection);

        Vector3 cameraStart = cameraTransform.position;

        Vector3 cameraTarget = nextRoom.CameraCenter != null
            ? nextRoom.CameraCenter.position
            : nextRoom.transform.position;

        cameraTarget.z = cameraStart.z;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / transitionDuration);

            t = t * t * (3f - 2f * t);

            cameraTransform.position = Vector3.Lerp(
                cameraStart,
                cameraTarget,
                t
            );

            yield return null;
        }

        cameraTransform.position = cameraTarget;

        currentRoom = nextRoom;

        for (int i = 0; i < playerControls.Length; i++)
        {
            if (playerControls[i] != null)
                playerControls[i].enabled = previousStates[i];
        }

        isTransitioning = false;
    }

    private static DoorMask Opposite(DoorMask direction)
    {
        switch (direction)
        {
            case DoorMask.Up: return DoorMask.Down;
            case DoorMask.Right: return DoorMask.Left;
            case DoorMask.Down: return DoorMask.Up;
            case DoorMask.Left: return DoorMask.Right;
            default: return DoorMask.None;
        }
    }
}
