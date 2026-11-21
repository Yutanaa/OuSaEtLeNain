using System.Collections;
using UnityEngine;

public class SaleDuNainstabilité : MonoBehaviour
{
    public Transform _spawn;
    public GameObject _pont;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.transform.GetComponent<Player>().onRope)
            {
                StartCoroutine(Falling(collision.gameObject));
            }
            
        }
    }
    IEnumerator Falling(GameObject player)
    {
        player.transform.GetComponent<Player>().animator.SetTrigger("Falling");
        yield return new WaitForSeconds(1);
        player.transform.GetComponent<Player>().Teleport(_spawn.position);
    }

    public void SpawnPont()
    {
        _pont.SetActive(true);
    }
}
