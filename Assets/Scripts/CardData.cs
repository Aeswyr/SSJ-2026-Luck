using System;
using JetBrains.Annotations;
using UnityEngine;

[Serializable] public struct CardData
{
    public string name;
    public CardID id;
    public CardRarity rarity;
    public Sprite icon;
    public int charges;
}


public enum CardRarity
{
    COMMON, UNCOMMON, RARE
}

public enum CardID
{
    HIGH_CARD, PAIR, FULL_HOUSE, JOKER, HIDDEN_ACE,
    REND, RECALL, HOMECOMING, ALL_IN, FINISHING_STROKE,
    CALLED_SHOT, CALLBACK, PEERLESS_FOCUS, DOUBLE_DOWN, THOUSAND_CUTS,
    DECK_FIXING, CLOWN_CAR, MISSED_CONNECTION,
    INSIGHT, FOOLIN_AROUND
}