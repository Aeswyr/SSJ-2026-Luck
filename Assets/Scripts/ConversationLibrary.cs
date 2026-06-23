using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConversationLibrary", menuName = "ScriptableObjects/ConversationLibrary", order = 1)]
public class ConversationLibrary : ScriptableObject
{
    [SerializeField] List<Portrait> portraits = new ();
    static Dictionary<string, Sprite> portraitTable = new();
    static Dictionary<string, Conversation> conversations;

    public void Load()
    {
        foreach (var portrait in portraits)
            portraitTable.Add(portrait.name, portrait.sprite);

        conversations = new();
        TextAsset text = Resources.Load<TextAsset>("SSJ_Luck_26 - Dialog");
        List<string> textLines = new(text.text.Split('\n'));
        textLines.RemoveAt(0);// skip title line
        string line;
        Conversation currentConvo = default;
        while (textLines.Count > 0)
        {
            line = textLines[0];
            string[] param = line.Split('\t');
            if (!string.IsNullOrEmpty(param[0]))
            {
                if (!param[0].Equals(currentConvo.key))
                {
                    if (!string.IsNullOrEmpty(currentConvo.key))
                    {
                        Debug.Log($"Added conversation with key [{currentConvo.key}]");
                        conversations.Add(currentConvo.key, currentConvo);
                        currentConvo = new();
                    }
                    currentConvo.lines = new();
                    currentConvo.key = param[0];
                }
                
                currentConvo.lines.Add(new DialogLine().Parse(param, this));
            }

            textLines.RemoveAt(0);
        }

        Debug.Log($"Added conversation with key [{currentConvo.key}]");
        conversations.Add(currentConvo.key, currentConvo);

    }

    public Conversation GetConversation(string key)
    {
        return conversations[key];
    }

    public Sprite GetPortrait(string key)
    {
        return portraitTable.ContainsKey(key) ? portraitTable[key] : null;
    }

    [Serializable] private struct Portrait
    {
        public string name;
        public Sprite sprite;
    }
}

public struct Conversation
{
    public string key;
    public List<DialogLine> lines;

}

public struct DialogLine
{
    public string name;
    public string text;
    public bool flip;
    public Sprite portrait;

    public DialogLine Parse(string[] param, ConversationLibrary library)
    {
        name = param[1];
        text = param[2];
        portrait = library.GetPortrait(param[3]);
        flip =  param[4].ToLower().Equals("right");
        
        return this;
    }
}


