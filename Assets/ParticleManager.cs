using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    public GameObject muzzleFlash;
    public GameObject hitBullet;
    public GameObject popcornHitEffect;
    public GameObject landMineExplosion;
    public GameObject barrelExplosion;
    public GameObject moveSmoke;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
