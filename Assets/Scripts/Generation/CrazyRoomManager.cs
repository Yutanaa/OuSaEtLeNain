using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class CrazyRoomManager : MonoBehaviour
{
    [Header("Refeerences")]
    [SerializeField] private GameObject _TeleportPoints;
    private List<Transform> _listTPPoints;

    public List<CrazyNain> ListCrazyNains { get; private set; }

    public List<Transform> ListTPPoints { get => _listTPPoints; set => _listTPPoints = value; }

    public GameObject nainBecile;
    private Transform startNainBecilePos;

    private void Awake()
    {
        ListTPPoints = new List<Transform>();

        foreach (Transform child in _TeleportPoints.transform)
        {
            ListTPPoints.Add(child);
        }

        startNainBecilePos = nainBecile.transform;
    }


    public Transform GetRandomTPPoint(Transform pointActuel)
    {

        if (ListTPPoints.Count == 0)
            return null;
        // Retourne un point de téléportation aléatoire à partir de la liste qui n'est pas le point actuel;
        int randomIndex = Random.Range(0, ListTPPoints.Count);
        while (ListTPPoints[randomIndex] == pointActuel)
        {
            randomIndex = Random.Range(0, ListTPPoints.Count);
        }
        return ListTPPoints[randomIndex];

    }

    public void AddCrazyNain(CrazyNain nain)
    {
        if (ListCrazyNains == null)
            ListCrazyNains = new List<CrazyNain>();
        if (!ListCrazyNains.Contains(nain))
            ListCrazyNains.Add(nain);
        if(ListCrazyNains.Count == 2)
        {
            //TODO : RECUPERER LES NAINS
            Debug.Log("2 NAINS DANS LA LISTE");
            foreach(CrazyNain n in ListCrazyNains)
            {
                Destroy(n.gameObject);
            }
        }
    }

    public void StartCrazyRoom()
    {
        nainBecile.transform.position = startNainBecilePos.position;
        nainBecile.SetActive(true); 
    }

    public void ResetRoom()
    {
        nainBecile.transform.position = startNainBecilePos.position;
        nainBecile.SetActive(false);
    }
}
