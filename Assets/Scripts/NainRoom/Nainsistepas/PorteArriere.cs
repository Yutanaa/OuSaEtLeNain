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
            collision.transform.position = _poitToTp.position;

            foreach (GameObject c in _colliderToDesable)
            {
                c.SetActive(false);
            }

            _exit.SetActive(true);
        }
    }
}
