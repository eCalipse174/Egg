using System;
using UnityEngine;

public class EnhanceManager : MonoBehaviour
{
    public static EnhanceManager Instance;

    public event Action<int> OnEnhanceLevelChanged;

    private const int MaxLevel = 100;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsMaxLevel()
    {
        return GameManager.Instance.UserInfo.enhance_level >= MaxLevel;
    }

    public int GetCurrentLevel()
    {
        return GameManager.Instance.UserInfo.enhance_level;
    }

    public long GetUpgradeCost()
    {
        return EnhancementCostCalculator.GetUpgradeCost(GetCurrentLevel());
    }

    public void Enhance(Action<bool> onComplete)
    {
        if (IsMaxLevel())
        {
            onComplete?.Invoke(false);
            return;
        }

        long cost = GetUpgradeCost();
        long currentGold = GameManager.Instance.UserInfo.gold;

        if (currentGold < cost)
        {
            onComplete?.Invoke(false);
            return;
        }

        long newGold = currentGold - cost;
        int newLevel = GetCurrentLevel() + 1;

        NetworkManager.Instance.UpdateGold(GameManager.Instance.UserInfo.id, newGold, (goldSuccess, goldJson) =>
        {
            if (!goldSuccess)
            {
                Debug.LogWarning("enhance gold update failed");
                onComplete?.Invoke(false);
                return;
            }

            GameManager.Instance.UserInfo.gold = newGold;

            NetworkManager.Instance.UpdateEnhanceLevel(GameManager.Instance.UserInfo.id, newLevel, (levelSuccess, levelJson) =>
            {
                if (levelSuccess)
                {
                    GameManager.Instance.UserInfo.enhance_level = newLevel;
                    OnEnhanceLevelChanged?.Invoke(newLevel);
                }
                else
                {
                    Debug.LogWarning("enhance level update failed");
                }
                onComplete?.Invoke(levelSuccess);
            });
        });
    }
}