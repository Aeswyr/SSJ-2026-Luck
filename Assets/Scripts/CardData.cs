using System;
using UnityEngine;

[Serializable] public struct CardData
{
    public string name;
    public CardID id;
    public CardRarity rarity;
    public Sprite icon;
    [Space]
    [Header("Gameplay Info")]
    public CardType type;
    public bool exhaust;
    public bool noDiscard;
    [Space]
    [Header("Combat Info")]
    public int charges;
    public int baseDamage;
    public bool shouldStick;
}


public enum CardRarity
{
    COMMON, UNCOMMON, RARE, NONE
}

public enum CardID
{
    HIGH_CARD, PAIR, FULL_HOUSE, JOKER, HIDDEN_ACE,
    REND, RECALL, HOMECOMING, ALL_IN, FINISHING_STROKE,
    CALLED_SHOT, CALLBACK, PEERLESS_FOCUS, BLEED_OUT, THOUSAND_CUTS,
    DECK_FIXING, CLOWN_CAR, MISSED_CONNECTION,
    INSIGHT, FOOLIN_AROUND, DOUBLE_DOWN, SECOND_CHANCE, JACKPOT, LUCKY_SEVENS,
    FLUSH, INNER_STRENGTH, RAISE_THE_STAKES,
    DEALERS_ADVANTAGE, TRIPLE_DEAL, LUCKY_DEAL
}

public enum CardType
{
    ATTACK, SPECIAL, BUFF, DEBUFF, INSTANT
}