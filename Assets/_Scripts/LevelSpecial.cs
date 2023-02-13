using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpecial : MonoBehaviour
{
    public int fightZoneLeft;
    public GameObject test;
    public List<Transform> enemies = new List<Transform>();
    void Start()
    {
        GameManager.Instance.currentLevel = this;
        test.transform.SetParent(UIManager.Instance.levelMaps);
    }

    public void GameCheck()
    {
        if (fightZoneLeft <= 0)
        {
            GameManager.Instance.GameWin();
        }
    }
    
}
