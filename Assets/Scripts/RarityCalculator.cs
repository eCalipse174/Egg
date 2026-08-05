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

        Debug.Log(Gacha().Egg_Name + $" {temprarity:F5}");
        Debug.Log(Gacha().Egg_Name + $" {temprarity:F5}");
        Debug.Log(Gacha().Egg_Name + $" {temprarity:F5}");
        Debug.Log(Gacha().Egg_Name + $" {temprarity:F5}");
        Debug.Log(Gacha().Egg_Name + $" {temprarity:F5}");
    }

    public Item Gacha()
    {
        int tier = (int)Roll(tierList.list, GetTierWeight).id;                              //레벨 기반 티어 뽑기
        double t1 = temprarity;

        int id = Roll(ItemManager.Instance.GetItemIdsByTier(tier).ToList(), GetItemWeight); //아이템 고유 가중치 기반 아이템 뽑기
        double t2 = temprarity;

        temprarity = t1 * t2 * 100;
        return ItemManager.Instance.GetItemById(id);
    }

    public Item Gacha(out double rarity)
    {
        var item = Gacha();
        rarity = temprarity;
        return item;
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
                temprarity = getWeight(list[i]) / totalWeight;
                return list[i];
            }
        }

        return list[0];
    }


    private double GetTierWeight(Tier tier)
    {
        return TierRarityCalc(
            GameManager.Instance.UserInfo.enhance_level,
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