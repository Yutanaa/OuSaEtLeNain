using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NainPoster : Interactible , INainPReset
{
    private int _witchSecond;
    [SerializeField] private List<GameObject> _ventsList = new List<GameObject>();
    [SerializeField] public List<GameObject> _originalVentsList = new List<GameObject>();

    public Vector2 _center;
    private SpriteRenderer _spriteRenderer;
    private float _timer;
    private bool _canMove = true;
    private bool _goToVent = false;
    private Vector2 _goHere;
    private Vector2 _spawnPos;

    public void SetCenter(Vector2 center)
    {
        _center = center;
    }

    private void OnEnable()
    {
      _witchSecond  = (int)Random.Range(1, 6);
        Debug.Log("j'irais dans la vent à " +  _witchSecond);
    }

    private void Start()
    {
        ResetPos();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ResetList();
        StartCoroutine(GoDiego());
    }

    private void FixedUpdate()
    {
        if(_timer < 10)
        {
            _timer += Time.deltaTime;
            if (_canMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, _goHere, 3f * Time.deltaTime);
            }
            if (Vector2.Distance(transform.position, _goHere) < 0.1f)
            {
                StartCoroutine(GoDiego());
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _spawnPos, 3f * Time.deltaTime);
        }


        if ((int)_timer == (int)_witchSecond)
        {
            _goToVent = true;
        } 
    }

    IEnumerator GoDiego()
    {
        if (_canMove)
        {
            if (_goToVent)
            {
                if (_ventsList != null)
                {
                    _goHere = _ventsList[Random.Range(0, _ventsList.Count)].transform.position;
                    Debug.Log("muhehehe");
                }
            }
            else
            {
                _goHere = WhereToGo();
                yield return null;
            }
        }
    }
    private Vector2 WhereToGo()
    {
        Vector2 Here = new Vector2(RandomPos(-4f, 4f), RandomPos(-4f, 4f));
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_ventsList.Contains(collision.gameObject))
        {
            _ventsList.Remove(collision.gameObject);
            Debug.Log("ok1");
            StartCoroutine(InVent());
            Debug.Log("ok2");
            StartCoroutine(Teleportation(_ventsList[Random.Range(0, _ventsList.Count)].transform.position));
            Debug.Log("ok3");
        }
        Debug.Log("non");
    }

    IEnumerator InVent()
    {
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.5f);
        _spriteRenderer.enabled = true;
    }

    IEnumerator Teleportation(Vector2 where)
    {   
        if (_goToVent)
        {
            transform.position = where;
            ResetList();
            _goToVent = false;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(GoDiego());
        }
    }

    public virtual void ResetAllStats()
    {
        _timer = 0;
        ResetPos();
    }
    private void ResetList()
    {
        _ventsList = _originalVentsList;
    }
    private void ResetPos()
    {
        _spawnPos = transform.position;
    }
}