using UnityEngine;

public class SaleDuNainstabilité : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.GetComponent<Player>().canMove = false;
        }
    }
}
