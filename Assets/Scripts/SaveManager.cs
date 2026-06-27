using UnityEngine;
using System.Collections.Generic;

public class SaveManager : Singleton<SaveManager>
{
    List<string> conversationsHeard = new();

    public void SaveConversationHeard(string key)
    {
        if (!conversationsHeard.Contains(key))
            conversationsHeard.Add(key);
    }

    public bool ConversationHeard(string key)
    {
        return conversationsHeard.Contains(key);
    }
}
