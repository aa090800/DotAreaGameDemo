using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCtrl0113 : MonoBehaviour
{
    public Vector2Int MoveWay;
    public Vector2Int gridPos;//Enemy的gridPos
    float MovingTimer;

    //DotAreaGameManager0113 game;
    DotAreaGridManager0113 grid;
    DotAreaScripts game;
    DotAreaGameManager0113 gameMgr;

    public void Init(DotAreaGridManager0113 gridMgr, DotAreaScripts game)
    {
        grid = gridMgr;
        this.game = game;

        gameMgr = DotAreaGameManager0113.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMgr.state != GameState.Playing) return;
        MoveTick();

    }

    //會移動 撞到玩家怎麼處理

    void MoveTick()
    {
        MovingTimer += Time.deltaTime;
        if (MovingTimer < 0.1f) return;
        MovingTimer = 0;

        //enemy自動移動邏輯 :斜著移動 撞到filled反彈
        Vector2Int target = gridPos + MoveWay;

        Vector2Int checkX = new Vector2Int(MoveWay.x + gridPos.x, gridPos.y);
        if (grid.gridData[checkX.x, checkX.y] == CellState.Wall && grid.IsInGrid(checkX))
        {
            MoveWay.x *= -1;
        }
        Vector2Int checkY = new Vector2Int(gridPos.x, MoveWay.y + gridPos.y);
        if (grid.gridData[checkY.x, checkY.y] == CellState.Wall && grid.IsInGrid(checkY))
        {
            MoveWay.y *= -1;
        }
        target = gridPos + MoveWay;

        //撞到玩家或碰到線
        if (grid.gridData[target.x, target.y] == CellState.Line|| game.GetPlayerGridPos()==target )
        {
            game.player.TouchLine(game.player.StartDrawingPos);

            game.UI.ShowLifeTxt(gameMgr.playerLife);
        }

        gridPos = target;
        transform.position = grid.GridToWorldPos(gridPos);
    }
}
