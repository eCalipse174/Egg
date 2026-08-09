using System.Collections.Generic;
using UnityEngine;
public class InventoryUI : MonoBehaviour
{
    private static InventoryUI instance;

    public static InventoryUI Instance => instance;
    public Transform slotContainer;
    public GameObject slotPrefab;
    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnItemSold += HandleInventoryChanged;
        InventoryManager.Instance.OnLockToggled += HandleLockToggled;
        InventoryManager.Instance.OnItemAdded += HandleInventoryChanged;
        InventoryManager.Instance.OnCapacityChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnItemSold -= HandleInventoryChanged;
        InventoryManager.Instance.OnLockToggled -= HandleLockToggled;
        InventoryManager.Instance.OnItemAdded -= HandleInventoryChanged;
        InventoryManager.Instance.OnCapacityChanged -= HandleInventoryChanged;
    }

    private void HandleInventoryChanged(int slotIndex)
    {
        RefreshSlots();
    }

    private void HandleLockToggled(int slotIndex, bool isLocked)
    {
        RefreshSlots();
    }

    public void OpenInventory()
    {
        RefreshSlots();
        gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    private void BuildSlots(int totalSlots)
    {
        if (slotUIs.Count == totalSlots) return;
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        slotUIs.Clear();
        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            slotUIs.Add(slotObj.GetComponent<InventorySlotUI>());
        }
    }

    public void RefreshSlots()
    {
        List<UserItems> data = InventoryManager.Instance.GetAllSlots();
        int unlockedCapacity = InventoryManager.Instance.Capacity;

        bool hasNextSlot = unlockedCapacity < InventoryManager.MaxCapacity;
        int totalToShow = hasNextSlot ? unlockedCapacity + 1 : unlockedCapacity;

        BuildSlots(totalToShow);

        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < unlockedCapacity)
            {
                UserItems found = data.Find(slot => slot.slot_index == i);
                if (found != null)
                {
                    Item itemInfo = ItemManager.Instance.GetItemById(found.item_id);
                    Sprite icon = LoadIconForItem(itemInfo);
                    slotUIs[i].SetItem(found, icon);
                }
                else
                {
                    slotUIs[i].SetEmpty();
                }
            }
            else
            {
                slotUIs[i].SetLocked(true, InventoryManager.SlotUnlockCost);
            }
        }
    }

    private Sprite LoadIconForItem(Item itemInfo)
    {
        if (itemInfo == null) return null;
        EggVisualInfo visualInfo = EggVisualDatabase.Instance.GetVisualInfo(itemInfo.ID);
        return visualInfo != null ? visualInfo.Sprite : null;
    }
}