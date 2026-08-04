using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

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

    public void Gacha()
    {
        Debug.Log("°¡Ã­!");
    }

    public void RegisterInfo(Users users)
    {
        userInfo = users;
    }
}
