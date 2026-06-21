using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject menuParent;

    private bool menuEnabled;

    void Start()
    {
        menuParent.SetActive(false);
    }

    void FixedUpdate()
    {
        if (InputHandler.Instance.menu.pressed)
            ToggleMenu();
    }

    void ToggleMenu()
    {
        menuEnabled = !menuEnabled;

        menuParent.SetActive(menuEnabled);

        Time.timeScale = menuEnabled ? 0 : 1;
    }

    public void OnReturnPressed()
    {
        ToggleMenu();

        InputHandler.Instance.FlushBuffer();
    }

    public void OnEndPressed()
    {
        ToggleMenu();
        
        SceneManager.LoadScene("MenuScene");
    }
}
