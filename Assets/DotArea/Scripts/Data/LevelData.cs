using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//關卡資料
[System.Serializable]
public class LevelData
{
    public int width;
    public int height;
    public float passPercent;
    public Vector2Int playerStart;

    public List<EnemyData> enemies;

}

