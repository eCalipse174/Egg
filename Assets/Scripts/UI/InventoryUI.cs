using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform slotContainer;
    public GameObject slotPrefab;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    public void OpenInventory(int capacity)
    {
        BuildSlots(capacity);
        RefreshSlots();
        gameObject.SetActive(true);
    }

    private void BuildSlots(int capacity)
    {
        if (slotUIs.Count == capacity) return; // 이미 만들어져 있으면 재사용

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        slotUIs.Clear();

        for (int i = 0; i < capacity; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            slotUIs.Add(slotObj.GetComponent<InventorySlotUI>());
        }
    }

    private void RefreshSlots()
    {
        List<UserItems> data = InventoryManager.Instance.GetAllSlots();

        for (int i = 0; i < slotUIs.Count; i++)
        {
            UserItems found = data.Find(slot => slot.slot_index == i);

            if (found != null)
            {
                Item itemInfo = ItemManager.Instance.GetItemById(found.item_id);
                Sprite icon = LoadIconForItem(itemInfo); // 아이콘 불러오는 방식은 직접 구현
                slotUIs[i].SetItem(found, icon);
            }
            else
            {
                slotUIs[i].SetEmpty();
            }
        }
    }

    private Sprite LoadIconForItem(Item itemInfo)
    {
        // Resources.Load, Addressables, 또는 딕셔너리 방식 중 원하는 걸로 구현
        return null;
    }
}