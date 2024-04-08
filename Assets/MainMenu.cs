using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartScene()
    {
        SceneManager.LoadScene("GamePlay Level01");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
