using UnityEngine;

[System.Serializable]
public class ApiConfigData
{
    public string baseUrl;
    public string token;
}

public static class ApiConfig
{
    private static ApiConfigData s_data;

    public static ApiConfigData Data
    {
        get
        {
            if (s_data == null)
            {
                TextAsset file = Resources.Load<TextAsset>("ApiConfig");
                if (file == null)
                {
                    Debug.LogError("ApiConfig.json not found in Assets/Resources");
                    return null;
                }
                s_data = JsonUtility.FromJson<ApiConfigData>(file.text);
            }
            return s_data;
        }
    }
}