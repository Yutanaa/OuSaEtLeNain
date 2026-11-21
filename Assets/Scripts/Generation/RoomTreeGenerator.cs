
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Unity.VisualScripting;
using UnityEngine;

public class RoomTreeGenerator : MonoBehaviour
{
    [Header("Room Definitions")]
    [SerializeField] private RoomDefinition startRoom;

    [SerializeField] private RoomDefinition[] normalRooms;
    [Header("Special Rooms")]
    [SerializeField] private RoomDefinition[] specialRooms = new RoomDefinition[6];
    private List<RoomDefinition> roomsPlaced = new List<RoomDefinition>();


    private Dictionary<Vector2Int, Tile> generatedRooms =
        new Dictionary<Vector2Int, Tile>();


    private List<RoomDefinition> shuffledSpecialRooms = new List<RoomDefinition>();
    private int specialRoomIndex;

    [Header("Room Placement")]
    [SerializeField] private Transform roomsParent;
    [SerializeField] private Vector2 roomSize = new Vector2(20, 12);

    [Header("Generation Settings")]
    [SerializeField] private bool randomizeSeed = true;
    [SerializeField] private int seed = 12345;
    [SerializeField] private int minDepth = 2;
    [SerializeField] private int maxDepth = 6;


    [Range(0f, 1f)]
    [SerializeField] private float twoChildrenChance = 0.25f;

    [Range(0f, 1f)]
    [SerializeField] private float threeChildrenChance = 0.08f;


    [Header("Branch Settings")]
    [SerializeField, Min(1)] private int minRoomsAfterSplit = 2;

    private System.Random rng;

    private static readonly DoorMask[] Directions =
    {
        DoorMask.Up,
        DoorMask.Right,
        DoorMask.Down,
        DoorMask.Left
    };

    private class RoomNode
    {
        public Vector2Int position;
        public int depth;
        public DoorMask parentDoor;

        public int roomsSinceSplit;

        public RoomNode(
            Vector2Int position,
            int depth,
            DoorMask parentDoor,
            int roomsSinceSplit = 0)
        {
            this.position = position;
            this.depth = depth;
            this.parentDoor = parentDoor;
            this.roomsSinceSplit = roomsSinceSplit;
        }
    }

    private struct RoomChoice
    {
        public RoomDefinition definition;
        public DoorMask openings;
        public int rotationSteps;
    }

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        if (!ValidateDefinitions() || maxDepth < 2)
        {
            Debug.LogError("RoomTreeGenerator : Configuration invalide.");
            return;
        }

        if (roomsParent == null)
            roomsParent = transform;

        if (randomizeSeed)
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        rng = new System.Random(seed);

        ClearRooms();
        generatedRooms.Clear();
        PrepareSpecialRoomPool();
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        List<RoomNode> terminalRooms = new List<RoomNode>();

        int currentLeafCount = 1;

        int targetTerminalCount = specialRooms.Length;

        int maxChildrenPerRoom = GetMaxChildrenPerRoom();

        Vector2Int startPosition = Vector2Int.zero;

        occupied.Add(startPosition);


        RoomChoice startChoice = new RoomChoice
        {
            definition = startRoom,
            openings = startRoom.openings,
            rotationSteps = 0
        };

        PlaceRoom(startChoice, startPosition);


        Vector2Int firstPosition = Vector2Int.down;

        occupied.Add(firstPosition);

        Queue<RoomNode> pendingRooms = new Queue<RoomNode>();

        pendingRooms.Enqueue(
            new RoomNode(
                firstPosition,
                1,
                DoorMask.Up,
                minRoomsAfterSplit
            )
        );






