using System;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    private static RankingManager instance;
    public static RankingManager Instance => instance;

    private const int rankingLimit = 10;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum RankingType
    {
        GachaCount,
        Gold,
        PlayTime
    }

    [Serializable]
    private class RawEntry
    {
        public int id;
        public string nickname;
        public int enhance_level;
        public int equipped_egg_id;
        public long gold;
        public int gacha_count;
        public int play_time_seconds;
        public string created_at;
    }

    [Serializable]
    private class RawRankingResponse
    {
        public RawEntry[] list;
    }

    public class RankingEntry
    {
        public int id;
        public string nickname;
        public int enhance_level;
        public int equipped_egg_id;
        public long gold;
        public int gacha_count;
        public int play_time_seconds;
        public string created_at;
        public long value;
    }

    // ---------- public API ----------

    public void LoadRanking(RankingType type, Action<List<RankingEntry>> onComplete)
    {
        string column = GetColumnName(type);

        NetworkManager.Instance.GetRankingByColumn(column, rankingLimit, (success, json) =>
        {
            if (!success)
            {
                Debug.LogWarning("랭킹 로드 실패: " + json);
                onComplete(new List<RankingEntry>());
                return;
            }

            onComplete(ParseRanking(json, type));
        });
    }

    // ---------- private implementation ----------

    private string GetColumnName(RankingType type)
    {
        switch (type)
        {
            case RankingType.GachaCount:
                return "gacha_count";
            case RankingType.Gold:
                return "gold";
            case RankingType.PlayTime:
                return "play_time_seconds";
        }

        return "gold";
    }

    private List<RankingEntry> ParseRanking(string json, RankingType type)
    {
        List<RankingEntry> result = new List<RankingEntry>();

        RawRankingResponse response = JsonUtility.FromJson<RawRankingResponse>(json);
        if (response == null || response.list == null)
            return result;

        foreach (RawEntry raw in response.list)
        {
            RankingEntry entry = new RankingEntry();
            entry.id = raw.id;
            entry.nickname = raw.nickname;
            entry.enhance_level = raw.enhance_level;
            entry.equipped_egg_id = raw.equipped_egg_id;
            entry.gold = raw.gold;
            entry.gacha_count = raw.gacha_count;
            entry.play_time_seconds = raw.play_time_seconds;
            entry.created_at = raw.created_at;
            entry.value = GetValue(raw, type);
            result.Add(entry);
        }

        return result;
    }

    private long GetValue(RawEntry raw, RankingType type)
    {
        switch (type)
        {
            case RankingType.GachaCount:
                return raw.gacha_count;
            case RankingType.Gold:
                return raw.gold;
            case RankingType.PlayTime:
                return raw.play_time_seconds;
        }

        return 0;
    }
}