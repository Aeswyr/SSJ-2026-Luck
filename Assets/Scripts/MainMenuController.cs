using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuParent;
    [SerializeField] private GameObject settingsMenuParent;

    void Start()
    {
        settingsMenuParent.SetActive(false);
    }
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
        settingsMenuParent.SetActive(true);
        mainMenuParent.SetActive(false);
    }

    public void OnReturnPressed()
    {
        settingsMenuParent.SetActive(false);
        mainMenuParent.SetActive(true);
    }

    public void OnResetSavePressed()
    {
        SaveManager.Instance.ClearSave("save");
    }
}
