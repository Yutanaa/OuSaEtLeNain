using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class CrazyRoomManager : MonoBehaviour
{
    [Header("Refeerences")]
    [SerializeField] private GameObject _TeleportPoints;
    private List<Transform> _listTPPoints;

    public List<Transform> ListTPPoints { get => _listTPPoints; set => _listTPPoints = value; }

    private void Awake()
    {
        ListTPPoints = new List<Transform>();
        foreach (Transform child in _TeleportPoints.transform)
        {
            ListTPPoints.Add(child);
        }
    }

    public Transform GetRandomTPPoint()
    {
        if (ListTPPoints.Count == 0)
            return null;
        int randomIndex = Random.Range(0, ListTPPoints.Count);
        return ListTPPoints[randomIndex];
    }
}
