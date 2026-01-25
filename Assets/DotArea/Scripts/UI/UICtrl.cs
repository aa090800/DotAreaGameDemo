using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//UI只改顯示UI 以及最小改遊戲狀態的GameState

public class DotAreaUICtrl0113 : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject pausedPanel;
    public GameObject passedPanel;
    public GameObject clearPanel;

    public TextMeshProUGUI lifeTxt;
    public TextMeshProUGUI percentTxt;

    DotAreaGameManager0113 gameMgr;
    DotAreaScripts game;

    public void Init(DotAreaScripts game)
    {
        this.game = game;
    }


    void Start()
    {
        gameMgr = DotAreaGameManager0113.Instance;
        ShowStartMenu();
        HideStateTxt();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameMgr.state == GameState.Playing) ShowPausedMenu();
            else if (gameMgr.state == GameState.Paused) HidePausedMenu();
        }
    }
    //======外部呼叫======
    public void ShowLifeTxt(int value)
    {
        lifeTxt.text = "生命 : " + value;
    }
    public void ShowAchieveTxt(float value)
    {
        percentTxt.text = "達成面積 : " + value.ToString("F2")+" %";
    }
    public void ShowPassedMenu()
    {
        passedPanel.SetActive(true);
    }
    public void HidePassedMenu()
    {
        passedPanel.SetActive(false);
    }
    public void ShowClearMenu()
    {
        clearPanel.SetActive(true);
    }
    public void HideClearMenu()
    {
        clearPanel.SetActive(false);
    }
    //======內部呼叫======
    void ShowStartMenu()
    {
        startPanel.SetActive(true);
        pausedPanel.SetActive(false);
        passedPanel.SetActive(false);
        clearPanel.SetActive(false);

        gameMgr.ChangeState(GameState.MainMenu);
    }

    void ShowPausedMenu()
    {
        pausedPanel.SetActive(true);
        HideStateTxt();
        gameMgr.ChangeState(GameState.Paused);
    }
    void HidePausedMenu()
    {
        ShowStateTxt();
        gameMgr.ChangeState(GameState.Playing);
        pausedPanel.SetActive(false);
    }
    void ShowStateTxt()
    {
        lifeTxt.gameObject.SetActive(true);
        percentTxt.gameObject.SetActive(true);
    }
    void HideStateTxt()
    {
        lifeTxt.gameObject.SetActive(false);
        percentTxt.gameObject.SetActive(false);
    }

    //=========button呼叫=========

    public void OnGameStart()
    {
        gameMgr.ChangeState(GameState.Playing);
        ShowStateTxt();
        startPanel.SetActive(false);
    }

    public void OnResume()
    {
        HidePausedMenu();
        ShowStateTxt();

    }
    public void OnRestart()
    {
        gameMgr.ReSetLevel();
        gameMgr.ChangeState(GameState.Playing);
        game.LoadLevel();
        HidePausedMenu();
    }
    public void OnExit()
    {
        Application.Quit();
    }
}
