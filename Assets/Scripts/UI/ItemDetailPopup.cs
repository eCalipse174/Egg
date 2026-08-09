using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPopup : MonoBehaviour
{
    public static ItemDetailPopup Instance;

    [SerializeField] private GameObject root;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text subtitleText;
    [SerializeField] private Text tierText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text obtainedAtText;
    [SerializeField] private Text priceText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button lockButton;
    [SerializeField] private Text lockButtonText;

    private UserItems currentUserItem;
    private Item currentItemInfo;
    private EggVisualInfo currentVisualInfo;
    private Tier currentItemTier;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnItemSold += HandleItemSold;
        InventoryManager.Instance.OnLockToggled += HandleLockToggled;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnItemSold -= HandleItemSold;
        InventoryManager.Instance.OnLockToggled -= HandleLockToggled;
    }

    public void Open(UserItems userItem, Item itemInfo, Sprite icon)
    {
        if (itemInfo == currentItemInfo)
            return;

        currentUserItem = userItem;
        currentItemInfo = itemInfo;
        currentVisualInfo = EggVisualDatabase.Instance.GetVisualInfo(itemInfo.ID);
        currentItemTier = EggVisualDatabase.Instance.TierList.list[(int)currentItemInfo.Tier];
        root.SetActive(true);

        iconImage.sprite = currentVisualInfo.Sprite;
        nameText.text = itemInfo.Egg_Name;
        nameText.font = currentVisualInfo.Font;
        nameText.color = currentVisualInfo.NameColor;
        subtitleText.text = itemInfo.Egg_SubTitle;
        subtitleText.font = currentVisualInfo.Font;
        subtitleText.color = currentVisualInfo.NameColor;
        tierText.text = currentItemTier.name;
        tierText.color = currentItemTier.color;
        descriptionText.text = currentItemInfo.Egg_Desc;
        obtainedAtText.text = userItem.obtained_at;
        priceText.text = "ÆÇ¸Å(" + itemInfo.price.ToString() + "G)";

        RefreshLockState(userItem.is_locked);
    }

    public void Close()
    {
        currentUserItem = null;
        currentItemInfo = null;
        root.SetActive(false);
    }

    private void RefreshLockState(bool isLocked)
    {
        lockButtonText.text = isLocked ? "Unlock" : "Lock";
        sellButton.interactable = !isLocked;
    }

    public void OnSellButtonClicked()
    {
        if (currentUserItem == null)
        {
            return;
        }
        InventoryManager.Instance.SellItem(currentUserItem.slot_index, success => { });
    }

    public void OnLockButtonClicked()
    {
        if (currentUserItem == null)
        {
            return;
        }
        InventoryManager.Instance.ToggleLock(currentUserItem.slot_index, success => { });
    }

    private void HandleItemSold(int slotIndex)
    {
        if (currentUserItem != null && currentUserItem.slot_index == slotIndex)
        {
            Close();
        }
    }

    private void HandleLockToggled(int slotIndex, bool isLocked)
    {
        if (currentUserItem != null && currentUserItem.slot_index == slotIndex)
        {
            RefreshLockState(isLocked);
        }
    }
}