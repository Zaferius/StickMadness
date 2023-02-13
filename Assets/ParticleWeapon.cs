using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleWeapon : MonoBehaviour
{
    public float damage;
    public ParticleSystem shootParticle;
    void Start()
    {
        // shootParticle.transform.GetChild(1).GetComponent<FlamethrowerFire>().damage = damage;
    }
    
    void Update()
    {
        
    }
}
