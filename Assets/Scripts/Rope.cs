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
    public override void Interaction()
    {
        player._spawn = _spawn;
        player._targetToReach = _target.position;
        player.transform.position = _ropeStart.position;
        player.canMove = false;
        player.onRope = true;
        player.KeyLoader();
    }
}
