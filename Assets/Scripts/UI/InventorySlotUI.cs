using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    private UserItems slotData;

    public void SetEmpty()
    {
        slotData = null;
        iconImage.enabled = false;
    }

    public void SetItem(UserItems data, Sprite iconSprite)
    {
        slotData = data;
        iconImage.enabled = true;
        iconImage.sprite = iconSprite;
    }

    public void OnSlotClicked()
    {
        if (slotData == null) return;
        Debug.Log("클릭한 슬롯의 item_id: " + slotData.item_id);
        // 여기서 상세정보 팝업 등 원하는 동작 연결
    }
}