using System.Collections;
using UnityEngine;

public class PorteArriere : MonoBehaviour
{
    [SerializeField] private Transform _poitToTp;
    [SerializeField] private GameObject[] _colliderToDesable;
    [SerializeField] private GameObject _exit;

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
        SoundManager.Instance.PlaySFX("NainsistepasOpenCoridoor");
        joueur.GetComponent<Player>().Teleport(_poitToTp.position);
        yield return new WaitForSeconds(0.2f);


        foreach (GameObject c in _colliderToDesable)
        {
            Destroy(c);
        }
        _exit.SetActive(true);
    }
}
