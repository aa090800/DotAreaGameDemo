using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl0113 : MonoBehaviour
{
    bool Moving;
    bool Drawing;
    Vector2Int MoveWay;
    public Vector2Int StartDrawingPos;
    public Vector2Int gridPos;//player的gridpos
    
    float MovingTimer;


    DotAreaScripts game;
    DotAreaGridManager0113 grid;
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

        HandleInput();

        MoveTick();

    }
    //管理輸入移動方向
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveUp();
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveDown();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
    }
    void StartMove(Vector2Int dir)
    {
        MoveWay = dir;
        Moving = true;
    }

    //玩家移動邏輯
    void MoveTick()
    {
        if (!Moving) return;

        MovingTimer += Time.deltaTime;
        if (MovingTimer < 0.1f) return;
        MovingTimer = 0;


        if(!Drawing) StartDrawingPos = gridPos;//resetLine用


        Vector2Int target = gridPos + MoveWay;

        if (!grid.IsInGrid(target))
        {
            Moving = false;            
            return;
        }


        var state = grid.gridData[target.x, target.y];

        //自撞line
        if (state == CellState.Line)
        {
            TouchLine(StartDrawingPos);
            return;
        }

        //撞牆或線上移動
        if (state == CellState.Filled) 
        {
            gridPos = target;
            transform.position = grid.GridToWorldPos(gridPos);

            if (Drawing)
            {
                Drawing = false;
                grid.finishDraw(game.GetEnemyGridPos());

            }
            Moving = false;
        }

        //起步畫線和移動中
        if (state == CellState.Empty)
        {
            Drawing = true;
            grid.gridData[target.x, target.y] = CellState.Line;
            grid.DrawCell(target, Color.gray);
        }

        gridPos = target;
        transform.position = grid.GridToWorldPos(gridPos);
    }

    //自撞線
    public void TouchLine(Vector2Int pos)
    {
        gridPos = pos;
        transform.position = grid.GridToWorldPos(gridPos);

        Moving = false;
        gameMgr.LostLife();
        grid.ClearAllLine();

        game.UI.ShowLifeTxt(gameMgr.playerLife);
    }


    //======玩家移動======
    void MoveUp()
    {
        StartMove(Vector2Int.up);
    }
    void MoveDown()
    {
        StartMove(Vector2Int.down);
    }
    void MoveLeft()
    {
        StartMove(Vector2Int.left);
    }
    void MoveRight()
    {
        StartMove(Vector2Int.right);
    }

    public static System.Action OnMoveLeft,OnMoveRight,OnMoveUp,OnMoveDown;
    private void OnEnable()
    {
        OnMoveUp += MoveUp;
        OnMoveDown += MoveDown;
        OnMoveLeft += MoveLeft;
        OnMoveRight += MoveRight;
    }
    private void OnDisable()
    {
        OnMoveUp -= MoveUp;
        OnMoveDown -= MoveDown;
        OnMoveLeft -= MoveLeft;
        OnMoveRight -= MoveRight;
    }
   
    
}
