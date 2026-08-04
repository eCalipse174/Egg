using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private LoginHandler loginHandler;
    [SerializeField] private List<Button> buttons;
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    public void GameStartButton()
    {
        loginHandler.TryLogin();
        startButton.interactable = false;
        quitButton.interactable = false;
        //foreach (var button in buttons) 
        //    button.interactable = false;

    }

    public void QuitButton()
    {
        StartCoroutine(GameManager.Instance.SaveAndQuit());
    }
}
