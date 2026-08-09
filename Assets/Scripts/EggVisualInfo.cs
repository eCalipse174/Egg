using UnityEngine;

[CreateAssetMenu(fileName = "EggVisualInfo", menuName = "ScriptableObject/EggVisualInfo")]
public class EggVisualInfo : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color nameColor;
    [SerializeField] private Font font;

    public int Id => id;
    public Sprite Sprite => sprite;
    public Color NameColor => nameColor;
    public Font Font => font;
}
