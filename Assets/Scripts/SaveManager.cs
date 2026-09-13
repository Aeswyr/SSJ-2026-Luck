using UnityEngine;
using System.Collections.Generic;

public class SaveManager : Singleton<SaveManager>
{
    List<string> conversationsHeard = new();
    private string activeSaveSlot;
    private string conversationKey;
    private string booleanKey;

    List<string> booleans = new();
    public static string INTRO_SEEN = "intro_seen";

    public void LoadSave(string slot)
    {
        activeSaveSlot = slot;
        conversationKey = slot + "-CONVERSATION";
        booleanKey = slot + "-BOOLS";
        if (PlayerPrefs.HasKey(activeSaveSlot))
        {
            var rawConvo = PlayerPrefs.GetString(conversationKey);
            if (rawConvo != null) {
                conversationsHeard = new (rawConvo.Split(','));
            }

            var rawBools = PlayerPrefs.GetString(booleanKey);
            if (rawBools != null) {
                booleans = new (rawBools.Split(','));
            }
        }
    }

    public void ClearSave(string slot)
    {
        PlayerPrefs.SetString(slot + "-CONVERSATION", null);
        PlayerPrefs.SetString(slot + "-BOOLS", null);
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

    public void SaveBool(string key)
    {
        if (booleans.Contains(key))
            return;
        booleans.Add(key);

        SaveAll();
    }

    public bool GetBool(string key)
    {
        return booleans.Contains(key);
    }

    public void SaveAll()
    {
        PlayerPrefs.SetInt(activeSaveSlot, 1);

        string convoChain = "";
        foreach (var key in conversationsHeard)
        {
            convoChain += key + ",";
        }
        convoChain.Remove(convoChain.Length - 1);
        PlayerPrefs.SetString(conversationKey, convoChain);

        string boolChain = "";
        foreach (var key in booleans) {
            boolChain += key + ",";
        }
        boolChain.Remove(boolChain.Length - 1);
        PlayerPrefs.SetString(booleanKey, boolChain);
    }
}
