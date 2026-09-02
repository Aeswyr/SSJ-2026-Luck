using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayPressed()
    {
        SaveManager.Instance.LoadSave("save");
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

    public void OnSettingsPressed()
    {
        SaveManager.Instance.ClearSave("save");
    }
}
