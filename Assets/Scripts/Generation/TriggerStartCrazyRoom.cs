using UnityEngine;

public class TriggerStartCrazyRoom : MonoBehaviour
{
    public TriggerExitCrazyRoom triggerExitCrazyRoom;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CrazyRoomManager crazyRoomManager = GetComponentInParent<CrazyRoomManager>();

        if (collision.CompareTag("Player") && crazyRoomManager != null)
        {
            crazyRoomManager.StartCrazyRoom();
            triggerExitCrazyRoom.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
