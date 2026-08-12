using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private RankingEntryView[] entrySlots;

    [SerializeField] private Button gachaTabButton;
    [SerializeField] private Button goldTabButton;
    [SerializeField] private Button playTimeTabButton;

    [SerializeField] private GameObject loadingIndicator;

    private RankingManager.RankingType currentType;

    private void Awake()
    {
        gachaTabButton.onClick.AddListener(() => SelectTab(RankingManager.RankingType.GachaCount));
        goldTabButton.onClick.AddListener(() => SelectTab(RankingManager.RankingType.Gold));
        playTimeTabButton.onClick.AddListener(() => SelectTab(RankingManager.RankingType.PlayTime));
    }

    private void Start()
    {
        Close();
    }

    public void Open()
    {
        popupRoot.SetActive(true);
        SelectTab(RankingManager.RankingType.Gold);
    }

    public void Close()
    {
        popupRoot.SetActive(false);
    }

    private void SelectTab(RankingManager.RankingType type)
    {
        currentType = type;
        LoadCurrentRanking();
    }

    private void LoadCurrentRanking()
    {
        ClearEntries();

        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);

        RankingManager.Instance.LoadRanking(currentType, OnRankingLoaded);
    }

    private void OnRankingLoaded(List<RankingManager.RankingEntry> entries)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);

        for (int i = 0; i < entrySlots.Length; i++)
        {
            if (i < entries.Count)
            {
                entrySlots[i].gameObject.SetActive(true);
                entrySlots[i].Setup(i + 1, entries[i], OnEntryClicked);
            }
            else
            {
                entrySlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnEntryClicked(RankingManager.RankingEntry entry)
    {
        UserDetailPopup.Instance.Open(entry);
    }

    private void ClearEntries()
    {
        for (int i = 0; i < entrySlots.Length; i++)
        {
            entrySlots[i].gameObject.SetActive(false);
        }
    }
}