using TMPro;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    public TextMeshProUGUI nbNainText;

    void Start()
    {
        nbNainText.text = "Vous avez récupéré " + GameManager.Instance.GetNbNain() + " nains !";
    }
    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
        SceneLoadManager.Instance.LoadGame();
    }
}
