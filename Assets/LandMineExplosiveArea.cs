using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineExplosiveArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            other.GetComponent<EnemySpecial>().TakeDamage(50);
        }
    }
}
