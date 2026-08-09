using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour
{
    [SerializeField] private Text gold;

    private bool autoGachaRunning;

    private void Start()
    {
        UpdateGold(0);
        InventoryManager.Instance.OnItemSold += UpdateGold;
        EnhanceManager.Instance.OnEnhanceLevelChanged += UpdateGold;

        Setting.Instance.OnAutoGachaChanged += HandleAutoGachaChanged;
        if (Setting.Instance.AutoGachaEnabled)
        {
            HandleAutoGachaChanged(true);
        }
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnItemSold -= UpdateGold;
        EnhanceManager.Instance.OnEnhanceLevelChanged -= UpdateGold;
        Setting.Instance.OnAutoGachaChanged -= HandleAutoGachaChanged;
    }

    public void Gacha()
    {
        if (InventoryManager.Instance.IsInventoryFull())
        {
            Debug.LogWarning("inventory is full, gacha blocked");
            ContinueAutoGachaIfNeeded();
            return;
        }

        GameManager.Instance.IncreaseGacha();

        var item = RarityCalculator.Instance.Gacha(out var rarity);
        Debug.Log($"{item.Egg_Name} {rarity:F5}");

        EggVisualInfo visualInfo = EggVisualDatabase.Instance.GetVisualInfo(item.ID);
        bool isNewItem = !CollectionManager.Instance.IsUnlocked(item.ID);

        if (isNewItem)
        {
            CollectionManager.Instance.UnlockItem(item.ID, success => { });
        }

        if (visualInfo != null && visualInfo.HasCutscene)
        {
            PlayCutscene(visualInfo, () =>
            {
                StoreInventory(item);
                ContinueAutoGachaIfNeeded();
            });
            return;
        }

        if (Setting.Instance.ShouldAutoSell((int)item.Tier, isNewItem))
        {
            AutoSellItem(item);
            return;
        }

        ShowPresentation(item, visualInfo);
    }

    private void ShowPresentation(Item item, EggVisualInfo visualInfo)
    {
        StoreInventory(item);

        Sprite icon = visualInfo != null ? visualInfo.Sprite : null;
        GachaPresentation.Instance.Show(item, icon, ContinueAutoGachaIfNeeded);
    }

    private void PlayCutscene(EggVisualInfo visualInfo, Action onComplete)
    {
        CutscenePlayer.Instance.Play(visualInfo.CutsceneClip, onComplete);
    }

    private void AutoSellItem(Item item)
    {
        long newGold = GameManager.Instance.UserInfo.gold + item.price;
        NetworkManager.Instance.UpdateGold(GameManager.Instance.UserInfo.id, newGold, (success, json) =>
        {
            if (success)
            {
                GameManager.Instance.UserInfo.gold = newGold;
                UpdateGold(0);
            }
            else
            {
                Debug.LogWarning("auto sell gold update failed");
            }

            ContinueAutoGachaIfNeeded();
        });
    }

    public void UpdateGold(int _)
    {
        gold.text = GameManager.Instance.UserInfo.gold.ToString() + "G";
    }

    private void StoreInventory(Item item)
    {
        InventoryManager.Instance.AddItem(
            GameManager.Instance.UserInfo.id,
            item.ID,
            (success) =>
            {
                if (success)
                    Debug.Log($"inventory save complete: {item.Egg_Name}");
            });
    }

    private void HandleAutoGachaChanged(bool enabled)
    {
        autoGachaRunning = enabled;
        if (enabled)
        {
            Gacha();
        }
    }

    private void ContinueAutoGachaIfNeeded()
    {
        if (autoGachaRunning)
        {
            StartCoroutine(AutoGachaDelay());
        }
    }

    private IEnumerator AutoGachaDelay()
    {
        yield return new WaitForSeconds(Setting.Instance.GetAutoGachaInterval());
        if (autoGachaRunning)
        {
            Gacha();
        }
    }

    public void Quit()
    {
        StartCoroutine(GameManager.Instance.SaveAndQuit());
    }
}