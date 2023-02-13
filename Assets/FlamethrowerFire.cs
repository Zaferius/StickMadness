using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerFire : MonoBehaviour
{
    public float damage;
    public float rateOfFlame;
    public float rateOfFlameTime;
    public bool canBurn;

    public GameObject sideFlameObj;
    void Start()
    {
        canBurn = true;
        rateOfFlame = rateOfFlameTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canBurn)
        {
            rateOfFlame -= 5 * Time.deltaTime;
            if (rateOfFlame <= 0)
            {
                rateOfFlame = rateOfFlameTime;
                canBurn = true;
            }
        }
    }
    
    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Zombie") && canBurn)
        {
            Destroy(Instantiate(ParticleManager.Instance.hitBullet, other.transform.position, other.transform.rotation), 3);
            other.gameObject.GetComponent<EnemySpecial>().TakeDamage(damage);
            other.gameObject.GetComponent<EnemySpecial>().Burn();
            
            
            var sideFlameSphere = Instantiate(sideFlameObj.gameObject, other.transform.position, Quaternion.identity);
            sideFlameSphere.GetComponent<SideFlameSphere>().damage = damage;
            Destroy(sideFlameSphere, 0.5f);
            
            canBurn = false;
        }
    }
    
}
