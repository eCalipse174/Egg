using UnityEngine;
using UnityEngine.UI;
public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public GameObject lockedIcon;
    public Text priceText;

    private UserItems slotData;
    private bool isLocked;
    private bool isNextUnlockable;

    public void SetEmpty()
    {
        slotData = null;
        isLocked = false;
        iconImage.enabled = false;
        if (lockedIcon != null) lockedIcon.SetActive(false);
        if (priceText != null) priceText.gameObject.SetActive(false);
    }

    public void SetItem(UserItems data, Sprite iconSprite)
    {
        slotData = data;
        isLocked = false;
        iconImage.enabled = true;
        iconImage.sprite = iconSprite;
        if (lockedIcon != null) lockedIcon.SetActive(false);
        if (priceText != null) priceText.gameObject.SetActive(false);
    }

    public void SetLocked(bool isNext, int cost)
    {
        slotData = null;
        isLocked = true;
        isNextUnlockable = isNext;
        iconImage.enabled = false;
        if (lockedIcon != null) lockedIcon.SetActive(true);
        if (priceText != null)
        {
            priceText.gameObject.SetActive(isNext);
            priceText.text = cost.ToString() + "G";
        }
    }

    public void OnSlotClicked()
    {
        if (isLocked)
        {
            if (!isNextUnlockable) return;
            InventoryManager.Instance.UnlockNextSlot(success => { });
            return;
        }

        if (slotData == null) return;
        Debug.Log("clicked slot item_id: " + slotData.item_id);

        UserItems userItem = InventoryManager.Instance.GetAllSlots().Find(slot => slot.slot_index == slotData.slot_index);
        Item itemInfo = ItemManager.Instance.GetItemById(userItem.item_id);
        ItemDetailPopup.Instance.Open(userItem, itemInfo, this.iconImage.sprite);
    }
}