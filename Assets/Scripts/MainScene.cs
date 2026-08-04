using UnityEngine;

public class MainScene : MonoBehaviour
{
    public void Gacha()
    {
        Debug.Log("Gacha!");
    }

    public void Quit()
    {
        StartCoroutine(GameManager.Instance.SaveAndQuit());
    }
}