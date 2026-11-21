using UnityEngine;

public class Rope : Interactible
{
    private Player player;
    public Transform _target;
    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }
    public override void Interaction()
    {
        player._targetToReach = _target.position;
        player.canMove = false;
        player.onRope = true;
        player.KeyLoader();
    }
}
