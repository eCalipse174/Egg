using System;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public static Setting Instance { get; private set; }

    [SerializeField] private Toggle autoSellToggle;
    [SerializeField] private Slider autoSellTierSlider;
    [SerializeField] private Text autoSellTierValueText;
    [SerializeField] private Toggle autoGachaToggle;

    public bool AutoSellEnabled { get; private set; }
    public int AutoSellTierThreshold { get; private set; }
    public bool AutoGachaEnabled { get; private set; }

    public event Action<bool> OnAutoGachaChanged;

    private const string PrefAutoSellEnabled = "auto_sell_enabled";
    private const string PrefAutoSellTier = "auto_sell_tier_threshold";
    private const string PrefAutoGachaEnabled = "auto_gacha_enabled";

    private void Awake()
    {
        Instance = this;
        LoadSettings();
    }

    private void Start()
    {
        autoSellToggle.isOn = AutoSellEnabled;
        autoSellTierSlider.value = AutoSellTierThreshold;
        autoGachaToggle.isOn = AutoGachaEnabled;
        UpdateTierValueText();

        autoSellToggle.onValueChanged.AddListener(OnAutoSellToggleChanged);
        autoSellTierSlider.onValueChanged.AddListener(OnAutoSellTierChanged);
        autoGachaToggle.onValueChanged.AddListener(OnAutoGachaToggleChanged);

        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnAutoSellToggleChanged(bool value)
    {
        AutoSellEnabled = value;
        SaveSettings();
    }

    private void OnAutoSellTierChanged(float value)
    {
        AutoSellTierThreshold = Mathf.RoundToInt(value);
        UpdateTierValueText();
        SaveSettings();
    }

    private void OnAutoGachaToggleChanged(bool value)
    {
        AutoGachaEnabled = value;
        SaveSettings();
        OnAutoGachaChanged?.Invoke(value);
    }

    private void UpdateTierValueText()
    {
        if (autoSellTierValueText != null)
        {
            autoSellTierValueText.text = EggVisualDatabase.Instance.TierList.list[AutoSellTierThreshold].name;
        }
    }

    // enhance_level 0 to 100 maps to interval 1s down to 0.25s, linear
    public float GetAutoGachaInterval()
    {
        int enhanceLevel = GameManager.Instance.UserInfo.enhance_level;
        float t = Mathf.Clamp01(enhanceLevel / 100f);
        return Mathf.Lerp(1f, 0.25f, t);
    }

    // tier below the threshold and not a newly discovered item gets auto sold
    public bool ShouldAutoSell(int tier, bool isNewItem)
    {
        if (!AutoSellEnabled)
        {
            return false;
        }
        if (isNewItem)
        {
            return false;
        }
        return tier < AutoSellTierThreshold;
    }

    private void LoadSettings()
    {
        AutoSellEnabled = PlayerPrefs.GetInt(PrefAutoSellEnabled, 0) == 1;
        AutoSellTierThreshold = PlayerPrefs.GetInt(PrefAutoSellTier, 0);
        AutoGachaEnabled = PlayerPrefs.GetInt(PrefAutoGachaEnabled, 0) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(PrefAutoSellEnabled, AutoSellEnabled ? 1 : 0);
        PlayerPrefs.SetInt(PrefAutoSellTier, AutoSellTierThreshold);
        PlayerPrefs.SetInt(PrefAutoGachaEnabled, AutoGachaEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}