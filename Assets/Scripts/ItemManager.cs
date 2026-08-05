using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    private List<Item> allItems = new();
    public List<Item> AllItems => allItems;
    public bool IsLoaded { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadAllItems(System.Action onComplete)
    {
        NetworkManager.Instance.GetAllItems((success, json) =>
        {
            if (success)
            {
                ItemListResponse response = JsonUtility.FromJson<ItemListResponse>(json);
                allItems = new List<Item>(response.list);
                IsLoaded = true;
                Debug.Log("아이템 " + allItems.Count + "개 로드 완료");
            }
            else
            {
                Debug.LogWarning("아이템 로드 실패");
            }

            onComplete?.Invoke();
        });
    }

    public int[] GetItemIdsByTier(int tier)
    {
        List<int> result = new List<int>();
        if (tier == 0)
        {
            result.Add(0);
            return result.ToArray();
        }

        foreach (var item in allItems)
        {
            if ((int)item.Tier == tier)
            {
                result.Add(item.ID);
            }
        }
        return result.ToArray();
    }

    public Item GetItemById(int id)
    {
        return allItems.Find(item => item.ID == id);
    }
}