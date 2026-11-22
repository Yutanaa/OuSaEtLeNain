using System.Collections;
using UnityEngine;

public class Rope : Interactible
{
    private Player player;
    public Transform _target;
    public Transform _spawn;
    public Transform _ropeStart;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.transform.GetComponent<Player>().onRope)
            {
                StartCoroutine(Falling(collision.gameObject));
            }
        }
    }

    IEnumerator Falling(GameObject player)
    {
        player.transform.GetComponent<Player>().animator.SetTrigger("Falling");
        yield return new WaitForSeconds(1);
        player.transform.GetComponent<Player>().Teleport(_spawn.position);
    }
    public override void Interaction()
    {
        player._canva.SetActive(true);
        player._spawn = _spawn;
        player._targetToReach = _target.position;
        player.transform.position = _ropeStart.position;
        player.canMove = false;
        player.onRope = true;
        player.KeyLoader();
    }
}
