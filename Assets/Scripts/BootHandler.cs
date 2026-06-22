using UnityEngine;
using UnityEngine.SceneManagement;


public class BootHandler : MonoBehaviour
{
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private ConversationLibrary conversationLibrary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        conversationLibrary.Load();
        cardLibrary.Load();
        SceneManager.LoadScene("MenuScene");        
    }

}
