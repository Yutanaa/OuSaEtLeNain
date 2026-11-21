using UnityEngine;

public class NainVisible : Interactible
{
    private SpriteRenderer _renderer;

    private void OnEnable()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
    }
    public override void Interaction()
    {
        if (IsInteractible)
        {

        }
    }

    public void NotThisTime()
    {
        //faire spawn les particules de poussière
        print("pipi");
    }

    public void SalirLeNain()
    {
        
    }
}
