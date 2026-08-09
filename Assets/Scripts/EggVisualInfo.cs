using UnityEngine;

[CreateAssetMenu(fileName = "EggVisualInfo", menuName = "ScriptableObject/EggVisualInfo")]
public class EggVisualInfo : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color nameColor;
    [SerializeField] private Font font;
}
