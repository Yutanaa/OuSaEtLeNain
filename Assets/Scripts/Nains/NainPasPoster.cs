using System.Collections;
using UnityEngine;

public class NainPasPoster : Interactible , INainPReset
{
    private bool _canMove = true;
    private Vector2 _goHere;
    private Vector2 _spawnPos;
    private Vector2 _center;
    private float _timer;
    private void Start()
    {
        ResetPos();
        StartCoroutine(GoDiego());
    }

    public void SetCenter(Vector2 center)
    {
        _center = center;
    }
    private void FixedUpdate()
    {
        if (_timer < 10)
        {
            _timer += Time.deltaTime;

            if (_canMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, _goHere , 3f * Time.deltaTime);
                if (Vector2.Distance(transform.position, _goHere) < 0.1f)
                {
                    StartCoroutine(GoDiego());
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
        Vector2 Here = new Vector2(RandomPos(-4f,4f) + _center.x, RandomPos(-4f, 4f) + _center.y);
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
