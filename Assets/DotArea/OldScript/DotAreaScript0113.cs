using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;





public class DotAreaScript0113 : MonoBehaviour
{
    //待增加/修正功能 只再filled上自動行走


    public static DotAreaScript0113 Instance;
    public GameState state;
    private void Awake()
    {
        Instance = this;
    }

    public void ChangeState(GameState newState)
    {
        state = newState;
        Time.timeScale = (state == GameState.Paused) ? 0 : 1;
    }



    public TextMeshProUGUI lifeTxt, percentTxt,passedTxt;
    public GameObject Player;
    public GameObject Enemy;

    public GameObject cellPrefab;


    Dictionary<Vector2Int, GameObject> SpawnedCell = new Dictionary<Vector2Int, GameObject>();

    CellState[,] gridData;
    Vector2 StartPos;

    public int PlayerLife = 3;
    
    int width=60;
    int height=40;
    float cellSize=0.1f;
    SpriteRenderer sr;
    public Vector2Int playerGridPos;
    public Vector2Int enemyGridPos;

    //public static bool GamePaused = false;

    //計算面積百分比
    float filledPercent;
    int total, filled=0;

    //通關條件
    float passedEquirement = 70;


    void Start()
    {
        StartPos = new Vector2(-cellSize * width / 2, -cellSize * height / 2);
        gridData = new CellState[width, height];

        passedTxt.gameObject.SetActive(false);

        showLifeTxt();


        int _temptotal = 0;
        for(int i = 0; i < width; i++)
        {
            for(int j=0;j<height; j++)
            {
                //最外圈設為filled 其他裡面為Empty
                if (i == 0 || i == width - 1) gridData[i, j] = CellState.Filled;
                else if (j == 0 || j == height - 1) gridData[i, j] = CellState.Filled;
                else gridData[i, j] = CellState.Empty;

                if (gridData[i, j] == CellState.Filled)
                {
                    DrawCell(new Vector2Int(i, j), Color.black);
                    _temptotal++;
                }
            }
        }
        //最外圈不算
        total = width * height - _temptotal;//60*40-196 總面積:2204
        filledPercent = filled / total *100 ;//50/60*40-196
        percentTxt.text = "達成面積 : " + filledPercent.ToString("F2") + "%";

        playerGridPos = new Vector2Int(width / 2, 0);
        Player = Instantiate(Player, GridToWorldPos(playerGridPos), Quaternion.identity);
        enemyGridPos = new Vector2Int(width / 2 / 2, height / 2);
        Enemy = Instantiate(Enemy, GridToWorldPos(playerGridPos), Quaternion.identity);

        EnemyMoveWay = new Vector2Int(-1, 1);
    }


    //把gridPos加入SpawnedCell內並生成顏色Cell
    void DrawCell(Vector2Int gridPos, Color color)
    {
        GameObject go = Instantiate(cellPrefab, GridToWorldPos(gridPos), Quaternion.identity);
        SpawnedCell.Add(gridPos, go);

        sr = go.GetComponent<SpriteRenderer>();
        sr.color = color;
    }


    //grid<->world座標轉換
    Vector2 GridToWorldPos(Vector2Int gridPos)
    {
        float x = StartPos.x + gridPos.x * cellSize + cellSize * 0.5f;
        float y = StartPos.y + gridPos.y * cellSize + cellSize * 0.5f;
        return new Vector2(x, y);
    }
    Vector2Int WorldToGridPos(Vector2 pos)
    {
        int x = Mathf.FloorToInt((pos.x - StartPos.x) / cellSize );
        int y = Mathf.FloorToInt((pos.y - StartPos.y) / cellSize) ;
        return new Vector2Int(x, y);
    }



    void Update()
    {
        if (state == GameState.Playing)
        {
            PlayerMove();
            EnemyMove();
        }
        
    }


    //Player部分//

    
    bool PlayerMoving;
    bool isDrawingLine;
    Vector2Int PlayerMoveWay;
    float MovingTimer;
    //玩家移動條件
    void PlayerMove()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {            
            PlayerMoveWay=Vector2Int.up;
            PlayerMoving = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerMoveWay = Vector2Int.down;
            PlayerMoving = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerMoveWay = Vector2Int.right;
            PlayerMoving = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerMoveWay = Vector2Int.left;
            PlayerMoving = true;
        }

