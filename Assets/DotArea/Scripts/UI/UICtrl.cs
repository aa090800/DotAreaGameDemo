using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//UI只改顯示UI 以及最小改遊戲狀態的GameState

//啟動遊戲->(start)start/quitgame->按下start->(playing)按下esc->(paused)resume/restart/quit->
//reume回到遊戲/restart重新導入關卡資料/quit離開遊戲->通關時->level++->新關卡->若通關最後一關顯示ClearAllLevel
//死亡時->()死亡menu

public class DotAreaUICtrl0113 : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject pausedPanel;
    public GameObject passedPanel;
    public GameObject passedText;
    public GameObject clearPanel;
    public GameObject failedPanel;

    public GameObject ArrowPanel;
    public GameObject ThanksPanel;

    //玩家資料UI
    public GameObject lifePanel;
    public GameObject percentPanel;
    public TextMeshProUGUI percentTxt;

    DotAreaGameManager0113 gameMgr;
    DotAreaScripts game;

    //==========lifeCtrl==========
    public GameObject[] life;
    public Sprite lifeFul,lifeEpt;

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
            Debug.Log("偵測");
            if (gameMgr.state == GameState.Playing) ShowPausedMenu();
            else if (gameMgr.state == GameState.Paused) HidePausedMenu();
        }
         

    }
    //======外部呼叫======
    //數值
    public void ShowLifeTxt(int value)
    {
        if (value == 3)
        {
            for (int i = 0; i < value; i++) life[i].GetComponent<Image>().sprite = lifeFul;
        }
        else
        {
            for (int i = value; i <3 ; i++) life[i].GetComponent<Image>().sprite = lifeEpt;
        }
    }
    public void ShowAchieveTxt(float value)
    {
        percentTxt.text = "達成面積 : " + value.ToString("F2")+" %";
    }
    //通關
    public void ShowPassedMenu()
    {
        passedPanel.SetActive(true);
        StartCoroutine(TextFlash(passedText));
    }
    public void HidePassedMenu()
    {
        passedPanel.SetActive(false);
    }
    //暫停
    public void ShowClearMenu()
    {
        clearPanel.SetActive(true);
    }
    public void HideClearMenu()
    {
        clearPanel.SetActive(false);
    }
    //失敗
    public void ShowFailMenu()
    {
        failedPanel.SetActive(true);
    }
    public void HideFailMenu()
    {
        failedPanel.SetActive(false);
    }
    //感謝
    public void ShowThanksMenu()
    {
        ThanksPanel.SetActive(true);
    }
    //======內部呼叫======
    void ShowStartMenu()
    {
        startPanel.SetActive(true);
        pausedPanel.SetActive(false);
        passedPanel.SetActive(false);
        clearPanel.SetActive(false);
        failedPanel.SetActive(false);

        ArrowPanel.SetActive(false);
        ThanksPanel.SetActive(false);

        gameMgr.ChangeState(GameState.MainMenu);
    }

    void ShowPausedMenu()
    {
        pausedPanel.SetActive(true);
        ArrowPanel.SetActive(false);
        HideStateTxt();
        gameMgr.ChangeState(GameState.Paused);
    }
    void HidePausedMenu()
    {
        ShowStateTxt();
        gameMgr.ChangeState(GameState.Playing);
        pausedPanel.SetActive(false);
        ArrowPanel.SetActive(true);
    }

    void ShowStateTxt()
    {
        lifePanel.gameObject.SetActive(true);
        percentPanel.gameObject.SetActive(true);
    }
    void HideStateTxt()
    {
        lifePanel.gameObject.SetActive(false);
        percentPanel.gameObject.SetActive(false);
    }

    //text閃爍效果
    IEnumerator TextFlash(GameObject obj)
    {
        obj.SetActive(true);
        while (passedPanel)
        {
            obj.SetActive(!obj.activeSelf);
            yield return new WaitForSeconds(0.7f);
        }
        obj.SetActive(false);
    }

    //=========button呼叫=========

    public void OnGameStart()
    {
        gameMgr.ChangeState(GameState.Playing);
        ShowStateTxt();
        startPanel.SetActive(false);
        ArrowPanel.SetActive(true);
    }

    public void OnResume()
    {
        HidePausedMenu();
        ShowStateTxt();

    }
    public void OnRestart()
    {
        gameMgr.ResetLevel();
        gameMgr.ChangeState(GameState.Playing);
        game.LoadLevel();
        HidePausedMenu();
        HideFailMenu();
    }
    public void OnExit()
    {
        Application.Quit();
    }
    public void PressUpBtn() { PlayerCtrl0113.OnMoveUp?.Invoke(); }
    public void PressDownBtn() { PlayerCtrl0113.OnMoveDown?.Invoke(); }
    public void PressRightBtn() { PlayerCtrl0113.OnMoveRight?.Invoke(); }
    public void PressLeftBtn() { PlayerCtrl0113.OnMoveLeft?.Invoke(); }
}
