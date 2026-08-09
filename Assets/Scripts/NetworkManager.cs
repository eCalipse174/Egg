using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    private const string baseUrl = "http://localhost:8080/api/v2/tables";
    private const string apiToken = "nc_pat_CWvLZe88DC0aafWZSDPRefp6hgjgnK2lnwkvVTVf";

    // 테이블 ID 모음 (API Docs에서 확인한 값으로 교체)
    private const string usersTableId = "modzj7grug18qlj";
    private const string itemsTableId = "matr6tfj332ouoc";
    private const string userItemsTableId = "mctr16amxbs8ay6";
    private const string userCollectionsTableId = "mhvz39j84x8jld7";

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- 공통 요청 처리 (private) ----------

    private IEnumerator SendRequest(string tableid, string method, string query, string jsonBody, Action<bool, string> callback)
    {
        string url = baseUrl + "/" + tableid + "/records";
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

        Debug.Log("응답 코드: " + request.responseCode);
        Debug.Log(request.error);
        //Debug.Log(request.downloadHandler.text);

        bool success = request.result == UnityWebRequest.Result.Success;
        callback(success, request.downloadHandler.text);
    }

    private void Get(string tableid, string query, Action<bool, string> callback)
    {
        StartCoroutine(SendRequest(tableid, "GET", query, null, callback));
    }

    private void Post(string tableid, string json, Action<bool, string> callback)
    {
        StartCoroutine(SendRequest(tableid, "POST", null, json, callback));
    }

    private void Patch(string tableid, string json, Action<bool, string> callback)
    {
        string wrappedJson = "[" + json + "]";
        StartCoroutine(SendRequest(tableid, "PATCH", null, wrappedJson, callback));
    }

    private void Delete(string tableid, string json, Action<bool, string> callback)
    {
        StartCoroutine(SendRequest(tableid, "DELETE", null, json, callback));
    }

    // ---------- users ----------

    public void GetUser(string deviceid, Action<bool, string> callback)
    {
        Get(usersTableId, "where=(device_id,eq," + deviceid + ")", callback);
    }

    public void CreateUser(string deviceid, string nickname, Action<bool, string> callback)
    {
        string json = "{\"device_id\":\"" + deviceid + "\",\"nickname\":\"" + nickname + "\"}";
        Post(usersTableId, json, callback);
    }

    public void UpdateGold(int userid, int newGold, Action<bool, string> callback)
    {
        string json = "{\"id\":" + userid + ",\"gold\":" + newGold + "}";
        Patch(usersTableId, json, callback);
    }

    public void UpdateEnhanceLevel(int userid, int newLevel, Action<bool, string> callback)
    {
        string json = "{\"id\":" + userid + ",\"enhance_level\":" + newLevel + "}";
        Patch(usersTableId, json, callback);
    }

    public void IncreaseGachaCount(int userid, int newCount, Action<bool, string> callback)
    {
        string json = "{\"id\":" + userid + ",\"gacha_count\":" + newCount + "}";
        Patch(usersTableId, json, callback);
    }

    public void UpdatePlayTime(int userid, int newSeconds, Action<bool, string> callback)
    {
        string json = "{\"id\":" + userid + ",\"play_time_seconds\":" + newSeconds + "}";
        Patch(usersTableId, json, callback);
    }

    public void GetRanking(Action<bool, string> callback)
    {
        Get(usersTableId, "sort=-gold&limit=50", callback);
    }

    // ---------- user_items (inventory) ----------

    public void GetInventory(int userid, Action<bool, string> callback)
    {
        Get(userItemsTableId, "where=(user_id,eq," + userid + ")", callback);
    }

    public void AddItemToSlot(int userid, int itemid, int slotIndex, Action<bool, string> callback)
    {
        string json = "{\"user_id\":" + userid + ",\"item_id\":" + itemid + ",\"slot_index\":" + slotIndex + "}";
        Post(userItemsTableId, json, callback);
    }

    public void RemoveItemFromInventory(int userItemRowid, Action<bool, string> callback)
    {
        string json = "{\"id\":" + userItemRowid + "}";
        Delete(userItemsTableId, json, callback);
    }

    public void UpdateItemLock(int userItemRowid, bool isLocked, Action<bool, string> callback)
    {
        string json = "{\"id\":" + userItemRowid + ",\"is_locked\":" + (isLocked ? "1" : "0") + "}";
        Patch(userItemsTableId, json, callback);
    }

    // ---------- user_collections (dex) ----------

    public void GetCollection(int userid, Action<bool, string> callback)
    {
        Get(userCollectionsTableId, "where=(user_id,eq," + userid + ")", callback);
    }

    public void UnlockCollection(int userid, int itemid, Action<bool, string> callback)
    {
        string json = "{\"user_id\":" + userid + ",\"item_id\":" + itemid + "}";
        Post(userCollectionsTableId, json, callback);
    }

    // ---------- items (master data) ----------

    public void GetAllItems(Action<bool, string> callback)
    {
        Get(itemsTableId, "limit=1000", callback);
    }


    public void SaveUserProgress(int userid, int gold, int enhanceLevel, int gachaCount, int playTimeSeconds, Action<bool, string> callback)
    {
        string json = "{\"id\":" + userid
            + ",\"gold\":" + gold
            + ",\"enhance_level\":" + enhanceLevel
            + ",\"gacha_count\":" + gachaCount
            + ",\"play_time_seconds\":" + playTimeSeconds
            + "}";

        Patch(usersTableId, json, callback);

        Debug.Log("종료 시점 데이터 저장 완료");
    }
}