        while (pendingRooms.Count > 0)
        {
            RoomNode node = pendingRooms.Dequeue();


            bool mustExtend = node.roomsSinceSplit < minRoomsAfterSplit;


            bool targetReached = currentLeafCount >= targetTerminalCount;


            if (node.depth >= maxDepth && !mustExtend)
            {
                terminalRooms.Add(node);
                continue;
            }

            int maxAllowedChildren;
            int minRequiredChildren;


            if (node.depth == 1)
            {
                maxAllowedChildren = 2;
                minRequiredChildren = 2;
            }


            else if (mustExtend || targetReached)
            {
                maxAllowedChildren = 1;
                minRequiredChildren = 1;
            }
            else
            {
                maxAllowedChildren = Mathf.Min(
                    maxChildrenPerRoom,
                    targetTerminalCount - currentLeafCount + 1
                );

                minRequiredChildren = GetMinimumChildrenNeeded(
                    node,
                    pendingRooms,
                    terminalRooms,
                    targetTerminalCount,
                    maxChildrenPerRoom,
                    maxAllowedChildren
                );
            }

            if (minRequiredChildren > maxAllowedChildren)
            {
                terminalRooms.Add(node);
                continue;
            }

            int desiredChildren;

            if (node.depth == 1)
            {
                desiredChildren = 2;
            }
            else if (mustExtend || targetReached || node.depth < minDepth)
            {


                desiredChildren = 1;
            }
            else
            {
                desiredChildren = ChooseChildCount();
            }

            desiredChildren = Mathf.Clamp(
                desiredChildren,
                minRequiredChildren,
                maxAllowedChildren
            );

            RoomChoice choice = new RoomChoice();
            bool found = false;


            if (node.depth == 1)
            {
                found = TryChooseNormalRoom(
                    node,
                    2,
                    occupied,
                    out choice
                );
            }
            else
            {
                for (int count = desiredChildren; count >= minRequiredChildren; count--)
                {
                    if (TryChooseNormalRoom(node, count, occupied, out choice))
                    {
                        found = true;
                        break;
                    }
                }
            }


            if (!found)
            {
                if (mustExtend)
                {
                    Debug.LogWarning(
                        $"Impossible de prolonger la branche en {node.position}. " +
                        "Vérifie tes salles normales et les collisions."
                    );
                }

                terminalRooms.Add(node);
                continue;
            }

            PlaceRoom(choice, node.position);

            DoorMask childrenMask = choice.openings & ~node.parentDoor;

            int actualChildren = CountDoors(childrenMask);


            currentLeafCount += actualChildren - 1;


            int childRoomsSinceSplit = actualChildren > 1
                ? 0
                : node.roomsSinceSplit + 1;

            foreach (DoorMask direction in Directions)
            {
                if ((childrenMask & direction) == 0)
                    continue;

                Vector2Int childPosition =
                    node.position + DirectionToVector(direction);

                occupied.Add(childPosition);

                RoomNode child = new RoomNode(
                    childPosition,
                    node.depth + 1,
                    Opposite(direction),
                    childRoomsSinceSplit
                );

                pendingRooms.Enqueue(child);
            }
        }


        if (terminalRooms.Count != specialRooms.Length)
        {
            Debug.LogError(
                $"Impossible de générer le nombre souhaité de salles spéciales. " +
                $"Résultat : {terminalRooms.Count}/{specialRooms.Length}. " +
                "Augmente maxDepth ou ajoute des salles avec davantage d'ouvertures."
            );

            return;
        }


        foreach (RoomNode node in terminalRooms)
        {
            RoomDefinition selectedRoom = GetNextSpecialRoom();
            roomsPlaced.Add(selectedRoom);
            if (!PlaceTerminalRoom(selectedRoom, node))
            {
                //ClearRooms();
                return;
            }
        }

        Debug.Log(
            $"Génération terminée ! " +
            $"Seed : {seed} | " +
            $"Salles spéciales : {terminalRooms.Count}"
        );
        BuildRoomConnections();

