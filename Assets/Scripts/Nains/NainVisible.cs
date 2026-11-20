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
        IsInteractible = true;
        _renderer.enabled = true;
        print("caca");
    }

    public void NotThisTime()
    {
        //faire spawn les particules de poussière
        print("pipi");
    }
}
