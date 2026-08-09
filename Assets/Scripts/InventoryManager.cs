using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<UserItems> slots = new List<UserItems>();
    private int capacity;

    public const int MaxCapacity = 40;
    public const int SlotUnlockCost = 10000;

    public event Action<int> OnItemSold;
    public event Action<int, bool> OnLockToggled;
    public event Action<int> OnItemAdded;
    public event Action<int> OnCapacityChanged;

    public int Capacity => capacity;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadInventory(int userId, int inventoryCapacity, System.Action onComplete)
    {
        capacity = inventoryCapacity;
        NetworkManager.Instance.GetInventory(userId, (success, json) =>
        {
            if (success)
            {
                UserItemListResponse response = JsonUtility.FromJson<UserItemListResponse>(json);
                slots = new List<UserItems>(response.list);
                Debug.Log("inventory load complete, slots used: " + slots.Count);
            }
            else
            {
                Debug.LogWarning("inventory load failed");
            }
            onComplete?.Invoke();
        });
    }

    private int FindEmptySlotIndex()
    {
        for (int i = 0; i < capacity; i++)
        {
            bool occupied = slots.Exists(slot => slot.slot_index == i);
            if (!occupied)
            {
                return i;
            }
        }
        return -1; // no empty slot
    }

    public void AddItem(int userId, int itemId, System.Action<bool> onComplete)
    {
        int emptySlot = FindEmptySlotIndex();
        if (emptySlot == -1)
        {
            Debug.LogWarning("inventory is full");
            onComplete?.Invoke(false);
            return;
        }
        NetworkManager.Instance.AddItemToSlot(userId, itemId, emptySlot, (success, json) =>
        {
            if (success)
            {
                IdOnlyResponse idResponse = JsonUtility.FromJson<IdOnlyResponse>(json);
                UserItems newSlot = new UserItems();
                newSlot.id = idResponse.id;
                newSlot.user_id = userId;
                newSlot.item_id = itemId;
                newSlot.slot_index = emptySlot;
                slots.Add(newSlot);
                OnItemAdded?.Invoke(emptySlot);
            }
            else
            {
                Debug.LogWarning("add item failed");
            }
            onComplete?.Invoke(success);
        });
    }

    public void RemoveItem(int slotIndex, System.Action<bool> onComplete)
    {
        UserItems target = slots.Find(slot => slot.slot_index == slotIndex);
        if (target == null)
        {
            Debug.LogWarning("no item in this slot");
            onComplete?.Invoke(false);
            return;
        }
        NetworkManager.Instance.RemoveItemFromInventory(target.id, (success, json) =>
        {
            if (success)
            {
                slots.Remove(target);
                Debug.Log("slot " + slotIndex + " item removed");
            }
            else
            {
                Debug.LogWarning("remove item failed");
            }
            onComplete?.Invoke(success);
        });
    }

    public void SellItem(int slotIndex, System.Action<bool> onComplete)
    {
        UserItems target = slots.Find(slot => slot.slot_index == slotIndex);
        if (target == null)
        {
            onComplete?.Invoke(false);
            return;
        }
        if (target.is_locked)
        {
            onComplete?.Invoke(false);
            return;
        }

        Item itemInfo = ItemManager.Instance.GetItemById(target.item_id);
        if (itemInfo == null)
        {
            onComplete?.Invoke(false);
            return;
        }

        NetworkManager.Instance.RemoveItemFromInventory(target.id, (success, json) =>
        {
            if (!success)
            {
                Debug.LogWarning("sell item failed");
                onComplete?.Invoke(false);
                return;
            }

            slots.Remove(target);

            long newGold = GameManager.Instance.UserInfo.gold + itemInfo.price;
            NetworkManager.Instance.UpdateGold(GameManager.Instance.UserInfo.id, newGold, (goldSuccess, goldJson) =>
            {
                if (goldSuccess)
                {
                    GameManager.Instance.UserInfo.gold = newGold;
                }
                else
                {
                    Debug.LogWarning("gold update failed after sell");
                }

                OnItemSold?.Invoke(slotIndex);
            });

            onComplete?.Invoke(true);
        });
    }

    public void ToggleLock(int slotIndex, System.Action<bool> onComplete)
    {
        UserItems target = slots.Find(slot => slot.slot_index == slotIndex);
        if (target == null)
        {
            onComplete?.Invoke(false);
            return;
        }

        bool newState = !target.is_locked;

        NetworkManager.Instance.UpdateItemLock(target.id, newState, (success, json) =>
        {
            if (success)
            {
                target.is_locked = newState;
                OnLockToggled?.Invoke(slotIndex, newState);
            }
            else
            {
                Debug.LogWarning("lock toggle failed");
            }
            onComplete?.Invoke(success);
        });
    }

    public List<UserItems> GetAllSlots()
    {
        return slots;
    }

    public bool IsInventoryFull()
    {
        return FindEmptySlotIndex() == -1;
    }

    public void UnlockNextSlot(System.Action<bool> onComplete)
    {
        if (capacity >= MaxCapacity)
        {
            onComplete?.Invoke(false);
            return;
        }

        long currentGold = GameManager.Instance.UserInfo.gold;
        if (currentGold < SlotUnlockCost)
        {
            onComplete?.Invoke(false);
            return;
        }

        long newGold = currentGold - SlotUnlockCost;
        int newCapacity = capacity + 1;

        NetworkManager.Instance.UpdateGold(GameManager.Instance.UserInfo.id, newGold, (goldSuccess, goldJson) =>
        {
            if (!goldSuccess)
            {
                Debug.LogWarning("slot unlock gold update failed");
                onComplete?.Invoke(false);
                return;
            }

            GameManager.Instance.UserInfo.gold = newGold;

            NetworkManager.Instance.UpdateInventoryCapacity(GameManager.Instance.UserInfo.id, newCapacity, (capSuccess, capJson) =>
            {
                if (capSuccess)
                {
                    capacity = newCapacity;
                    GameManager.Instance.UserInfo.inventory_capacity = newCapacity;
                    OnCapacityChanged?.Invoke(newCapacity);
                }
                else
                {
                    Debug.LogWarning("inventory capacity update failed");
                }
                onComplete?.Invoke(capSuccess);
            });
        });
    }
}