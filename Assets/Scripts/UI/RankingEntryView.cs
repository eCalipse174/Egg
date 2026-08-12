using System;
using UnityEngine;
using UnityEngine.UI;

public class RankingEntryView : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text nicknameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text valueText;
    [SerializeField] private Image equippedEggIcon;
    [SerializeField] private Button rowButton;

    private const int noEggEquipped = -1;

    private RankingManager.RankingEntry currentEntry;
    private Action<RankingManager.RankingEntry> onClicked;

    public void Setup(int rank, RankingManager.RankingEntry entry, Action<RankingManager.RankingEntry> onClickedCallback)
    {
        rankText.text = rank.ToString();
        nicknameText.text = entry.nickname;
        levelText.text = "Lv." + entry.enhance_level;
        valueText.text = entry.value.ToString();

        currentEntry = entry;
        onClicked = onClickedCallback;

        SetEggIcon(entry.equipped_egg_id);
    }

    private void Awake()
    {
        rowButton.onClick.AddListener(() =>
        {
            onClicked?.Invoke(currentEntry);
        });
    }

    private void SetEggIcon(int eggId)
    {
        if (eggId == noEggEquipped)
        {
            equippedEggIcon.enabled = false;
            return;
        }

        equippedEggIcon.sprite = EggVisualDatabase.Instance.GetVisualInfo(eggId).Sprite;

        equippedEggIcon.enabled = true;

        // TODO: ItemManager instance-based lookup for the actual sprite
        // Sprite sprite = ItemManager.Instance.GetItemById(eggId).icon;
        // equippedEggIcon.sprite = sprite;
    }
}