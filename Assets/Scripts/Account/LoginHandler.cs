using UnityEngine;
using UnityEngine.UI;

public class LoginHandler : MonoBehaviour
{
    [HideInInspector] public int currentUserId;
    [HideInInspector] public Users currentUserData;

    [SerializeField] private GameObject nicknameInputPanel;
    [SerializeField] private InputField nicknameInputField;

    private string deviceId;

    public void TryLogin()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;

        NetworkManager.Instance.GetUser(deviceId, (success, json) =>
        {
            if (!success)
            {
                Debug.Log("로그인 조회 실패, 네트워크 확인 필요");
                return;
            }

            UsersListResponse response = JsonUtility.FromJson<UsersListResponse>(json);

            if (response.list.Length > 0)
            {
                currentUserData = response.list[0];
                currentUserId = currentUserData.id;
                Debug.Log("기존 유저 로그인, Id: " + currentUserId);
                RegisterAndStart();
            }
            else
            {
                nicknameInputPanel.SetActive(true);
            }
        });
    }

    public void OnConfirmNicknameButtonClicked()
    {
        string nickname = nicknameInputField.text;

        if (string.IsNullOrEmpty(nickname))
        {
            Debug.Log("닉네임을 입력해야 합니다");
            return;
        }

        NetworkManager.Instance.CreateUser(deviceId, nickname, (success, json) =>
        {
            if (!success)
            {
                Debug.Log("유저 생성 실패");
                return;
            }

            Users newUser = JsonUtility.FromJson<Users>(json);
            currentUserData = newUser;
            currentUserId = newUser.id;
            nicknameInputPanel.SetActive(false);
            Debug.Log("새 유저 생성 완료, Id: " + currentUserId);
            RegisterAndStart();
        });
    }

    private void RegisterAndStart()
    {
        GameManager.Instance.RegisterInfo(currentUserData);
        FadeSceneManager.Instance.LoadScene("MainScene");
    }
}