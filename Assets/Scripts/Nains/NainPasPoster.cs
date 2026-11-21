using System.Collections;
using UnityEngine;

public class NainPasPoster : Interactible
{
    private bool _canMove = true;
    private Vector2 _goHere;
    private void Start()
    {
        StartCoroutine(GoDiego());
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, _goHere, 3f*Time.deltaTime);
    }
    IEnumerator Wait(float duration)
    {
        _canMove = false;
        StopAllCoroutines();
        yield return new WaitForSeconds(duration);
        _canMove = true;
    }

    IEnumerator GoDiego()
    {
        if (_canMove)
        {
            _goHere = WhereToGo();
            yield return new WaitForSeconds(3);
            StartCoroutine(GoDiego());
        }
    }
    private Vector2 WhereToGo()
    {
        Vector2 Here = new Vector2(RandomPos(-5f,5), RandomPos(-3f, 3f));
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
}
