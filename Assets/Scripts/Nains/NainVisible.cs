using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NainVisible : NainBase
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
            GameManager.Instance.AddNain(1);
            Destroy(this.gameObject);

        }
    }

    public void NotThisTime()
    {
        SoundManager.Instance.PlaySFX("NainvisibleLaugh");
        //faire spawn les particules de poussière
        if (particle != null)
        {
            Instantiate(particle, transform.position, Quaternion.identity);


        }

    }

    public void SalirLeNain()
    {

        SoundManager.Instance.PlaySFX("NainvisibleGetDirty");
        IsInteractible = true;
        _renderer.enabled = true;
    }
}
