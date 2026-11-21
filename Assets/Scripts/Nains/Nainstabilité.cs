using UnityEngine;

public class Nainstabilité : Interactible
{

    public override void Interaction()
    {
        Destroy(gameObject);
    }
}
