
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform mapContainer;
    [SerializeField] private Image roomIconPrefab;
    [SerializeField] private Image connectionPrefab;

    [Header("Map Layout")]
    [Tooltip("Doit correspondre au roomSize du générateur.")]
    [SerializeField] private Vector2 roomWorldSize = new Vector2(20, 12);

    [Tooltip("Espacement entre deux icônes sur la minimap.")]
    [SerializeField] private Vector2 mapRoomSpacing = new Vector2(22, 22);

    [SerializeField] private float connectionThickness = 4f;

    [Header("Appearance")]
    [SerializeField] private Color visitedRoomColor = Color.white;
    [SerializeField] private Color currentRoomColor = Color.cyan;
    [SerializeField] private Color connectionColor = Color.white;

    private Dictionary<Vector2Int, Image> discoveredRooms =
        new Dictionary<Vector2Int, Image>();

    private Vector3 originWorldPosition;

    private Image currentRoomIcon;
    private bool isInitialized;

    private static readonly DoorMask[] Directions =
    {
        DoorMask.Up,
        DoorMask.Right,
        DoorMask.Down,
        DoorMask.Left
    };

    public void Initialize(Tile startingRoom)
    {
        if (startingRoom == null)
        {
            Debug.LogError("Minimap : salle de départ manquante.");
            return;
        }

        if (mapContainer == null || roomIconPrefab == null)
        {
            Debug.LogError("Minimap : références UI manquantes.");
            return;
        }

        if (roomWorldSize.x <= 0 || roomWorldSize.y <= 0)
        {
            Debug.LogError("Minimap : roomWorldSize invalide.");
            return;
        }

        for (int i = mapContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(mapContainer.GetChild(i).gameObject);
        }

        discoveredRooms.Clear();
        currentRoomIcon = null;

        originWorldPosition = startingRoom.transform.position;
        isInitialized = true;

        VisitRoom(startingRoom);
    }

    public void VisitRoom(Tile room)
    {
        if (!isInitialized || room == null)
            return;

        Vector2Int gridPosition = WorldToGrid(room);

        if (discoveredRooms.TryGetValue(gridPosition, out Image existingIcon))
        {
            if (currentRoomIcon != null)
                currentRoomIcon.color = visitedRoomColor;

            currentRoomIcon = existingIcon;
            currentRoomIcon.color = currentRoomColor;

            return;
        }

        if (currentRoomIcon != null)
            currentRoomIcon.color = visitedRoomColor;


        Image newIcon = Instantiate(room.tileIcon, mapContainer);


        newIcon.rectTransform.rotation = room.transform.rotation;
        RectTransform iconRect = newIcon.rectTransform;

        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);

        iconRect.anchoredPosition = GridToMap(gridPosition);

        newIcon.color = currentRoomColor;

        discoveredRooms.Add(gridPosition, newIcon);
        currentRoomIcon = newIcon;


        foreach (DoorMask direction in Directions)
        {
            Tile neighbour = room.GetNeighbour(direction);

            if (neighbour == null)
                continue;

            Vector2Int neighbourPosition = WorldToGrid(neighbour);

            if (discoveredRooms.ContainsKey(neighbourPosition))
            {
                CreateConnection(gridPosition, neighbourPosition);
            }
        }

        newIcon.transform.SetAsLastSibling();

        Debug.Log("Minimap : salle découverte " + gridPosition);
    }

    private Vector2Int WorldToGrid(Tile room)
    {
        Vector3 offset = room.transform.position - originWorldPosition;

        return new Vector2Int(
            Mathf.RoundToInt(offset.x / roomWorldSize.x),
            Mathf.RoundToInt(offset.y / roomWorldSize.y)
        );
    }

    private Vector2 GridToMap(Vector2Int gridPosition)
    {
        return new Vector2(
            gridPosition.x * mapRoomSpacing.x,
            gridPosition.y * mapRoomSpacing.y
        );
    }
    private void CreateConnection(
        Vector2Int from,
        Vector2Int to)
    {
        if (connectionPrefab == null)
            return;

        Image connection = Instantiate(connectionPrefab, mapContainer);

        RectTransform rect = connection.rectTransform;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Vector2 startPosition = GridToMap(from);
        Vector2 endPosition = GridToMap(to);

        Vector2 difference = endPosition - startPosition;

        rect.anchoredPosition = (startPosition + endPosition) / 2f;

        rect.sizeDelta = new Vector2(
            difference.magnitude,
            connectionThickness
        );

        float angle = Mathf.Atan2(
            difference.y,
            difference.x
        ) * Mathf.Rad2Deg;

        rect.localRotation = Quaternion.Euler(0, 0, angle);

        connection.color = connectionColor;


        connection.transform.SetAsFirstSibling();
    }
}
