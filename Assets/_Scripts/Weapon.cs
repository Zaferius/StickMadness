using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Weapon : MonoBehaviour
{
    public enum WeaponClass
    {
       Ranged,
       Melee
    }

    public WeaponClass weaponClass;
    
    
    public float damage;
    public float shootForce;
    public float knockBackPower;
    public float range;
    public Transform firePoint;
    public GameObject pistolBullet;
    public bool recoil;
    public GameObject bulletShell;
    public Transform bulletShellPos;

    [Header("Melee")] 
    public GameObject trailObj;
    public Collider weaponCol;

    void Start()
    {
        if (weaponClass == WeaponClass.Melee)
        {
            weaponCol.enabled = false;
            trailObj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            Destroy(Instantiate(ParticleManager.Instance.hitBullet, other.transform.position, other.transform.rotation), 3);
            Destroy(Instantiate(ParticleManager.Instance.popcornHitEffect, transform.position, other.transform.rotation), 3);
            other.GetComponent<EnemySpecial>().TakeDamage(damage);
            CameraManager.Instance.ShakeCam(5f,0.2f);
        }
    }
}
