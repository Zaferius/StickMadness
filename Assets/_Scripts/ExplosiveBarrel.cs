using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public sealed class ExplosiveBarrel : MonoBehaviour
{
    public float hp;
    public GameObject explosiveArea;
    private Vector3 defScale;

    private void Start()
    {
        defScale = transform.localScale;
    }

    public void TakeDamage(float amount)
    {
        if (!(hp > 0)) return;
        hp -= amount;
            
        transform.DOPunchScale(new Vector3(0.2f,-0.2f,0.2f), 0.1f).OnComplete(() =>
        {
            transform.DOScale(defScale, 0.1f);
        });

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Explode();
    }
    
    private void Explode()
    {
        explosiveArea.gameObject.SetActive(true);
        Destroy(Instantiate(ParticleManager.Instance.barrelExplosion, transform.position, Quaternion.identity), 2);
        TimeManager.Instance.transform.DOMoveX(0, 0.05f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
