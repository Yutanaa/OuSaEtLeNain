using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalleDesNainPoster : MonoBehaviour
{
    [SerializeField] private List<GameObject> _place = new List<GameObject>();
    [SerializeField] private List<GameObject> _allPlace = new List<GameObject>();
    [SerializeField] private List<GameObject> _ventsList = new List<GameObject>();
    [SerializeField] private GameObject[] _nains;
    [SerializeField] private List<GameObject> _nainsActif = new List<GameObject>();
    [SerializeField] private GameObject _center;
    private Collider2D _collider;
    [SerializeField] Transform _entrer;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _portillon;
    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        
    }

    private void SpawnNains()
    {
        if(_nainsActif.Count > 0)
        {

            return;
        }
        StartCoroutine(Delay());
        Debug.Log("Spawning nains");
        _player.transform.position = _entrer.position;
        foreach (GameObject p in _allPlace)
        {
            _place.Add(p);
        }

        foreach (GameObject N in _nains)
        {
            GameObject here = _place[Random.Range(0, _place.Count)];
            GameObject _nain = Instantiate(N, here.transform.position, Quaternion.identity);
            if (_nain.gameObject.GetComponent<NainPoster>())
            {
                _nain.gameObject.GetComponent<NainPoster>().SetCenter(_center.transform);
                _nain.gameObject.GetComponent<NainPoster>()._originalVentsList = _ventsList;
                _nain.gameObject.GetComponent<NainPoster>()._salleDesNains = this;

            }
            if (_nain.gameObject.GetComponent<NainPasPoster>())
            {
                _nain.gameObject.GetComponent<NainPasPoster>()._SalleDesNainPoster = this;
                _nain.gameObject.GetComponent<NainPasPoster>().SetCenter(_center.transform);
            }
            _nainsActif.Add(_nain);
            _place.Remove(here);
        }
    }
    IEnumerator Delay()
    {
        ActiveFence();
        yield return new WaitForSeconds(10);
        DesactiveFence();
    }

    public void ActiveFence()
    {
        _portillon.SetActive(true);
    }

    public void DesactiveFence()
    {
        _portillon.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _player = collision.gameObject;
        Debug.Log("fjihisjocijosef");
        StartEvent();
        _collider.enabled = false;
    }
    private void StartEvent()
    {
        SpawnNains();
        StartCoroutine(TimerStart());
    }

    IEnumerator TimerStart()
    {
        yield return new WaitForSeconds(0.5f);
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
        Debug.Log("oui je restart" + _nainsActif.Count);
        foreach (GameObject N in _nainsActif)
        {
            Destroy(N);
        }
        _nainsActif.Clear();
        Debug.Log("oui j'ai restart" + _nainsActif.Count);
        StartEvent();
    }

    public void DestroyTheNains()
    {
        foreach (GameObject N in _nainsActif)
        {
            Destroy(N);
        }
    }
}
