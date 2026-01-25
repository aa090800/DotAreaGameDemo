using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotAreaScripts : MonoBehaviour
{
    public DotAreaGridManager0113 grid;
    public PlayerCtrl0113 player;
    public EnemyCtrl0113 enemy;
    public DotAreaUICtrl0113 UI;
    DotAreaGameManager0113 game;

    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public List<GameObject> enemiesObj = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        LoadLevel();

        UI.Init(this);
        UI.ShowLifeTxt(game.playerLife);
        UI.ShowAchieveTxt(game.achievedCount);


    }

    //讀取關卡資料
    public void LoadLevel()
    {
        game = DotAreaGameManager0113.Instance;
        game.Init(this);

        LevelData level = game.CurrentLevel;
        grid.InitGrid(level);//初始化

        GameObject playerObj = Instantiate(PlayerPrefab);
        player = playerObj.GetComponent<PlayerCtrl0113>();
        player.Init(grid, this);
        player.gridPos = level.playerStart;
        player.transform.position = grid.GridToWorldPos(player.gridPos);


        foreach (var enemyData in level.enemies)
        {
            GameObject enemyObj = Instantiate(EnemyPrefab);
            enemy = enemyObj.GetComponent<EnemyCtrl0113>();
            enemy.Init(grid, this);
            enemy.gridPos = enemyData.startPos;
            enemy.MoveWay = enemyData.startDir;
            enemy.transform.position = grid.GridToWorldPos(enemy.gridPos);
            enemiesObj.Add(enemyObj);
        }

    }

    //enemy&player抓位置用
    public Vector2Int GetPlayerGridPos()
    {
        return player.gridPos;
    }
    public List<Vector2Int> GetEnemyGridPos()
    {
        List<Vector2Int> enemies = new List<Vector2Int>();
        foreach(var e in enemiesObj)
        {
            enemies.Add(grid.WorldToGridPos(e.gameObject.transform.position));
        }
        return enemies;
    }
}
