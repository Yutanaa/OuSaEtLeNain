using UnityEngine;

public class SeauEnMain : Interactible
{
    [SerializeField] public NainVisible _nainScript;
    [SerializeField] private LayerMask _nainLayserMask;

    private void Start()
    {
        Interaction();
    }

    public override void Interaction()
    {
       if (IsTheGuyHere() == true)
        {
            _nainScript.Interaction();
        }
       else if (IsTheGuyHere() == false)
        {
            _nainScript.NotThisTime();
        }
    }

    public bool IsTheGuyHere()
    {
        if (Physics2D.Raycast(transform.position, transform.up, 10, _nainLayserMask))
        {
           return true;
        }
        else
        {
            return false;
        }
    }
}
