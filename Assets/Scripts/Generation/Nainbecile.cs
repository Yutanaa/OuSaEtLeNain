
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nainbecile : CrazyNain
{
    [Header("References")]
    [SerializeField] CrazyRoomManager _crazyRoomManager;
    [SerializeField] private Player player;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject sprite;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 0.25f;

    private Queue<Vector2> positionsToVisit = new Queue<Vector2>();

    private bool isMoving;

    private void OnEnable()
    {
        if (player == null)
        {

            player = FindFirstObjectByType<Player>();
        }

        if (_isDown)
        {
            PlayDownAnimation();
            return;
        }
        player.OnStepCompleted += OnPlayerStepCompleted;
        isMoving = false;
    }




    private void OnDisable()
    {

        if (player != null)
            player.OnStepCompleted -= OnPlayerStepCompleted;

    }

    public override void Hit()
    {
        if (_isDown) return;
        player.OnStepCompleted -= OnPlayerStepCompleted;
        base.Hit();

    }

    public override void Interaction()
    {
        base.Interaction();
        _crazyRoomManager.AddCrazyNain(this);

    }

    private void OnPlayerStepCompleted(Vector2 previousPosition)
    {
        if (_isDown) return;
        positionsToVisit.Enqueue(previousPosition);

        if (!isMoving)
            StartCoroutine(FollowPlayerPath());
    }
    private IEnumerator FollowPlayerPath()
    {
        if (_isDown) yield return null;
        isMoving = true;

        while (positionsToVisit.Count > 0)
        {
            Vector2 targetPosition = positionsToVisit.Dequeue();

            yield return StartCoroutine(
                MoveToPosition(targetPosition)
            );
        }

        isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector2 targetPosition)
    {
        if (_isDown) yield return null;
        Vector2 startPosition = transform.position;

        if (Vector2.Distance(startPosition, targetPosition) < 0.01f)
            yield break;

        if (sprite != null && targetPosition.x != startPosition.x)
        {
            sprite.transform.localScale = new Vector3(
                targetPosition.x > startPosition.x ? -1f : 1f,
                1f,
                1f
            );
        }

        if (animator != null)
            animator.SetTrigger("Walking");

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = Mathf.Clamp01(elapsed / moveDuration);

            transform.position = Vector2.Lerp(
                startPosition,
                targetPosition,
                t
            );

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPosition;
        SoundManager.Instance.PlaySFX("NainBecilePas");
    }

    public override void PlayDownAnimation()
    {

        if (animator != null)
            animator.Play("Down");
    }
}
