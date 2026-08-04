using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    private Users userInfo;
    public Users UserInfo => userInfo;

    private bool isSignIn;
    private double playTimeSeconds;

    private bool isSaveCompleted;

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

    private void Update()
    {
        if (isSignIn)
            playTimeSeconds += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(SaveAndQuit());
    }

    public void RegisterInfo(Users users)
    {
        userInfo = users;
        SignIn();
        Debug.Log(users.play_time_seconds);
    }


    // ---------Account---------

    public void SignIn()
    {
        isSignIn = true;
        playTimeSeconds = UserInfo.play_time_seconds;
        ItemManager.Instance.LoadAllItems
            (() =>
            Debug.Log("아이템 캐시 준비 완료")
            );
    }

    public IEnumerator SaveAndQuit()
    {
        Save();
        yield return new WaitUntil(() => isSaveCompleted);

        Quit();
    }

    public void Save()
    {
        Debug.Log("Save");


        if (userInfo != null)
        {
            Debug.Log("userInfo is not null");
            userInfo.play_time_seconds = (int)playTimeSeconds;
            NetworkManager.Instance.SaveUserProgress
                (
                userInfo.id,
                userInfo.gold,
                userInfo.enhance_level,
                userInfo.gacha_count,
                userInfo.play_time_seconds,
                (success, json) => 
                {
                    Debug.Log("success: " + success);
                    Debug.Log("response: " + json);
                    isSaveCompleted = true;
                }
                );
        }
        else
        {
            Debug.LogWarning("userInfo is null");
            isSaveCompleted = true;
        }
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
