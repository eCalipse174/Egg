using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance;

    private List<UserCollections> unlocked = new List<UserCollections>();

    public event Action<int> OnCollectionUpdated;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadCollection(int userId, Action onComplete)
    {
        NetworkManager.Instance.GetCollection(userId, (success, json) =>
        {
            if (success)
            {
                UserCollectionListResponse response = JsonUtility.FromJson<UserCollectionListResponse>(json);
                unlocked = new List<UserCollections>(response.list);
            }
            else
            {
                Debug.LogWarning("collection load failed");
            }
            onComplete?.Invoke();
        });
    }

    public bool IsUnlocked(int itemId)
    {
        return unlocked.Exists(x => x.item_id == itemId);
    }

    public string GetUnlockedAt(int itemId)
    {
        UserCollections found = unlocked.Find(x => x.item_id == itemId);
        return found != null ? found.unlocked_at : "";
    }

    public void UnlockItem(int itemId, Action<bool> onComplete)
    {
        if (IsUnlocked(itemId))
        {
            onComplete?.Invoke(true);
            return;
        }

        int userId = GameManager.Instance.UserInfo.id;

        NetworkManager.Instance.UnlockCollection(userId, itemId, (success, json) =>
        {
            if (success)
            {
                // server response only contains id, reconstruct the rest locally
                IdOnlyResponse idResponse = JsonUtility.FromJson<IdOnlyResponse>(json);
                UserCollections newEntry = new UserCollections();
                newEntry.id = idResponse.id;
                newEntry.user_id = userId;
                newEntry.item_id = itemId;
                newEntry.unlocked_at = DateTime.Now.ToString();

                unlocked.Add(newEntry);
                OnCollectionUpdated?.Invoke(itemId);
            }
            else
            {
                Debug.LogWarning("collection unlock failed");
            }
            onComplete?.Invoke(success);
        });
    }
}