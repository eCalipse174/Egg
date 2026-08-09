using System;
using UnityEngine;
using UnityEngine.UI;

public class GachaPresentation : MonoBehaviour
{
    public static GachaPresentation Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text subtitleText;
    [SerializeField] private Text tierText;

    private Action onDismissed;

    private EggVisualDatabase visualDB;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        visualDB = EggVisualDatabase.Instance;
        panel.SetActive(false);
    }

    public void Show(Item item, Sprite icon, Action onComplete)
    {
        var visual = visualDB.GetVisualInfo(item.ID);

        onDismissed = onComplete;

        iconImage.sprite = icon;
        nameText.text = item.Egg_Name;
        nameText.color = visual.NameColor;
        nameText.font = visual.Font;
        subtitleText.text = item.Egg_SubTitle;
        subtitleText.color = visual.NameColor;
        subtitleText.font = visual.Font;
        tierText.text = item.Tier.ToString();
        tierText.color = visualDB.TierList.list[(int)item.Tier].color;

        panel.SetActive(true);
    }

    // hook this up to an OnClick on a fullscreen invisible button covering the panel
    public void OnScreenClicked()
    {
        panel.SetActive(false);

        Action callback = onDismissed;
        onDismissed = null;
        callback?.Invoke();
    }
}