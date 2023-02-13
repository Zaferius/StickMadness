using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            Destroy(Instantiate(ParticleManager.Instance.hitBullet, collision.transform.position, collision.transform.rotation), 3);
            collision.gameObject.GetComponent<EnemySpecial>().TakeDamage(damage);
        }

        if (!collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
        
        if (collision.gameObject.CompareTag("ExplosiveBarrel"))
        {
            Destroy(Instantiate(ParticleManager.Instance.hitBullet, collision.transform.position, collision.transform.rotation), 3);
            collision.gameObject.GetComponent<ExplosiveBarrel>().TakeDamage(damage);
        }
        

      
    }
}
