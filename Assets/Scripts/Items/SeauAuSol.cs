using UnityEngine;

public class SeauAuSol : Interactible
{
    [SerializeField] private SeauEnMain _seauEnMain;
    [SerializeField] private NainVisible _nainVisible;

    public override void Interaction()
    {
        // GiveMeThatBucket();
    }


    public SeauEnMain GiveMeThatBucket(Transform parent)
    {
        SeauEnMain Seau_EM = Instantiate(_seauEnMain, parent);
        if (_nainVisible != null)
            Seau_EM._nainScript = _nainVisible;

        SoundManager.Instance.PlaySFX("GetDirt");
        return Seau_EM;
    }

    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     print("éééééé");
    //     if (collision.gameObject.GetComponent<Player>())
    //     {
    //         collision.gameObject.GetComponent<Player>().AddItem(GiveMeThatBucket());
    //     }
    // }
}
