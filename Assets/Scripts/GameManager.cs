using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int nbNains = 0;
    public bool isInGame = false;
    public Canvas gameUI;
    public TextMeshProUGUI nbNainAffich;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("already an instance running", gameObject);
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    public void RestartGame()
    {
        nbNains = 0;
        gameUI.enabled = true;
        nbNainAffich.text = nbNains.ToString();
    }

    public void EndGame()
    {
        gameUI.enabled = false;
        SceneLoadManager.Instance.LoadEndGame();
    }

    public void AddNain(int val)
    {
        nbNains += val;
        nbNainAffich.text = nbNains.ToString();
        if (nbNains >= 6)
        {
            EndGame();
        }
    }

    public int GetNbNain()
    {
        return nbNains;
    }
}
