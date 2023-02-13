using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LandMine : MonoBehaviour
{
    public bool ready;

    public Transform landMineExplosiveArea;
    public MeshRenderer mineRedButton;
    void Start()
    {
        ready = false;
        
        TimeManager.Instance.transform.DOMoveX(0, 3f).OnComplete(() =>
        {
            ready = true;
            mineRedButton.material.color = Color.white;
            mineRedButton.material.DOColor(Color.red, 0.2f).SetLoops(-1, LoopType.Yoyo);
        });

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        landMineExplosiveArea.gameObject.SetActive(true);
        Destroy(Instantiate(ParticleManager.Instance.landMineExplosion, transform.position, Quaternion.identity), 2);
        TimeManager.Instance.transform.DOMoveX(0, 0.05f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
