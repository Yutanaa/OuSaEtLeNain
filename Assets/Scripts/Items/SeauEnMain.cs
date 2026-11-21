using UnityEngine;

public class SeauEnMain : MonoBehaviour
{
    [SerializeField] public NainVisible _nainScript;
    [SerializeField] private LayerMask _nainLayserMask;

    private Vector2 startPosition;
    private Vector2 lookingDirection;

    //public override void Interaction()
    //{
    //    if (IsTheGuyHere() == true)
    //    {
    //        _nainScript.Interaction();
    //    }
    //    else if (IsTheGuyHere() == false)
    //    {
    //        _nainScript.NotThisTime();
    //    }
    //    Destroy(this);
    //}

    //public bool IsTheGuyHere()
    //{
    //    if (Physics2D.Raycast(transform.position, transform.up, 10, _nainLayserMask))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }

    //}


    public void ShootDirt(Vector2 LookingDirection, Vector2 StartPosition)
    {
        startPosition = StartPosition;
        lookingDirection = LookingDirection;
        //RaycastHit2D hit = Physics2D.BoxCast(StartPosition, Vector2.one * 0.5f, 0f, LookingDirection, 2f, _nainLayserMask);
        RaycastHit2D hit = Physics2D.Raycast(StartPosition, LookingDirection, 2f, _nainLayserMask);
        Debug.DrawRay(StartPosition, transform.TransformDirection(LookingDirection) * 2f, Color.brown);


        if (hit)
        {
            if (hit.collider.gameObject.TryGetComponent<NainVisible>(out NainVisible nain))
            {
                Debug.Log("Nain touché");

                if (nain)
                {
                    Debug.Log("Nain salit");
                    nain.SalirLeNain();

                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Debug.Log("Nain raté");
            Debug.DrawRay(StartPosition, transform.TransformDirection(LookingDirection) * 1f, Color.blue);


            _nainScript.NotThisTime();
            // Debug.Log("Did not Hit");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(startPosition, lookingDirection * 2f);
    }
}
