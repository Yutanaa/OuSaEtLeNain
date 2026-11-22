using UnityEngine;

public class Nainstabilité : NainBase
{
    public GameObject _pont;
    public GameObject _corde;
    public override void Interaction()
    {
        _pont.SetActive(true);
        _corde.SetActive(false);
        GameManager.Instance.AddNain(1);
        Destroy(gameObject);
    }
}
