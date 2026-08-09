using System;
using UnityEngine;
using UnityEngine.UI;

public class CollectionListEntryUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    public void Setup(Sprite icon, Action onClick)
    {
        iconImage.sprite = icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }
}