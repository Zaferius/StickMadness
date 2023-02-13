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

    private void Start()
    {
        canBurn = true;
        rateOfFlame = rateOfFlameTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (canBurn) return;
        rateOfFlame -= 5 * Time.deltaTime;
        if (!(rateOfFlame <= 0)) return;
        rateOfFlame = rateOfFlameTime;
        canBurn = true;
    }
    
    void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Zombie") || !canBurn) return;
        var position = other.transform.position;
        Destroy(Instantiate(ParticleManager.Instance.hitBullet, position, other.transform.rotation), 3);
        other.gameObject.GetComponent<EnemySpecial>().TakeDamage(damage);
        other.gameObject.GetComponent<EnemySpecial>().Burn();
            
            
        var sideFlameSphere = Instantiate(sideFlameObj.gameObject, position, Quaternion.identity);
        sideFlameSphere.GetComponent<SideFlameSphere>().damage = damage;
        Destroy(sideFlameSphere, 0.5f);
            
        canBurn = false;
    }
    
}
