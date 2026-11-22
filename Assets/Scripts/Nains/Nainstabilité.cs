using UnityEngine;

public class Nainstabilité : Interactible
{
    public GameObject _pont;
    public GameObject _corde;
    public override void Interaction()
    {
        _pont.SetActive(true);
        _corde.SetActive(false);
        Destroy(gameObject);
    }
}
