using System;
using System.Collections.Generic;
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
        Debug.Log($"{Roll(tierList.list, GetTierWeight).name} / {temprarity:F10}");
        Debug.Log($"{Roll(tierList.list, GetTierWeight).name} / {temprarity:F10}");
        Debug.Log($"{Roll(tierList.list, GetTierWeight).name} / {temprarity:F10}");
        Debug.Log($"{Roll(tierList.list, GetTierWeight).name} / {temprarity:F10}");
        Debug.Log($"{Roll(tierList.list, GetTierWeight).name} / {temprarity:F10}");
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
            GameManager.Instance.UserInfo.enhance_level,
            tier.unlockLevel,
            tier.rarityBase0,
            tier.rarityBase100
        );
    }

    //private double GetItemWeight()
}