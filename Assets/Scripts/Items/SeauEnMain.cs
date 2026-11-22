using UnityEngine;

public class SeauEnMain : MonoBehaviour
{
    [SerializeField] public NainVisible _nainScript;
    [SerializeField] private GameObject _dirtPrefab;
    [SerializeField] private LayerMask _nainLayserMask;
    [SerializeField] private LayerMask _groundLayerMask;

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
        SoundManager.Instance.PlaySFX("LaunchDirt");
        startPosition = StartPosition;
        lookingDirection = LookingDirection;
        //RaycastHit2D hit = Physics2D.BoxCast(StartPosition, Vector2.one * 0.5f, 0f, LookingDirection, 2f, _nainLayserMask);
        RaycastHit2D hit = Physics2D.Raycast(StartPosition, LookingDirection, 2f, _nainLayserMask);
        RaycastHit2D[] groundsHit = Physics2D.RaycastAll(StartPosition, LookingDirection, 2f, _groundLayerMask);
        Debug.DrawRay(StartPosition, transform.TransformDirection(LookingDirection) * 2f, Color.brown);

        if (groundsHit.Length > 0)
        {
            Debug.Log("Ground detected: " + groundsHit.Length);
            foreach (var ground in groundsHit)
            {
                Debug.Log("Ground hit: " + ground.collider.gameObject.name);
                Vector2 groudPosition = ground.collider.gameObject.transform.position;
                GameObject dirt = Instantiate(_dirtPrefab, groudPosition, Quaternion.identity);

                Destroy(dirt, 3f);

            }
        }
        else
        {
            Debug.Log("No ground detected.");
        }

        if (hit)
        {
            if (hit.collider.gameObject.TryGetComponent<NainVisible>(out NainVisible nain))
            {
                Debug.Log("Nain touché");

                if (nain)
                {
                    Debug.Log("Nain salit");
                    nain.SalirLeNain();

                }
            }
        }
        else
        {
            Debug.Log("Nain raté");
            Debug.DrawRay(StartPosition, transform.TransformDirection(LookingDirection) * 1f, Color.blue);

            if (_nainScript && _nainScript.gameObject != null)
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
