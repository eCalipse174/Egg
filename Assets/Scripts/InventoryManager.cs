using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<UserItems> slots = new List<UserItems>();
    private int capacity;

    private void Awake()
    {
        Instance = this;
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
                Debug.Log("인벤토리 로드 완료, 슬롯 " + slots.Count + "개 사용 중");
            }
            else
            {
                Debug.LogWarning("인벤토리 로드 실패");
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
        return -1; // 빈 슬롯 없음
    }

    public void AddItem(int userId, int itemId, System.Action<bool> onComplete)
    {
        int emptySlot = FindEmptySlotIndex();

        if (emptySlot == -1)
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다");
            onComplete?.Invoke(false);
            return;
        }

        NetworkManager.Instance.AddItemToSlot(userId, itemId, emptySlot, (success, json) =>
        {
            if (success)
            {
                // 서버 응답(id만 있음)에서 id만 꺼내고, 나머지는 이미 알고 있는 값으로 직접 구성
                IdOnlyResponse idResponse = JsonUtility.FromJson<IdOnlyResponse>(json);

                UserItems newSlot = new UserItems();
                newSlot.id = idResponse.id;
                newSlot.user_id = userId;
                newSlot.item_id = itemId;
                newSlot.slot_index = emptySlot;

                slots.Add(newSlot);
                //Debug.Log("슬롯 " + emptySlot + "에 아이템 " + itemId + " 추가됨, 서버 id: " + newSlot.id);
            }
            else
            {
                Debug.LogWarning("아이템 추가 실패");
            }

            onComplete?.Invoke(success);
        });
    }

    public void RemoveItem(int slotIndex, System.Action<bool> onComplete)
    {
        UserItems target = slots.Find(slot => slot.slot_index == slotIndex);

        if (target == null)
        {
            Debug.LogWarning("해당 슬롯에 아이템이 없습니다");
            onComplete?.Invoke(false);
            return;
        }

        NetworkManager.Instance.RemoveItemFromInventory(target.id, (success, json) =>
        {
            if (success)
            {
                slots.Remove(target);
                Debug.Log("슬롯 " + slotIndex + " 아이템 제거됨");
            }
            else
            {
                Debug.LogWarning("아이템 제거 실패");
            }

            onComplete?.Invoke(success);
        });
    }

    public List<UserItems> GetAllSlots()
    {
        return slots;
    }
}