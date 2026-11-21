using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NainPoster : Interactible
{
    private int _witchSecond;
    [SerializeField] private List<GameObject> _ventsList = new List<GameObject>();
    [SerializeField] public List<GameObject> _originalVentsList = new List<GameObject>();

    public Transform _center;
    private SpriteRenderer _spriteRenderer;
    private float _timer;
    private bool _canMove = false;
    private bool _goToVent = false;
    private Vector2 _goHere;
    private Vector2 _spawnPos;

    public void SetCenter(Transform center)
    {
        _center = center;
    }

    private void OnEnable()
    {

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _witchSecond  = (int)Random.Range(1, 6);
        Debug.Log("j'irais dans la vent à " +  _witchSecond);
        ResetPos();
    }

    public void StartMoving()
    {
        Debug.Log("staroui");
        ResetList();
        _canMove = true;
        GoDiego();
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;

        if(_timer < 10)
        {
            _timer += Time.deltaTime;
            if (_canMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, _goHere, 3f * Time.deltaTime);
            }
            if (Vector2.Distance(transform.position, _goHere) < 0.1f)
            {
               GoDiego();
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _spawnPos, 3f * Time.deltaTime);
        }


        if ((int)_timer == (int)_witchSecond)
        {
            _goToVent = true;
            Debug.Log("Je suis sus?");
        } 
    }

    private void GoDiego()
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
            }
        }
    }
    private Vector2 WhereToGo()
    {
        Vector2 Here = new Vector2(RandomPos(_center.position.x - 2f, _center.position.x + 2f), RandomPos(_center.position.y - 2f, _center.position.y + 2f));
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
        Debug.Log("let me clap that cheek");
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_goToVent)
        {
            Debug.Log("Il rentre dans la vent");
            if (_ventsList.Contains(collision.gameObject))
            {

                Debug.Log("La vent est possible, je rentre");
                StartCoroutine(InVent());
                Debug.Log("ok1");
                StartCoroutine(Teleportation(_ventsList[Random.Range(0, _ventsList.Count)].transform.position));
                Debug.Log("ok2");
            }
            Debug.Log("non");
        }  
    }

    IEnumerator InVent()
    {
        Debug.Log("I'm invisible");
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.5f);
        _spriteRenderer.enabled = true;
    }

    IEnumerator Teleportation(Vector2 where)
    {   
        if (_goToVent)
        {
            _goToVent = false;
            transform.position = where;
            ResetList();
            yield return new WaitForSeconds(0.5f);
            GoDiego();
        }
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