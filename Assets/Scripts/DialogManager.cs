using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DialogManager : Singleton<DialogManager>
{
    [SerializeField] private ConversationLibrary conversationLibrary;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI nameplateLeft;
    [SerializeField] private TextMeshProUGUI nameplateRight;
    [SerializeField] private GameObject dialogParent;

    private Conversation? activeDialog;
    private int conversationIndex;
    List<string> conversationsHeard;

    Action callback;

    void Start()
    {
        conversationsHeard = new();
        dialogParent.SetActive(false);
    }

    public void PlayConversation(string key, Action callback = null)
    {
        if (key.Contains(','))
        {
            var split = key.Split(',');
            switch (split[0])
            {
                case "!s":
                    Debug.Log("attempting to play a seqeuntial conversation");
                    split[0] = null;
                    foreach (var k in split)
                    {
                        if (!string.IsNullOrEmpty(k) && !conversationsHeard.Contains(k))
                        {
                            Debug.Log($"playing conversation {k}");
                            PlayConversation(k, callback);
                            break;
                        }
                        
                    }
                    return;
                case "!r":
                    PlayConversation(split[Random.Range(1, split.Length)], callback);
                    return;
                default:
                    break;
            }
        }

        dialogParent.SetActive(true);
        FindAnyObjectByType<PlayerController>().ToggleInputLock(true);

        activeDialog = conversationLibrary.GetConversation(key);

        WriteLine(activeDialog.Value.lines[0]);
        this.callback = callback;


        conversationsHeard.Add(key);
    }

    public void FixedUpdate()
    {
        if (activeDialog != null && InputHandler.Instance.interact.pressed)
        {
            conversationIndex++;
            if (conversationIndex < activeDialog.Value.lines.Count)
            {
                WriteLine(activeDialog.Value.lines[conversationIndex]);
            } else
            {
                conversationIndex = 0;
                activeDialog = null;
                dialogParent.SetActive(false);
                FindAnyObjectByType<PlayerController>().ToggleInputLock(false);

                callback?.Invoke();
            }
        }
    }

    private void WriteLine(DialogLine line)
    {
        bodyText.text = line.text;

        if (line.flip)
        {
            nameplateLeft.transform.parent.gameObject.SetActive(false);
            nameplateRight.transform.parent.gameObject.SetActive(true);
            nameplateRight.text = line.name;
        } else
        {
            nameplateRight.transform.parent.gameObject.SetActive(false);
            nameplateLeft.transform.parent.gameObject.SetActive(true);
            nameplateLeft.text = line.name;
        }
    }
}
