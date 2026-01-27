using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellState { Empty, Filled, Line, Wall }

//line->wall 初始黑線也是wall


public class DotAreaGridManager0113 : MonoBehaviour
{
    public int width = 60;
    public int height = 40;
    float cellSize = 0.1f;
    public GameObject cellPrefab;

    public float filledCount;
    public float totalCellCount;

    public CellState[,] gridData;
    Dictionary<Vector2Int, GameObject> SpawnedCell = new Dictionary<Vector2Int, GameObject>();
    DotAreaGameManager0113 gameMgr;

    Vector2 StartPos;


    public void InitGrid(LevelData data)
    {
        width = data.width;
        height = data.height;
               

        gameMgr = DotAreaGameManager0113.Instance;
        gameMgr.passRequirement = data.passPercent;

        StartPos = new Vector2(-cellSize * width / 2, -cellSize * height / 2);
        gridData = new CellState[width, height];


        int _temptotal = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //最外圈設為filled 其他裡面為Empty
                if (i == 0 || i == width - 1) gridData[i, j] = CellState.Wall;
                else if (j == 0 || j == height - 1) gridData[i, j] = CellState.Wall;
                else gridData[i, j] = CellState.Empty;

                if (gridData[i, j] == CellState.Wall)
                {
                    DrawCell(new Vector2Int(i, j), Color.black);
                    _temptotal++;
                }
            }
        }
        totalCellCount = width * height - _temptotal;
    }



    //grid<->world座標轉換
    public Vector2 GridToWorldPos(Vector2Int gridPos)
    {
        float x = StartPos.x + gridPos.x * cellSize + cellSize * 0.5f;
        float y = StartPos.y + gridPos.y * cellSize + cellSize * 0.5f;
        return new Vector2(x, y);
    }
    public Vector2Int WorldToGridPos(Vector2 pos)
    {
        int x = Mathf.FloorToInt((pos.x - StartPos.x) / cellSize);
        int y = Mathf.FloorToInt((pos.y - StartPos.y) / cellSize);
        return new Vector2Int(x, y);
    }

    //把gridPos加入SpawnedCell內並生成顏色Cell
    public void DrawCell(Vector2Int gridPos, Color color)
    {
        GameObject go = Instantiate(cellPrefab, GridToWorldPos(gridPos), Quaternion.identity);
        SpawnedCell.Add(gridPos, go);
        go.GetComponent<SpriteRenderer>().color = color;
    }

    //判定是否超出grid座標
    public bool IsInGrid(Vector2Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < width && gridPos.y >= 0 && gridPos.y < height;
    }


    //自撞或敵人撞到時 清除line的部分
    public void ClearAllLine()
    {
        //清除line
        List<Vector2Int> lineCells = new List<Vector2Int>();
        foreach (var kv in SpawnedCell)
        {
            if (gridData[kv.Key.x, kv.Key.y] == CellState.Line)
            {
                lineCells.Add(kv.Key);
            }
        }

        foreach (var pos in lineCells)
        {
            Destroy(SpawnedCell[pos]);
            SpawnedCell.Remove(pos);
            gridData[pos.x, pos.y] = CellState.Empty;
        }        
    }


    //
    public void finishDraw(List<Vector2Int> pos)
    {
        ConvertLineToFilled();
        FillClosedArea(pos);
        gameMgr.CountPercent();
        if (gameMgr.CheckPassed(gameMgr.achievedCount))
        {
            gameMgr.GamePass();
        }
    }

    void ConvertLineToFilled()
    {
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

                gridData[pos.x, pos.y] = CellState.Wall;
                DrawCell(pos, Color.black);

                filledCount++;
            }
        }
    }
    void FillClosedArea(List<Vector2Int> enemy)
    {
        
        bool[,] visited = new bool[width, height];
        Queue<Vector2Int> current = new Queue<Vector2Int>();
        foreach(var e in enemy)
        {
            current.Enqueue(e);
            visited[e.x, e.y] = true;
        }

        Vector2Int[] vct2Offset = { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };

        while (current.Count > 0)
        {
            Vector2Int cur = current.Dequeue();

            foreach (var next in vct2Offset)
            {
                Vector2Int nextgrid = next + cur;
                if (!IsInGrid(nextgrid) || visited[nextgrid.x, nextgrid.y]) continue;

                if (gridData[nextgrid.x, nextgrid.y] == CellState.Empty)
                {
                    visited[nextgrid.x, nextgrid.y] = true;
                    current.Enqueue(nextgrid);
                }
            }
        }
        //上色
        Color[] colors = { Color.blue, Color.red, Color.white, Color.yellow, Color.cyan, Color.green, Color.magenta, };
        Color GetRandomColor() { return colors[Random.Range(0, colors.Length)]; }
        Color color = GetRandomColor()*new Color(0.5f,0.5f,0.5f);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (gridData[i, j] == CellState.Empty && !visited[i, j])
                {
                    gridData[i, j] = CellState.Filled;
                    DrawCell(new Vector2Int(i, j), color);

                    filledCount++;
                }
            }
        }
    }
    public void DestroyAllCell()
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        foreach(var kv in SpawnedCell)
        {
            Vector2Int pos = kv.Key;
            if (gridData[pos.x, pos.y] == CellState.Filled || gridData[pos.x, pos.y] == CellState.Line || gridData[pos.x, pos.y] == CellState.Wall)
            {
                cells.Add(pos);
            }
        }
        foreach (var pos in cells)
        {
            Destroy(SpawnedCell[pos]);
            SpawnedCell.Remove(pos);

        }
    }
}
