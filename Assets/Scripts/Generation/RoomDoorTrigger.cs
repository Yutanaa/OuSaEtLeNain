
using UnityEngine;

public class RoomDoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private DoorMask localDirection;

    private Tile room;


    private void Awake()
    {
        room = GetComponentInParent<Tile>();

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (room == null || CameraManager.Instance == null)
            return;


        DoorMask worldDirection = room.LocalToWorldDirection(localDirection);

        CameraManager.Instance.TryTransition(worldDirection);
    }
}
