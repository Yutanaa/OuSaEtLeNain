using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int nbNains = 0;

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
    }

    public void AddNain(int val)
    {
        nbNains += val;
    }

    public int GetNbNain()
    {
        return nbNains;
    }
}
