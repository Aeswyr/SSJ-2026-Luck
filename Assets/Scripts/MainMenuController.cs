using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayPressed()
    {
        ScreenWipeManager.Instance.PlayWipeOn(() =>
        {
            InputHandler.Instance.FlushBuffer();
            SceneManager.LoadScene("GameScene");
        });
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
