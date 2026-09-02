using UnityEngine;
using System.Collections.Generic;

public class SaveManager : Singleton<SaveManager>
{
    List<string> conversationsHeard = new();
    private string activeSaveSlot;
    private string conversationKey;

    public void LoadSave(string slot)
    {
        activeSaveSlot = slot;
        conversationKey = slot + "-CONVERSATION";
        if (PlayerPrefs.HasKey(activeSaveSlot))
        {
            var rawConvo = PlayerPrefs.GetString(conversationKey);
            if (rawConvo != null) {
                conversationsHeard = new (rawConvo.Split(','));
            }
        }
    }

    public void ClearSave(string slot)
    {
        PlayerPrefs.SetString(slot + "-CONVERSATION", null);
    }

    public void SaveConversationHeard(string key)
    {
        if (!conversationsHeard.Contains(key))
            conversationsHeard.Add(key);

        SaveAll();
    }

    public bool ConversationHeard(string key)
    {
        return conversationsHeard.Contains(key);
    }

    public void SaveAll()
    {
        string convoChain = "";
        foreach (var key in conversationsHeard)
        {
            convoChain += key + ",";
        }
        convoChain.Remove(convoChain.Length - 1);
        
        PlayerPrefs.SetInt(activeSaveSlot, 1);
        PlayerPrefs.SetString(conversationKey, convoChain);
    }
}
