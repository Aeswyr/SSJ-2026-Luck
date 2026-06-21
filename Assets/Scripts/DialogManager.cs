using TMPro;
using UnityEngine;

public class DialogManager : Singleton<DialogManager>
{
    [SerializeField] private ConversationLibrary conversationLibrary;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private GameObject dialogParent;

    private Conversation? activeDialog;
    private int conversationIndex;

    void Start()
    {
        dialogParent.SetActive(false);
    }

    public void PlayConversation(string key)
    {
        dialogParent.SetActive(true);
        FindAnyObjectByType<PlayerController>().ToggleInputLock(true);

        activeDialog = conversationLibrary.GetConversation(key);

        bodyText.text = activeDialog.Value.lines[0].text;
    }

    public void FixedUpdate()
    {
        if (activeDialog != null && InputHandler.Instance.interact.pressed)
        {
            conversationIndex++;
            if (conversationIndex < activeDialog.Value.lines.Count)
            {
                bodyText.text = activeDialog.Value.lines[conversationIndex].text;
            } else
            {
                conversationIndex = 0;
                activeDialog = null;
                dialogParent.SetActive(false);
                FindAnyObjectByType<PlayerController>().ToggleInputLock(false);
            }
        }
    }
}
