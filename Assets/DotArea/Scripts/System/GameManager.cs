using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
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
        if (playerLife <= 0) state = GameState.GameOver;
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
        game.UI.ShowPassedMenu();
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Count)
        {
            game.UI.ShowClearMenu();
            return;
        }
        state = GameState.Playing;
        game.UI.HidePassedMenu();
        ReSetLevel();
        game.LoadLevel();
    }
    public void ReSetLevel()
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
