using System.Collections.Generic;
using UnityEngine;

public class SalleDesNainPoster : MonoBehaviour
{
    [SerializeField] private List<GameObject> _place = new List<GameObject>();
    [SerializeField] private List<GameObject> _ventsList = new List<GameObject>();
    [SerializeField] private GameObject[] _nains;
    [SerializeField] private List<GameObject> _nainsActif = new List<GameObject>();
    [SerializeField] private GameObject _center;
    private Collider2D _collider;
    private void Start()
    {
        _collider = GetComponent<Collider2D>();
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
            GameObject _nain = Instantiate(N, here.transform.position, Quaternion.identity);
            if (N.gameObject.GetComponent<NainPoster>())
            {
                N.gameObject.GetComponent<NainPoster>().SetCenter(_center.transform);
                N.gameObject.GetComponent<NainPoster>()._originalVentsList = _ventsList;

            }
            if (N.gameObject.GetComponent<NainPasPoster>())
            {

                N.gameObject.GetComponent<NainPasPoster>().SetCenter(_center.transform);
            }
            _nainsActif.Add(_nain);
            _place.Remove(here);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        Debug.Log("fjihisjocijosef");
        StartEvent();
        _collider.enabled = false;
    }
    private void StartEvent()
    {
        foreach (GameObject N in _nainsActif)
        {
            if (N.gameObject.GetComponent<NainPoster>())
            {
                N.gameObject.GetComponent<NainPoster>().StartMoving();
            }
            if (N.gameObject.GetComponent<NainPasPoster>())
            {
                N.gameObject.GetComponent<NainPasPoster>().StartMoving();
            }
        }
    }
    public void RestartEvent()
    {
        foreach (GameObject N in _nainsActif)
        {
            Destroy(N);
        }
    }
}
