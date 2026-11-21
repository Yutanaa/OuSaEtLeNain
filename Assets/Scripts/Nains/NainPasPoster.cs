using System.Collections;
using UnityEngine;

public class NainPasPoster : Interactible, INainPReset
{
    public bool _canMove = true;
    public Vector2 _goHere;
    public Vector2 _spawnPos;
    public Transform _center;
    public float _timer;

    private Coroutine goCoroutine;
    private void Start()
    {
        ResetPos();

        if (_center)
            _goHere = WhereToGo();
    }

    public void SetCenter(Transform center)
    {
        if (center == null)
            Debug.Log("ON A PAS DE CENTRE NOUSUS");
        _center = center;
    }
    private void FixedUpdate()
    {
        if (_timer < 10)
        {
            _timer += Time.deltaTime;

            if (_canMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, _goHere, 3f * Time.deltaTime);
                if (Vector2.Distance(transform.position, _goHere) < 0.1f)
                {
                    _goHere = WhereToGo();
                }
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _spawnPos, 3f * Time.deltaTime);
        }
    }

    IEnumerator GoDiego()
    {
        _goHere = WhereToGo();
        yield return null;
    }

    private Vector2 WhereToGo()
    {
        if (!_center)
        {
            Debug.Log("No center");
            return Vector2.zero;
        }
        Vector2 Here = new Vector2(RandomPos(_center.position.x - 4f, _center.position.x + 4f), RandomPos(_center.position.y - 4f, _center.position.y + 4f));
        return Here;
    }

    private float RandomPos(float x, float y)
    {
        float f = Random.Range(x, y);
        //int i = (int)f;
        return f;
    }

    public override void Interaction()
    {
        //faire exploser le joueur pour qu'il retourne au début de la salle
    }

    public virtual void ResetAllStats()
    {
        _timer = 0;
        ResetPos();
    }
    private void ResetPos()
    {
        _spawnPos = transform.position;
    }
}
