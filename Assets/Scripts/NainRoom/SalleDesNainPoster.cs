using System.Collections.Generic;
using UnityEngine;

public class SalleDesNainPoster : MonoBehaviour
{
    [SerializeField] private List<GameObject> _place = new List<GameObject>();
    [SerializeField] private List<GameObject> _ventsList = new List<GameObject>();
    [SerializeField] private GameObject[] _nains;
    [SerializeField] private GameObject _center;
    private Collider _collider;
    private void Start()
    {
        _collider = GetComponent<Collider>();
        SpawnNains();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpawnNains();
        _collider.enabled = false;
    }

    private void SpawnNains()
    {
        foreach (GameObject N in _nains)
        {
            GameObject here = _place[Random.Range(0, _place.Count)];
            Instantiate(N, here.transform.position, Quaternion.identity);
            if (N.gameObject.GetComponent<NainPoster>())
            {
                N.gameObject.GetComponent<NainPoster>().SetCenter(_center.transform);
                N.gameObject.GetComponent<NainPoster>()._originalVentsList = _ventsList;
                N.gameObject.GetComponent<NainPoster>().StartMoving();

            }
            if (N.gameObject.GetComponent<NainPasPoster>())
            {

                N.gameObject.GetComponent<NainPasPoster>().SetCenter(_center.transform);
                _place.Remove(here);
                N.gameObject.GetComponent<NainPasPoster>().StartMoving();
            }
        }
    }
}
