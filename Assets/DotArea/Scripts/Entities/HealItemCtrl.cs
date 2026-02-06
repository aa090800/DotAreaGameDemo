using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItemCtrl : MonoBehaviour
{
    [SerializeField] int healAmount = 1;
    public DotAreaScripts game;
    DotAreaGameManager gameMgr;
    
    public void Init(DotAreaScripts game)
    {
        this.game = game;
    }

    private void Start()
    {
        gameMgr = DotAreaGameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (gameMgr.playerLife < gameMgr.maxPlayerLife) gameMgr.playerLife+= healAmount;
            gameMgr.totalHeart--;
            game.UI.RefreshLifeTxt();
            Destroy(gameObject);
        }
    }
}
