using System.Collections;
using System.Linq;
using UnityEngine;

public class NainPoster : Interactible
{
    private int _witchSecond;
    [SerializeField] private GameObject[] _vents;

    private SpriteRenderer _spriteRenderer;
    private float _timer;
    private bool _canMove = true;
    private Vector2 _goHere;

    private void OnEnable()
    {
      _witchSecond  = (int)Random.Range(1, 5);
    }

    private void Start()
    {
        StartCoroutine(GoDiego());
    }

    private void Update()
    {
        while (_timer < 5)
        {
            _timer = +Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, _goHere, 3f * Time.deltaTime);
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
            if ((int)_timer == _witchSecond)
            {
                if (_vents != null)
                {
                    _goHere = _vents[Random.Range(0,_vents.Length)].transform.position;
                }
            }
            else
            {
                _goHere = WhereToGo();
                yield return new WaitForSeconds(3);
                StartCoroutine(GoDiego());
            }
            
        }
    }
    private Vector2 WhereToGo()
    {
        Vector2 Here = new Vector2(RandomPos(-5f, 5), RandomPos(-3f, 3f));
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

    private void OnCollisionEnter(Collision collision)
    {
        if (_vents.Contains(collision.gameObject))
        {
        }
    }

    IEnumerator InVent()
    {
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(1.5f);
        _spriteRenderer.enabled = true;
    }
}