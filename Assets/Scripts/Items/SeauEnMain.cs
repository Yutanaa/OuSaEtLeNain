using UnityEngine;

public class SeauEnMain : Interactible
{
    [SerializeField] public NainVisible _nainScript;
    [SerializeField] private LayerMask _nainLayserMask;

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
        Destroy(this);
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


    public void ShootFirt(Vector2 LookingDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, LookingDirection, 3f, _nainLayserMask);

        if (hit)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(LookingDirection) * hit.distance, Color.orange);

                if (hit.collider.GetComponent<NainVisible>())
                {
                    hit.collider.GetComponent<NainVisible>().SalirLeNain();
                }
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(LookingDirection) * 10000000, Color.orangeRed);
                // Debug.Log("Did not Hit");
            }
    }
}
