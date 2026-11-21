using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoomsController : MonoBehaviour
{
    [SerializeField] private List<GameObject> nainsInRoom = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            GetComponent<Collider2D>().enabled = false;

            //TODO : ADD NAIN AU JOURNAL
            foreach (GameObject nain in nainsInRoom)
            {
                if (nain != null)
                {
                    ////Oriente les nains vers le bas de l'écran

                    //nain.transform.rotation =
                    //Quaternion.Inverse(transform.rotation)
                    //* nain.transform.rotation;

                }
            }
        }
    }
}
