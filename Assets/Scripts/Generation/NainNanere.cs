using System.Collections;
using Unity.Hierarchy;
using UnityEngine;

public class NainNanere : CrazyNain
{
    [Header("Refeerences")]
    [SerializeField] CrazyRoomManager _crazyRoomManager;
    [SerializeField] private LayerMask playerLayerMask;
    [Header("Settings")]
    [SerializeField] private float _detectRangePlayer = 4f;

    private Animator animator;
    private Coroutine teleportCoroutine;
    private Player player;

    private bool isTeleporting = false;

    private Transform actualTPPoint;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(isTeleporting || _isDown)
            return;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _detectRangePlayer, Vector2.zero, 0f, playerLayerMask);

        if (hit)
        {
            Transform tp = _crazyRoomManager.GetRandomTPPoint(actualTPPoint);
            actualTPPoint = tp;
            player = hit.collider.GetComponent<Player>();

            if (tp != null)
            {

                animator.Play("Teleport");
                transform.position = tp.position;
                if (teleportCoroutine != null)
                {
                    StopCoroutine(teleportCoroutine);
                }
                teleportCoroutine = StartCoroutine(TeleportToPoint(tp));
            }
        }
    }

    public override void Interaction()
    {
        base.Interaction();
        _crazyRoomManager.AddCrazyNain(this);

    }

    IEnumerator TeleportToPoint(Transform tp)
    {

        SoundManager.Instance.PlaySFX("NainnanèreTP");
        isTeleporting = true;
        yield return new WaitForSeconds(0.1f);
        isTeleporting = false;

    }


    public override void PlayDownAnimation()
    {
        animator.Play("Down");
    }





}
