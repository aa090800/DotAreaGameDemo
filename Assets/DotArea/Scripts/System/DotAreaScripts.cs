using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotAreaScripts : MonoBehaviour
{
    public DotAreaGridManager grid;
    public PlayerCtrl player;
    public EnemyCtrl enemy;
    public DotAreaUICtrl UI;
    public HealItemCtrl healItem;
    DotAreaGameManager game;

    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject HealthItemPrefab;
    public List<GameObject> enemiesObj = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        LoadLevel();

        UI.Init(this);
        UI.RefreshLifeTxt();
        UI.ShowAchieveTxt(game.achievedCount);


    }

    //讀取關卡資料
    public void LoadLevel()
    {
        game = DotAreaGameManager.Instance;
        game.Init(this);

        LevelData level = game.CurrentLevel;
        grid.InitGrid(level);//初始化

        GameObject playerObj = Instantiate(PlayerPrefab);
        player = playerObj.GetComponent<PlayerCtrl>();
        player.Init(grid, this);
        player.gridPos = level.playerStart;
        player.transform.position = grid.GridToWorldPos(player.gridPos);


        //enemy生成附值
        foreach (var enemyData in level.enemies)
        {
            GameObject enemyObj = Instantiate(EnemyPrefab);
            enemy = enemyObj.GetComponent<EnemyCtrl>();
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
