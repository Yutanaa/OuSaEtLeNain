using UnityEngine;

public class Nainsistepas : NainBase
{
    public override void Interaction()
    {
        Debug.Log("touché");
        GameManager.Instance.AddNain(1);
        //to do add nain reçu
        Destroy(gameObject);
    }
}
