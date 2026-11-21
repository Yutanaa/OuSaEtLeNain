using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NainVisible : Interactible
{
    private SpriteRenderer _renderer;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject allSpots;

    private List<Transform> _spots = new List<Transform>();



    private void OnEnable()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        foreach (Transform child in allSpots.transform)
        {
            _spots.Add(child);
        }

        transform.position = _spots[Random.Range(0, _spots.Count)].position;
    }

    public override void Interaction()
    {
        if (IsInteractible)
        {
            //TODO : Rajouter le nain
            Destroy(this.gameObject);

        }
    }

    public void NotThisTime()
    {
        //faire spawn les particules de poussière
        if (particle != null)
        {
            Instantiate(particle, transform.position, Quaternion.identity);


        }

    }

    public void SalirLeNain()
    {
        IsInteractible = true;
        _renderer.enabled = true;
    }
}
