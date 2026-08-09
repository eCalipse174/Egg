using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "EggVisualInfo", menuName = "ScriptableObject/EggVisualInfo")]
public class EggVisualInfo : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color nameColor;
    [SerializeField] private Font font;
    [SerializeField] private VideoClip cutsceneClip;

    public int Id => id;
    public Sprite Sprite => sprite;
    public Color NameColor => nameColor;
    public Font Font => font;
    public VideoClip CutsceneClip => cutsceneClip;
    public bool HasCutscene => cutsceneClip != null;
}