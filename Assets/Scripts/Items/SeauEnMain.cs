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


    public void ShootDirt(Vector2 LookingDirection, Vector2 StartPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(StartPosition, LookingDirection, 1f, _nainLayserMask);

        if (hit.collider?.GetComponent<NainVisible>())
        {
            Debug.DrawRay(StartPosition, transform.TransformDirection(LookingDirection) * hit.distance, Color.orange);

            if (hit.collider.GetComponent<NainVisible>())
            {
                hit.collider.GetComponent<NainVisible>().SalirLeNain();
            }
        }
        else
        {
            Debug.DrawRay(StartPosition, transform.TransformDirection(LookingDirection) * 1f, Color.blue);

            print("eeeeeeee");
            _nainScript.NotThisTime();
            Destroy(gameObject);
            // Debug.Log("Did not Hit");
        }
    }
}
