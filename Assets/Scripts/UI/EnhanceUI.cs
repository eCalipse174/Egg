using UnityEngine;
using UnityEngine.UI;

public class EnhanceUI : MonoBehaviour
{
    [SerializeField] private Text levelText;
    [SerializeField] private Text priceText;
    [SerializeField] private Button enhanceButton;

    private void OnEnable()
    {
        EnhanceManager.Instance.OnEnhanceLevelChanged += HandleLevelChanged;
        Refresh();
    }

    private void OnDisable()
    {
        EnhanceManager.Instance.OnEnhanceLevelChanged -= HandleLevelChanged;
    }

    private void HandleLevelChanged(int newLevel)
    {
        Refresh();
    }

    public void OnEnhanceButtonClicked()
    {
        EnhanceManager.Instance.Enhance(success => { });
    }

    private void Refresh()
    {
        if (EnhanceManager.Instance.IsMaxLevel())
        {
            enhanceButton.interactable = false;
            levelText.text = "Level Max";
            priceText.gameObject.SetActive(false);
            return;
        }

        int currentLevel = EnhanceManager.Instance.GetCurrentLevel();
        long cost = EnhanceManager.Instance.GetUpgradeCost();

        enhanceButton.interactable = true;
        levelText.text = "Level Up \n(" + currentLevel + "->" + (currentLevel + 1) + ")";
        priceText.gameObject.SetActive(true);
        priceText.text = cost.ToString() + "G";
    }
}