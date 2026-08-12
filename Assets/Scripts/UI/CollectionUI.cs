using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionUI : MonoBehaviour
{
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject listEntryPrefab;
    [SerializeField] private Sprite lockedIcon;

    [SerializeField] private Image detailIconImage;
    [SerializeField] private Text detailNameText;
    [SerializeField] private Text detailSubtitleText;
    [SerializeField] private Text detailTierText;
    [SerializeField] private Text detailDescriptionText;
    [SerializeField] private Text detailUnlockedAtText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Text equipButtonText;

    [Space]
    [SerializeField] private Font defaultFont;

    private List<CollectionListEntryUI> entries = new List<CollectionListEntryUI>();

    private Item currentItem;

    private void OnEnable()
    {
        equipButton.onClick.AddListener(OnEquipButtonClicked);
    }

    private void OnDisable()
    {
        equipButton.onClick.RemoveListener(OnEquipButtonClicked);
    }

    public void Open()
    {
        BuildList();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void BuildList()
    {
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }
        entries.Clear();

        List<Item> sortedItems = new(ItemManager.Instance.AllItems);
        sortedItems.Sort((a, b) =>
        {
            int tierCompare = ((int)a.Tier).CompareTo((int)b.Tier);
            if (tierCompare != 0)
            {
                return tierCompare;
            }

            int weightCompare = b.Weight.CompareTo(a.Weight);
            if (weightCompare != 0)
            {
                return weightCompare;
            }

            return a.ID.CompareTo(b.ID);
        });

        foreach (Item item in sortedItems)
        {
            GameObject entryObj = Instantiate(listEntryPrefab, listContainer);
            CollectionListEntryUI entry = entryObj.GetComponent<CollectionListEntryUI>();

            bool unlocked = CollectionManager.Instance.IsUnlocked(item.ID);
            Sprite icon = unlocked ? GetIconForItem(item) : lockedIcon;

            entry.Setup(icon, () => ShowDetail(item));
            entries.Add(entry);
        }

        if (sortedItems.Count > 0)
        {
            ShowDetail(sortedItems[0]);
        }
    }

    private void ShowDetail(Item item)
    {
        currentItem = item;
        bool unlocked = CollectionManager.Instance.IsUnlocked(item.ID);
        var visualInfo = EggVisualDatabase.Instance.GetVisualInfo(item.ID);

        if (!unlocked)
        {
            detailIconImage.sprite = lockedIcon;
            detailNameText.text = "???";
            detailNameText.font = defaultFont;
            detailNameText.color = visualInfo.NameColor;
            detailSubtitleText.text = "???";
            detailSubtitleText.font = defaultFont;
            detailSubtitleText.color = visualInfo.NameColor;
            detailTierText.text = "???";
            detailTierText.color = EggVisualDatabase.Instance.TierList.list[(int)item.Tier].color;
            detailDescriptionText.text = "???";
            detailUnlockedAtText.text = "";
            equipButton.gameObject.SetActive(false);
            return;
        }

        detailIconImage.sprite = GetIconForItem(item);
        detailNameText.text = item.Egg_Name;
        detailNameText.color = visualInfo.NameColor;
        detailNameText.font = visualInfo.Font;
        detailSubtitleText.text = item.Egg_SubTitle;
        detailSubtitleText.font = visualInfo.Font;
        detailSubtitleText.color = visualInfo.NameColor;
        detailTierText.text = EggVisualDatabase.Instance.TierList.list[(int)item.Tier].name;
        detailTierText.color = EggVisualDatabase.Instance.TierList.list[(int)item.Tier].color;
        detailDescriptionText.text = item.Egg_Desc;
        detailUnlockedAtText.text = CollectionManager.Instance.GetUnlockedAt(item.ID);

        equipButton.gameObject.SetActive(true);
        UpdateEquipButtonState();
    }

    private void UpdateEquipButtonState()
    {
        bool isEquipped = GameManager.Instance.UserInfo.equipped_egg_id == currentItem.ID;
        equipButton.interactable = !isEquipped;
        equipButtonText.text = isEquipped ? "전시중" : "전시하기";
    }

    private void OnEquipButtonClicked()
    {
        GameManager.Instance.SetEquippedEgg(currentItem.ID);
        UpdateEquipButtonState();
    }

    private Sprite GetIconForItem(Item item)
    {
        EggVisualInfo visualInfo = EggVisualDatabase.Instance.GetVisualInfo(item.ID);
        return visualInfo != null ? visualInfo.Sprite : null;
    }
}