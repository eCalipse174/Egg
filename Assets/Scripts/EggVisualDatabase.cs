using System.Collections.Generic;
using UnityEngine;

public class EggVisualDatabase : MonoBehaviour
{
    public static EggVisualDatabase Instance;

    [SerializeField] private EggVisualInfo[] allVisualInfos;
    [SerializeField] private TierList tierList;
    public TierList TierList => tierList;

    private Dictionary<int, EggVisualInfo> visualInfoById = new Dictionary<int, EggVisualInfo>();

    private void Awake()
    {
        Instance = this;

        foreach (var info in allVisualInfos)
        {
            visualInfoById[info.Id] = info;
        }
    }

    public EggVisualInfo GetVisualInfo(int id)
    {
        if (visualInfoById.TryGetValue(id, out EggVisualInfo info))
        {
            return info;
        }

        Debug.LogWarning("id " + id + "에 해당하는 EggVisualInfo를 찾지 못했습니다");
        return null;
    }
}