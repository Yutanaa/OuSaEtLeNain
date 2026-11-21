using UnityEngine;

public class TriggerExitCrazyRoom : MonoBehaviour
{
    public TriggerStartCrazyRoom triggerStartCrazyRoom;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CrazyRoomManager crazyRoomManager = GetComponentInParent<CrazyRoomManager>();

        if (collision.CompareTag("Player") && crazyRoomManager != null)
        {
            crazyRoomManager.ResetRoom();
            triggerStartCrazyRoom.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
