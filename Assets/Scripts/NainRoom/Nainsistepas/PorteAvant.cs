using System.Collections;
using UnityEngine;

public class PorteAvant : MonoBehaviour
{
    [SerializeField] private Transform _poitToTp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Player>().StopCoroutine("SlidingRoutine");
            StartCoroutine(TpJoueur(collision.gameObject));
        }
    }

    IEnumerator TpJoueur(GameObject joueur)
    {
        yield return new WaitForSeconds(0.2f);
        joueur.gameObject.SetActive(false);
        joueur.transform.position = _poitToTp.position;
        yield return new WaitForSeconds(0.2f);
        joueur.SetActive(true);
        joueur.GetComponent<Player>().canMove = true;
    }
}
