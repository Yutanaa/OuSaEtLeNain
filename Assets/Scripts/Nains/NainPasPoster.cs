using System.Collections;
using UnityEngine;

public class NainPasPoster : Interactible
{
    public SalleDesNainPoster _SalleDesNainPoster;
    public bool _canMove = false;
    public Vector2 _goHere;
    public Vector2 _spawnPos;
    public Transform _center;
    public float _timer;
    [SerializeField] private Animator _animator;

    private Coroutine goCoroutine;

    private void OnEnable()
    {
        ResetPos();
    }
    public void StartMoving()
    {
        Debug.Log("staroui");
        _canMove = true;
        GoDiego();
    }

    public override void Interaction()
    {
        _SalleDesNainPoster.RestartEvent();
        //_SalleDesNainPoster.ActiveFence();
    }

    public void SetCenter(Transform center)
    {
        if (center == null)
            Debug.Log("ON A PAS DE CENTRE NOUSUS");
        _center = center;
    }
    private void FixedUpdate()
    {

        if (!_canMove) return;

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
            _animator.SetBool("Idle", true);
        }
    }

    private void GoDiego()
    {
        _goHere = WhereToGo();
    }

    private Vector2 WhereToGo()
    {
        if (!_center)
        {
            Debug.Log("No center");
            return Vector2.zero;
        }
        Vector2 Here = new Vector2(RandomPos(_center.position.x - 2f, _center.position.x + 2f), RandomPos(_center.position.y - 2f, _center.position.y + 2f));
        return Here;
    }

    private float RandomPos(float x, float y)
    {
        float f = Random.Range(x, y);
        //int i = (int)f;
        return f;
    }

    private void ResetPos()
    {
        _spawnPos = transform.position;
    }
}
