using System;
using UnityEngine;

[Obsolete]
public class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo instance;
    public static PlayerInfo Instance => instance;

    private Users userInfo;
    public Users UserInfo => userInfo;

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

    //정보 불러오기... 게임매니저에서 가져와야 할지도
    public void RegisterInfo(Users users)
    {
        userInfo = users;
    }
}