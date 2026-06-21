using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "ConversationLibrary", menuName = "ScriptableObjects/ConversationLibrary", order = 1)]
public class ConversationLibrary : ScriptableObject
{
    [SerializeField] private string path;
    [SerializeField] List<Portrait> portraits = new ();
    static Dictionary<string, Sprite> portraitTable = new();
    static Dictionary<string, Conversation> conversations;

    public void Load()
    {
        foreach (var portrait in portraits)
            portraitTable.Add(portrait.name, portrait.sprite);

        conversations = new();

        StreamReader dialogReader = new StreamReader(path);
        dialogReader.ReadLine();// skip title line
        string line;
        Conversation currentConvo = default;
        while ((line = dialogReader.ReadLine()) != null)
        {
            string[] param = line.Split(',');
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
                
                currentConvo.lines.Add(new DialogLine().Parse(param));
            }
        }

        Debug.Log($"Added conversation with key [{currentConvo.key}]");
        conversations.Add(currentConvo.key, currentConvo);

        dialogReader.Close();
    }

    public Conversation GetConversation(string key)
    {
        return conversations[key];
    }

    public Sprite GetPortrait(string key)
    {
        return portraitTable[key];
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

    public DialogLine Parse(string[] param)
    {
        name = param[1];
        text = param[2];
        flip =  param[3].ToLower().Equals("right");
        return this;
    }
}


