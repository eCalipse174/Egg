using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Tier
{
    public TierName id;
    public string name;
    public double rarityBase0;
    public double rarityBase100;
    public int unlockLevel;
}

public enum TierName
{
    Default,
    Normal,
    Rare,
    Epic,
    Unique,
    Legendary,
    Mythic,
    Absolute,
    Unlimited,
    Infinity,
    Special
}

[CreateAssetMenu(fileName = "TierList", menuName = "ScriptableObject/TierList")]
public class TierList : ScriptableObject
{
    public List<Tier> list;
}