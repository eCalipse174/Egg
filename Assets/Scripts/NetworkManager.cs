using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    private const string baseUrl = "http://localhost:8080/api/v2/tables";
    private const string apiToken = "nc_pat_VN62s0FBTsK7IS1a9NrJYtoQKT6QXDaCNQafu7";

    // 테이블 ID 모음 (API Docs에서 확인한 값으로 교체)
    private const string usersTableId = "m5z2oasma8pp4h7";
    private const string itemsTableId = "mr0i55hwgi0lxmp";
    private const string userItemsTableId = "mtx40ii7dcjizqd";
    private const string userCollectionsTableId = "ma7ihxjcxgh66h1";

    private void Awake()
    {
        Instance = this;
    }

    // ---------- 공통 요청 처리 (private) ----------

    private IEnumerator SendRequest(string tableId, string method, string query, string jsonBody, Action<bool, string> callback)
    {
        string url = baseUrl + "/" + tableId + "/records";
        if (!string.IsNullOrEmpty(query))
        {
            url += "?" + query;
        }

        UnityWebRequest request;

        if (method == "GET")
        {
            request = UnityWebRequest.Get(url);
        }
        else
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody ?? "");
            request = new UnityWebRequest(url, method);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
        }

        request.SetRequestHeader("xc-token", apiToken);

        yield return request.SendWebRequest();

        bool success = request.result == UnityWebRequest.Result.Success;
        callback(success, request.downloadHandler.text);
    }

    private void Get(string tableId, string query, Action<bool, string> callback)
    {
        StartCoroutine(SendRequest(tableId, "GET", query, null, callback));
    }

    private void Post(string tableId, string json, Action<bool, string> callback)
    {
        StartCoroutine(SendRequest(tableId, "POST", null, json, callback));
    }

    private void Patch(string tableId, string json, Action<bool, string> callback)
    {
        StartCoroutine(SendRequest(tableId, "PATCH", null, json, callback));
    }

    private void Delete(string tableId, string json, Action<bool, string> callback)
    {
        StartCoroutine(SendRequest(tableId, "DELETE", null, json, callback));
    }

    // ---------- users ----------

    public void GetUser(string deviceId, Action<bool, string> callback)
    {
        Get(usersTableId, "where=(device_id,eq," + deviceId + ")", callback);
    }

    public void CreateUser(string deviceId, string nickname, Action<bool, string> callback)
    {
        string json = "{\"device_id\":\"" + deviceId + "\",\"nickname\":\"" + nickname + "\"}";
        Post(usersTableId, json, callback);
    }

    public void UpdateGold(int userId, int newGold, Action<bool, string> callback)
    {
        string json = "{\"Id\":" + userId + ",\"gold\":" + newGold + "}";
        Patch(usersTableId, json, callback);
    }

    public void UpdateEnhanceLevel(int userId, int newLevel, Action<bool, string> callback)
    {
        string json = "{\"Id\":" + userId + ",\"enhance_level\":" + newLevel + "}";
        Patch(usersTableId, json, callback);
    }

    public void IncreaseGachaCount(int userId, int newCount, Action<bool, string> callback)
    {
        string json = "{\"Id\":" + userId + ",\"gacha_count\":" + newCount + "}";
        Patch(usersTableId, json, callback);
    }

    public void UpdatePlayTime(int userId, int newSeconds, Action<bool, string> callback)
    {
        string json = "{\"Id\":" + userId + ",\"play_time_seconds\":" + newSeconds + "}";
        Patch(usersTableId, json, callback);
    }

    public void GetRanking(Action<bool, string> callback)
    {
        Get(usersTableId, "sort=-gold&limit=50", callback);
    }

    // ---------- user_items (inventory) ----------

    public void GetInventory(int userId, Action<bool, string> callback)
    {
        Get(userItemsTableId, "where=(user_id,eq," + userId + ")", callback);
    }

    public void AddItemToSlot(int userId, int itemId, int slotIndex, Action<bool, string> callback)
    {
        string json = "{\"user_id\":" + userId + ",\"item_id\":" + itemId + ",\"slot_index\":" + slotIndex + "}";
        Post(userItemsTableId, json, callback);
    }

    public void RemoveItemFromInventory(int userItemRowId, Action<bool, string> callback)
    {
        string json = "{\"Id\":" + userItemRowId + "}";
        Delete(userItemsTableId, json, callback);
    }

    // ---------- user_collections (dex) ----------

    public void GetCollection(int userId, Action<bool, string> callback)
    {
        Get(userCollectionsTableId, "where=(user_id,eq," + userId + ")", callback);
    }

    public void UnlockCollection(int userId, int itemId, Action<bool, string> callback)
    {
        string json = "{\"user_id\":" + userId + ",\"item_id\":" + itemId + "}";
        Post(userCollectionsTableId, json, callback);
    }

    // ---------- items (master data) ----------

    public void GetAllItems(Action<bool, string> callback)
    {
        Get(itemsTableId, "limit=1000", callback);
    }
}