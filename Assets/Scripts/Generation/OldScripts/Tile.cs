
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Room Information")]
    public Vector2Int GridPosition { get; private set; }
    public DoorMask Openings { get; private set; }
    public int RotationSteps { get; private set; }
    public DoorMask EnteredFrom { get; private set; }
    private Dictionary<DoorMask, Tile> neighbours =
        new Dictionary<DoorMask, Tile>();

    [Header("Transition Points")]
    [SerializeField] private Transform northEntryPoint;
    [SerializeField] private Transform southEntryPoint;
    [SerializeField] private Transform eastEntryPoint;
    [SerializeField] private Transform westEntryPoint;

    [SerializeField] private Transform cameraCenter;
    public Transform CameraCenter => cameraCenter;

    public void Initialize(
        Vector2Int gridPosition,
        DoorMask openings,
        int rotationSteps)
    {
        GridPosition = gridPosition;
        Openings = openings;
        RotationSteps = rotationSteps;
    }

    public void SetNeighbour(
        DoorMask direction,
        Tile neighbour)
    {
        neighbours[direction] = neighbour;
    }

    public Tile GetNeighbour(DoorMask direction)
    {
        neighbours.TryGetValue(direction, out Tile neighbour);
        return neighbour;
    }
    public void SetEnteredFrom(DoorMask direction)
    {
        EnteredFrom = direction;
    }

    public DoorMask LocalToWorldDirection(DoorMask localDirection)
    {
        return RotateMask(localDirection, RotationSteps);
    }

    public Transform GetEntryPoint(DoorMask worldDirection)
    {
        int inverseRotation = (4 - RotationSteps) % 4;

        DoorMask localDirection = RotateMask(
            worldDirection,
            inverseRotation
        );

        switch (localDirection)
        {
            case DoorMask.Up: return northEntryPoint;
            case DoorMask.Down: return southEntryPoint;
            case DoorMask.Right: return eastEntryPoint;
            case DoorMask.Left: return westEntryPoint;

            default: return null;
        }
    }


    private static DoorMask RotateMask(DoorMask mask, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            DoorMask rotated = DoorMask.None;

            if ((mask & DoorMask.Up) != 0)
                rotated |= DoorMask.Left;

            if ((mask & DoorMask.Right) != 0)
                rotated |= DoorMask.Up;

            if ((mask & DoorMask.Down) != 0)
                rotated |= DoorMask.Right;

            if ((mask & DoorMask.Left) != 0)
                rotated |= DoorMask.Down;

            mask = rotated;
        }

        return mask;
    }
}
