using NUnit.Framework.Interfaces;
using System;

[Serializable]
public class Item
{
    public int ID;
    public string Egg_Name;
    public string Egg_SubTitle;
    public string Egg_Desc;
    public TierName Tier;
    public double Weight;
    public long price;

    public void Print()
    {
        UnityEngine.Debug.Log($"{ID} {Egg_Name} {Egg_SubTitle} {Egg_Desc} {Tier} {Weight}");
    }
}

[Serializable]
public class ItemListResponse
{
    public Item[] list;
}