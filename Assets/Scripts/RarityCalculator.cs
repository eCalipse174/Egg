using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RarityCalculator : MonoBehaviour
{
    private static RarityCalculator instance;
    public static RarityCalculator Instance => instance;

    public TierList tierList;

    double temprarity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var items = ItemManager.Instance.AllItems;

        Debug.Log($"{Roll(tierList.list, GetTierWeight).name} / {temprarity:F10}\n");

        //foreach (var item in ItemManager.Instance.GetItemIdsByTier(0))
        //{
        //    Debug.Log(item);
        //}

        foreach (var i in ItemManager.Instance.GetItemIdsByTier(0).ToList())
            Debug.Log(items[i].Egg_Name + items[i].ID.ToString());

        int id;
        id = Roll(ItemManager.Instance.GetItemIdsByTier(0).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name} {items[id].Tier}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(1).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(2).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(3).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(4).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(5).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(6).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(7).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(8).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
        id = Roll(ItemManager.Instance.GetItemIdsByTier(9).ToList(), GetItemWeight);
        Debug.Log($"{items[id].Egg_Name}");
    }

    private void Update()
    {
        if (Input.anyKeyDown)
            Debug.Log($"{Roll(tierList.list, GetTierWeight).name} /{temprarity:F10}");
    }

    private double TierRarityCalc(int e, int unlockLevel, double rarityBase0, double rarityBase100)
    {
        if (e < unlockLevel)
        {
            return 0.0;
        }

        double progress = (double)(e - unlockLevel) / (100 - unlockLevel);

        double logStart = Math.Log10(rarityBase0);
        double logEnd = Math.Log10(rarityBase100);

        double logNow = logStart + (logEnd - logStart) * progress;

        double result = Math.Pow(10, logNow);

        return result;
    }

    private T Roll<T>(List<T> list, Func<T, double> getWeight)
    {
        double totalWeight = 0.0;

        List<double> weights = new();

        foreach (var item in list)
        {
            double weight = getWeight(item);

            weights.Add(weight);
            totalWeight += weight;
        }

        double roll = UnityEngine.Random.value * totalWeight;
        double cumulative = 0.0;

        for (int i = 0; i < list.Count; i++)
        {
            cumulative += weights[i];

            if (roll <= cumulative)
            {
                temprarity = getWeight(list[i]) / totalWeight * 100;
                return list[i];
            }
        }

        return list[0];
    }


    private double GetTierWeight(Tier tier)
    {
        return TierRarityCalc(
            /*GameManager.Instance.UserInfo.enhance_level*/100,
            tier.unlockLevel,
            tier.rarityBase0,
            tier.rarityBase100
        );
    }

    private double GetItemWeight(int id)
    {
        return ItemManager.Instance.GetItemById(id).Weight;
    }
}