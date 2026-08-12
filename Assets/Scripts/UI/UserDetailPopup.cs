using UnityEngine;
using UnityEngine.UI;

public class UserDetailPopup : MonoBehaviour
{
    private static UserDetailPopup instance;
    public static UserDetailPopup Instance => instance;

    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Image equippedEggIcon;
    [SerializeField] private Text nicknameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text playTimeText;
    [SerializeField] private Text gachaCountText;
    [SerializeField] private Text createdAtText;

    private const int noEggEquipped = -1;

    private void Awake()
    {
        instance = this;
        Close();
    }

    public void Open(RankingManager.RankingEntry entry)
    {
        popupRoot.SetActive(true);

        nicknameText.text = entry.nickname;
        levelText.text = "Lv." + entry.enhance_level;
        playTimeText.text = FormatPlayTime(entry.play_time_seconds);
        gachaCountText.text = entry.gacha_count.ToString();
        createdAtText.text = entry.created_at;

        SetEggIcon(entry.equipped_egg_id);
    }

    public void Close()
    {
        popupRoot.SetActive(false);
    }

    private string FormatPlayTime(int seconds)
    {
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        return hours + "½Ã°£ " + minutes + "ºÐ";
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