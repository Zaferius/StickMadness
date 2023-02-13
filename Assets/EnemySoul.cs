using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemySoul : MonoBehaviour
{
    public float soulAmount;
    public bool magnetActive;
    public bool canGo;
    public Transform target;
    public Transform targetGoPos;
    public float force;
    public float distance;
    private Rigidbody rb;
  
    void Start()
    {
        target = GameManager.Instance.playerSpecial.expGoPos.transform;
        rb = GetComponent<Rigidbody>();
        canGo = false;
    }
    
    void Update()
    {
        distance = (transform.position - target.transform.position).sqrMagnitude;
        if (distance < 200 && !magnetActive)
        {
            magnetActive = true;
        }
        else if (distance < 0.5f)
        {
            GiveExpPoint();
        }
        
        if (magnetActive && canGo)
        {
            // rb.AddForce((target.transform.position - transform.position) * force * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, force * Time.deltaTime);

        }
    }

    private void FixedUpdate()
    {
       
    }

    public void GiveExpPoint()
    {
        GameManager.Instance.playerSpecial.GainExp(soulAmount);
        Destroy(gameObject);
    }
}
