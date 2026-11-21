using UnityEngine;

public class TrapNain : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Nain"))
        {
            if(collision.gameObject.GetComponent<CrazyNain>())
            {
                collision.gameObject.GetComponent<CrazyNain>().Hit();
            }
        }
    }
}
