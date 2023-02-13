using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnvPieceSpecial : MonoBehaviour
{
    public bool cleared;
    public Image uiPiece;
    public int enemyCount;
    public int enemyLeft;
    public List<EnemySpecial> enemies = new List<EnemySpecial>();
    public List<Transform> points = new List<Transform>();
    void Start()
    {
        SpawnEnemies();
        PrepareZone();
    }
    

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
           var enemy = Instantiate(enemies[Random.Range(0, enemies.Count)], points[i].transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
           enemy.currentEnvironment = this;
        }

        enemyLeft = enemyCount;

    }

    public void Cleared()
    {
        GameManager.Instance.currentLevel.fightZoneLeft--;
        GameManager.Instance.currentLevel.GameCheck();
        uiPiece.color = Color.green;
        cleared = true;
    }

    public void PrepareZone()
    {
        if (cleared)
        {
            uiPiece.color = Color.green;
        }
        else
        {
            TimeManager.Instance.transform.DOMoveX(0, 0.1f).OnComplete(() =>
            {
                GameManager.Instance.currentLevel.fightZoneLeft++;
            });
           
        }
    }
    
}