        if (PlayerMoving)
        {
            MovingTimer += Time.deltaTime;
            if (MovingTimer > 0.1f)
            {
                TryMovePlayer(PlayerMoveWay);
                MovingTimer = 0;
            }
        }
    }


    Vector2Int startMovingPos;
    //玩家移動
    //4種狀況判斷:(1)filled->empty:起步 (2)filled->filled:邊界移動 (3)empty->empty:移動 (4)empty->filled:終點
    void TryMovePlayer(Vector2Int dir)
    {
        //紀錄起點(碰撞用)
        if (!isDrawingLine) startMovingPos = playerGridPos;

        //基本移動邏輯
        Vector2Int target = playerGridPos + dir;

        //出界不動
        if (!IsInGrid(target))
        {
            StopDrawing();
            return;
        }

        //邊界上移動和到達終點
        if (gridData[target.x, target.y] == CellState.Filled)
        {
            //邊界上移動&&最後移動到邊界
            playerGridPos = target;
            Player.transform.position = GridToWorldPos(playerGridPos);


            if (isDrawingLine)
            {
                //結算:line改成filled
                finishDrawing();
                //部分區域改成filled
                DrawFilledArea();

                
            }

            StopDrawing();

            return;
        }
        
        //碰到自己的線
        if(gridData[target.x, target.y] == CellState.Line && isDrawingLine)
        {
            touchLine();
            return;
        }

        //起步跟移動中
        if (gridData[target.x, target.y] == CellState.Empty)
        {
            if (!isDrawingLine) isDrawingLine = true;

            gridData[target.x, target.y] = CellState.Line;
            DrawCell(target, Color.gray);
        }

        playerGridPos = target;
        Player.transform.position = GridToWorldPos(playerGridPos);

    }

    //畫線停止
    void StopDrawing()
    {
        PlayerMoving = false;
        isDrawingLine = false;
        MovingTimer = 0;
    }

    //碰到line時的處理
    void touchLine()
    {
        //扣血
        PlayerLife--;

        showLifeTxt();
        StopDrawing();

        

        //回到出發點
        playerGridPos = startMovingPos;
        Player.transform.position = GridToWorldPos(playerGridPos);

        //清除line
        List<Vector2Int> lineCells = new List<Vector2Int>();
        foreach (var kv in SpawnedCell)
        {
            Vector2Int pos = kv.Key;
            if (gridData[pos.x, pos.y] == CellState.Line)
            {
                lineCells.Add(pos);
            }
        }
        foreach (var pos in lineCells)
        {
            if (SpawnedCell.TryGetValue(pos, out GameObject go))
            {
                Destroy(go);
                SpawnedCell.Remove(pos);

                gridData[pos.x, pos.y] = CellState.Empty;
            }
        }
    }

    //畫完線時封閉格子
    //Empty->Filled封閉格子 把line轉成filled
    void finishDrawing()
    {
        StopDrawing();
        List<Vector2Int> lineCells = new List<Vector2Int>();
        foreach(var kv in SpawnedCell)
        {
            Vector2Int pos = kv.Key;
            if (gridData[pos.x, pos.y] == CellState.Line)
            {
                lineCells.Add(pos);
            }
        }
        foreach (var pos in lineCells)
        {
            if (SpawnedCell.TryGetValue(pos,out GameObject go))
            {                                
                Destroy(go);
                SpawnedCell.Remove(pos);

                gridData[pos.x, pos.y] = CellState.Filled;
                DrawCell(pos, Color.black);
                filled++;
            }
        }
    }

    //判定是否超出grid座標
    bool IsInGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < width && gridPos.y >= 0 && gridPos.y < height;
    }

    //敵人部分//

    //起步往左上
    //左右碰到filled 上->下 下->上
    //上下碰到filled 左->右 右->左
    //碰到line 或 玩家 -> 玩家扣血 觸發碰到line邏輯 ->不反彈
    //
    Vector2Int EnemyMoveWay;
    float EnemyMovingTimer;
    void EnemyMove()
    {
        EnemyMovingTimer += Time.deltaTime;
        if (EnemyMovingTimer < 0.1f) return;
        EnemyMovingTimer = 0;

        Vector2Int target = enemyGridPos + EnemyMoveWay;

        Vector2Int checkX = new Vector2Int(EnemyMoveWay.x + enemyGridPos.x, enemyGridPos.y);
        if(gridData[checkX.x, checkX.y] == CellState.Filled && IsInGrid(checkX))
        {
            EnemyMoveWay.x *= -1;
        }
        Vector2Int checkY = new Vector2Int( enemyGridPos.x ,EnemyMoveWay.y + enemyGridPos.y);
        if (gridData[checkY.x, checkY.y] == CellState.Filled && IsInGrid(checkY))
        {
            EnemyMoveWay.y *= -1;
        }
        target = enemyGridPos + EnemyMoveWay;

        //撞到玩家or撞到線
        if (gridData[target.x, target.y] == CellState.Line || playerGridPos == target) 
        {
            touchLine();
        }


        enemyGridPos = target;
        Enemy.transform.position = GridToWorldPos(enemyGridPos);
    }

    //封閉格子填色
    void DrawFilledArea()
    {
        bool[,] visited = new bool[width, height];
        Queue<Vector2Int> current = new Queue<Vector2Int>();

        current.Enqueue(enemyGridPos);
        visited[enemyGridPos.x, enemyGridPos.y] = true;

        Vector2Int[] vct2Offset = { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };

        while (current.Count > 0)
        {
            Vector2Int cur = current.Dequeue();

            foreach(var next in vct2Offset)
            {
                Vector2Int nextgrid = next + cur;
                if (!IsInGrid(nextgrid) || visited[nextgrid.x,nextgrid.y]) continue;

                if(gridData[nextgrid.x, nextgrid.y] == CellState.Empty)
                {
                    visited[nextgrid.x, nextgrid.y] = true;
                    current.Enqueue(nextgrid);
                }                
            }
        }
        //上色
        Color[] colors = { Color.blue, Color.red, Color.white, Color.yellow, Color.cyan, Color.green, Color.magenta, };
        Color GetRandomColor() { return colors[Random.Range(0, colors.Length)]; }
        Color color = GetRandomColor();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gridData[i, j] == CellState.Empty && !visited[i, j])
                {
                    gridData[i, j] = CellState.Filled;
                    DrawCell(new Vector2Int(i, j), color);
                    filled++;
                }
            }
        }

        CountFilledPercent();
    }

    //計算達成面積
    void CountFilledPercent()
    {
        filledPercent = (float)filled / total * 100;

        percentTxt.text = "達成面積 : " + filledPercent.ToString("F2") + "%";

        //通關條件
        if (filledPercent >= passedEquirement)
        {
            //GamePaused = true;
            state = GameState.Passed;
            passedTxt.gameObject.SetActive(true);

        }
    }
    //UI 顯示生命
    void showLifeTxt()
    {
        lifeTxt.text = "生命 : " + PlayerLife;
    }
}