        CameraManager.Instance.Initialize(generatedRooms[startPosition]);
    }


    private int GetMaxChildrenPerRoom()
    {
        int maxChildren = 1;

        foreach (RoomDefinition definition in normalRooms)
        {
            if (definition == null)
                continue;

            int children = CountDoors(definition.openings) - 1;

            maxChildren = Mathf.Max(maxChildren, children);
        }

        return Mathf.Clamp(maxChildren, 1, 3);
    }

    private void BuildRoomConnections()
    {
        foreach (Tile room in generatedRooms.Values)
        {
            foreach (DoorMask direction in Directions)
            {
                if ((room.Openings & direction) == 0)
                    continue;

                Vector2Int neighbourPosition =
                    room.GridPosition + DirectionToVector(direction);

                if (!generatedRooms.TryGetValue(
                    neighbourPosition,
                    out Tile neighbour))
                {
                    continue;
                }


                if ((neighbour.Openings & Opposite(direction)) == 0)
                    continue;

                room.SetNeighbour(direction, neighbour);
            }
        }

        Debug.Log("Connexions entre les salles terminées.");
    }



    private int GetMaxLeavesFromDepth(
        int depth,
        int maxChildren,
        int targetCount)
    {
        int possibleLeaves = 1;

        for (int i = depth; i < maxDepth; i++)
        {
            possibleLeaves = Mathf.Min(
                targetCount,
                possibleLeaves * maxChildren
            );
        }

        return possibleLeaves;
    }


    private int GetMaximumPossibleTerminalCount(
        RoomNode currentNode,
        Queue<RoomNode> pendingRooms,
        List<RoomNode> terminalRooms,
        int candidateChildren,
        int maxChildren,
        int targetCount)
    {
        int possibleLeaves = terminalRooms.Count;


        foreach (RoomNode pendingNode in pendingRooms)
        {
            possibleLeaves += GetMaxLeavesFromDepth(
                pendingNode.depth,
                maxChildren,
                targetCount
            );

            if (possibleLeaves >= targetCount)
                return targetCount;
        }


        possibleLeaves += candidateChildren * GetMaxLeavesFromDepth(
            currentNode.depth + 1,
            maxChildren,
            targetCount
        );

        return Mathf.Min(possibleLeaves, targetCount);
    }


    private int GetMinimumChildrenNeeded(
        RoomNode node,
        Queue<RoomNode> pendingRooms,
        List<RoomNode> terminalRooms,
        int targetCount,
        int maxChildren,
        int maxAllowedChildren)
    {
        for (int count = 1; count <= maxAllowedChildren; count++)
        {
            int possibleLeaves = GetMaximumPossibleTerminalCount(
                node,
                pendingRooms,
                terminalRooms,
                count,
                maxChildren,
                targetCount
            );

            if (possibleLeaves >= targetCount)
                return count;
        }

        return maxAllowedChildren + 1;
    }



    private void PrepareSpecialRoomPool()
    {
        shuffledSpecialRooms.Clear();
        shuffledSpecialRooms.AddRange(specialRooms);

        ShuffleSpecialRooms();

        specialRoomIndex = 0;
    }


    private void ShuffleSpecialRooms()
    {
        for (int i = shuffledSpecialRooms.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);

            RoomDefinition temp = shuffledSpecialRooms[i];
            shuffledSpecialRooms[i] = shuffledSpecialRooms[j];
            shuffledSpecialRooms[j] = temp;
        }
    }


    private RoomDefinition GetNextSpecialRoom()
    {
        if(shuffledSpecialRooms.Count == 0)
        {
            Debug.Log("Aucune salle spéciale disponible.");
            return null;
        }
        int randomIndex = rng.Next(shuffledSpecialRooms.Count);
        RoomDefinition roomToReturn = shuffledSpecialRooms[randomIndex];
        shuffledSpecialRooms.RemoveAt(randomIndex);
        return roomToReturn;
    }


    private int ChooseChildCount()
    {
        double roll = rng.NextDouble();

        if (roll < threeChildrenChance)
            return 3;

        if (roll < threeChildrenChance + twoChildrenChance)
            return 2;

        return 1;
    }

    private bool TryChooseNormalRoom(
        RoomNode node,
        int desiredChildren,
        HashSet<Vector2Int> occupied,
        out RoomChoice choice)
    {
        List<RoomChoice> candidates = new List<RoomChoice>();

        foreach (RoomDefinition definition in normalRooms)
        {
            if (definition == null || definition.prefab == null)
                continue;

            for (int rotation = 0; rotation < 4; rotation++)
            {
                DoorMask mask = RotateMask(definition.openings, rotation);


                if ((mask & node.parentDoor) == 0)
                    continue;

                DoorMask childrenMask = mask & ~node.parentDoor;

                if (CountDoors(childrenMask) != desiredChildren)
                    continue;

                bool canPlace = true;

                foreach (DoorMask direction in Directions)
                {
                    if ((childrenMask & direction) == 0)
                        continue;

                    Vector2Int target =
                        node.position + DirectionToVector(direction);

                    if (occupied.Contains(target))
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (!canPlace)
                    continue;

                candidates.Add(new RoomChoice
                {
                    definition = definition,
                    openings = mask,
                    rotationSteps = rotation
                });
            }
        }

        if (candidates.Count == 0)
        {
            choice = default;
            return false;
        }

        choice = candidates[rng.Next(candidates.Count)];
        return true;
    }


    private bool PlaceTerminalRoom(RoomDefinition definition, RoomNode node)
    {
        if (definition == null || definition.prefab == null)
        {
            Debug.LogError("Salle de fin manquante.");
            return false;
        }

        if (CountDoors(definition.openings) != 1)
        {
            Debug.LogError(
                $"La salle {definition.name} doit avoir exactement une ouverture."
            );
            return false;
        }

        for (int rotation = 0; rotation < 4; rotation++)
        {
            DoorMask mask = RotateMask(definition.openings, rotation);


            if (mask != node.parentDoor)
                continue;

            RoomChoice choice = new RoomChoice
            {
                definition = definition,
                openings = mask,
                rotationSteps = rotation
            };

            PlaceRoom(choice, node.position);
            return true;
        }

        Debug.LogError(
            $"Impossible d'orienter correctement la salle {definition.name}."
        );

        return false;
    }


    private bool ValidateDefinitions()
    {
        if (startRoom == null || startRoom.prefab == null)
        {
            Debug.LogError("Salle de départ manquante.");
            return false;
        }

        if (normalRooms == null || normalRooms.Length == 0)
        {
            Debug.LogError("Aucune salle normale configurée.");
            return false;
        }


        HashSet<RoomDefinition> uniqueRooms = new HashSet<RoomDefinition>();

        foreach (RoomDefinition room in specialRooms)
        {
            if (!IsValidTerminal(room))
            {
                Debug.LogError(
                    "Une salle spéciale est manquante ou ne possède pas exactement une ouverture."
                );
                return false;
            }

            if (!uniqueRooms.Add(room))
            {
                Debug.LogError(
                    "Les six salles spéciales doivent être différentes."
                );
                return false;
            }
        }

        return true;
    }


    private bool IsValidTerminal(RoomDefinition definition)
    {
        return definition != null &&
               definition.prefab != null &&
               CountDoors(definition.openings) == 1;
    }

    private void PlaceRoom(RoomChoice choice, Vector2Int gridPosition)
    {
        Quaternion rotation = Quaternion.Euler(
            0f,
            0f,
            choice.rotationSteps * 90f
        );

        GameObject instance = Instantiate(
            choice.definition.prefab,
            GridToWorld(gridPosition),
            rotation,
            roomsParent
        );


        NainBase[] nains = instance.GetComponentsInChildren<NainBase>(true);

        foreach (NainBase nain in nains)
        {

            nain.transform.rotation =
                Quaternion.Inverse(instance.transform.rotation)
                * nain.transform.rotation;
        }

        Tile generatedRoom = instance.GetComponent<Tile>();

        if (generatedRoom == null)
        {
            Debug.LogError(
                $"Le prefab {instance.name} ne possède pas Tile."
            );

            return;
        }

        generatedRoom.Initialize(
            gridPosition,
            choice.openings,
            choice.rotationSteps
        );

        generatedRooms[gridPosition] = generatedRoom;
    }

    private Vector3 GridToWorld(Vector2Int position)
    {
        return new Vector3(
            position.x * roomSize.x,
            position.y * roomSize.y,
            0f
        );
    }

    private static Vector2Int DirectionToVector(DoorMask direction)
    {
        switch (direction)
        {
            case DoorMask.Up: return Vector2Int.up;
            case DoorMask.Right: return Vector2Int.right;
            case DoorMask.Down: return Vector2Int.down;
            case DoorMask.Left: return Vector2Int.left;
            default: return Vector2Int.zero;
        }
    }

    private static DoorMask Opposite(DoorMask direction)
    {
        switch (direction)
        {
            case DoorMask.Up: return DoorMask.Down;
            case DoorMask.Right: return DoorMask.Left;
            case DoorMask.Down: return DoorMask.Up;
            case DoorMask.Left: return DoorMask.Right;
            default: return DoorMask.None;
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

    private static int CountDoors(DoorMask mask)
    {
        int count = 0;

        foreach (DoorMask direction in Directions)
        {
            if ((mask & direction) != 0)
                count++;
        }

        return count;
    }

    private void ClearRooms()
    {
        if (roomsParent == null)
            return;

        for (int i = roomsParent.childCount - 1; i >= 0; i--)
        {
            GameObject child = roomsParent.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }
}
