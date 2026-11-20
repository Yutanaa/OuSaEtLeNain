
using System;
using UnityEngine;

[Flags]
public enum DoorMask
{
    None = 0,
    Up = 1,
    Right = 2,
    Down = 4,
    Left = 8
}

[CreateAssetMenu(fileName = "RoomDefinition", menuName = "Generation/Room Definition")]
public class RoomDefinition : ScriptableObject
{
    [Header("Room Settings")]
    public GameObject prefab;

    [Header("Room Openings")]
    public DoorMask openings;
}
