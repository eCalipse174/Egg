using UnityEngine;
using UnityEngine.UI;

public class RankingEntryView : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text nicknameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text valueText;
    [SerializeField] private Image equippedEggIcon;

    private const int noEggEquipped = -1;

    public void Setup(int rank, RankingManager.RankingEntry entry)
    {
        rankText.text = rank.ToString();
        nicknameText.text = entry.nickname;
        levelText.text = "Lv." + entry.enhance_level;
        valueText.text = entry.value.ToString();

        SetEggIcon(entry.equipped_egg_id);
    }

    private void SetEggIcon(int eggId)
    {
        if (eggId == noEggEquipped)
        {
            equippedEggIcon.enabled = false;
            return;
        }

        equippedEggIcon.enabled = true;

        // TODO: ItemManager instance-based lookup for the actual sprite
        // Sprite sprite = ItemManager.Instance.GetItemById(eggId).icon;
        // equippedEggIcon.sprite = sprite;
    }
}