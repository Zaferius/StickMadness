using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideFlameSphere : MonoBehaviour
{
    public float damage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            if (other.CompareTag("Zombie"))
            {
                Destroy(Instantiate(ParticleManager.Instance.hitBullet, other.transform.position, other.transform.rotation), 3);
                other.gameObject.GetComponent<EnemySpecial>().TakeDamage(damage);
                other.gameObject.GetComponent<EnemySpecial>().Burn();
            }
        }
    }
}
