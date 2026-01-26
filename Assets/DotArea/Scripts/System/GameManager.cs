using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { MainMenu, Playing, Paused, Passed, GameOver }
public class DotAreaGameManager0113 : MonoBehaviour
{
    public List<LevelData> levels;
    public int currentLevelIndex;

    public LevelData CurrentLevel => levels[currentLevelIndex];

    DotAreaScripts game;

    public static DotAreaGameManager0113 Instance;
    public GameState state;

    public float achievedCount;
    public int playerLife = 3;
    public float passRequirement = 70f;

    public void Init(DotAreaScripts game)
    {
        this.game = game;
    }
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if(state == GameState.Passed)
        {
            if (Input.anyKeyDown)
            {
                if (currentLevelIndex >= levels.Count)
                {
                    game.UI.ShowThanksMenu();
                    return;
                }
                NextLevel();
            }
        }
    }
    public void ChangeState(GameState newState)
    {
        state = newState;
        Time.timeScale = (state == GameState.Paused) ? 0 : 1;
    }

    public void LostLife()
    {
        playerLife--;
        if (playerLife <= 0)
        {
            state = GameState.GameOver;
            game.UI.ShowFailMenu();
        }
    }

    public bool CheckPassed(float percent)
    {
        return percent >= passRequirement;
    }

    public void CountPercent()
    {
        achievedCount = (float)game.grid.filledCount / game.grid.totalCellCount * 100f;
        game.UI.ShowAchieveTxt(achievedCount);
    }
    public void GamePass()
    {        
        state = GameState.Passed;
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Count)
        {
            game.UI.ShowClearMenu();
            return;
        }        
        game.UI.ShowPassedMenu();
    }

    public void NextLevel()
    {        
        
        state = GameState.Playing;
        game.UI.HidePassedMenu();
        ResetLevel();
        game.LoadLevel();
    }
    public void ResetLevel()
    {
        playerLife = 3;
        game.grid.filledCount = 0;
        CountPercent();

        game.grid.DestroyAllCell();
        Destroy(game.player.gameObject);
        foreach(var enemies in levels)
        {
            foreach(var e in game.enemiesObj)
            {
                Destroy(e);
            }
        }
        game.enemiesObj.Clear();
    }
}
