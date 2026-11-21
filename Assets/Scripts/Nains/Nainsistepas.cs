using UnityEngine;

public class Nainsistepas : Interactible
{
    public override void Interaction()
    {
        Debug.Log("touché");
        //to do add nain reçu
        Destroy(gameObject);
    }
}
