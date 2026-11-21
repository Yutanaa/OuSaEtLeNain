using UnityEngine;

public class NainVisible : Interactible
{
    private SpriteRenderer _renderer;
    [SerializeField] private ParticleSystem particle;



    private void OnEnable()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
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
            particle.Play();

    }

    public void SalirLeNain()
    {
        IsInteractible = true;
        _renderer.enabled = true;
    }
}
