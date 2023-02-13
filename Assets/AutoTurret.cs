using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AutoTurret : MonoBehaviour
{
    public float hp;
    public float damage;
    public bool ready;

    [Header("Turret Parts")] 
    public Transform turretRotatorHead;

    [Header("Turret Intelligent")] 
    public Transform nearestEnemy;
    public Transform aquiredEnemy;
    public float turretRange;
    public float lookSpeed;

    [Header("Custom")] 
    public float distanceToNearest;
    
    [Header("Laser")]
    public Transform laserHolder;
    public LineRenderer laserLine;
    public float laserRange;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindNearestEnemy();
        LookAt();
        CastLaser();

        if (nearestEnemy)
        {
            distanceToNearest = Mathf.Sqrt((transform.position - nearestEnemy.position).sqrMagnitude);
            if (distanceToNearest <= turretRange)
            {
                aquiredEnemy = nearestEnemy;
            }
            else
            {
                aquiredEnemy = null;
            }
        }
        
    }

    private void CastLaser()
    {
        if (aquiredEnemy)
        {
            laserLine.enabled = true;
            
            laserLine.SetPosition(0, laserHolder.transform.position);

            Vector3 test = new Vector3(nearestEnemy.transform.position.x, laserLine.GetPosition(0).y, nearestEnemy.transform.position.z);
                
            laserLine.SetPosition(1, test);
        }
        else
        {
            laserLine.enabled = false;
        }
        
    }

    private void FindNearestEnemy()
    {
        float minimumDistance = Mathf.Infinity;

        nearestEnemy = null;

        foreach (Transform enemy in GameManager.Instance.currentLevel.enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }
           
        }
    }

    private void LookAt()
    {
        if (aquiredEnemy)
        {
            var targetRotation = Quaternion.LookRotation(aquiredEnemy.transform.position - transform.position);
            turretRotatorHead.rotation = Quaternion.Slerp(turretRotatorHead.rotation, targetRotation, lookSpeed * Time.deltaTime);

        }
    }
}
