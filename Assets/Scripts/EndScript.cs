using UnityEngine;

public class EndScript : MonoBehaviour
{
    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
        SceneLoadManager.Instance.LoadGame();
    }
}
