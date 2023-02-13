using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemySpecial : MonoBehaviour
{
    public enum EnemyType
    {
        CommonZombie,
        FastZombie
    }
    public enum EnemyStance
    { 
        Patrolling,
        Idleing,
        Chasing,
        Dead
       
    }
    public EnemyStance enemyStance;
    public EnemyType enemyType;

    public bool isAlive;
    public float hp;
    public bool onMove;


    [Header("AI")]
    public float distanceToPlayer;
    public float patrolTime;
    public float patrolTimer;
    public Transform target;
    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    public EnvPieceSpecial currentEnvironment;
    
    [Header("Components")] 
    private Animator anim;
    [Header("BodyParts")]
    public Color defColor;
    private Vector3 defScale;
    public SkinnedMeshRenderer bodyMeshRenderer;
    public Rigidbody spine;
    [Header("HP")] 
    private float startingHP;
    public Transform hpBar;
    public Image hpImage;
    public float hpBarTimer;
    [Header("Exp")] 
    public GameObject soulObj;
    public float expAmount;
    [Header("Burning")]
    public bool burning;
    public ParticleSystem burningParticle;
    public float burnDamage;

    public float burningHitTime;
    public float burningHitTimer;
    
    public float burningGoTime;
    public float burningGoTimer;
    void Start()
    {
        patrolTime = Random.Range(20, 25);
        
        startingHP = hp;
        
        defColor = bodyMeshRenderer.material.color;
        defScale = transform.localScale;
        
        
        
        anim = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        
        TimeManager.Instance.transform.DOMoveX(0, 0.1f).OnComplete(() =>
        {
            GameManager.Instance.currentLevel.enemies.Add(transform);
            // target = GameManager.Instance.playerSpecial.transform;
            // destinationSetter.target = target;
        });
        
        foreach (Rigidbody childRb in GetComponentsInChildren<Rigidbody>(true))
        {
            childRb.isKinematic = true;
        }
        
        
        foreach (Collider childCol in GetComponentsInChildren<Collider>(true))
        {
            childCol.enabled = false;
        }

        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;



    }
    
    void Update()
    {
        if (isAlive)
        {
            if (burning)
            {
                burningGoTimer -= 5 * Time.deltaTime;
                burningHitTimer -= 5 * Time.deltaTime;

                if (!burningParticle.isPlaying)
                {
                    burningParticle.Play();
                }
                
                if (burningGoTimer <= 0)
                { 
                    burning = false;
                    burningGoTimer = burningGoTime;
                }
                
                if (burningHitTimer <= 0)
                { 
                    TakeDamage(burnDamage);
                    burningHitTimer = burningHitTime;
                }
            }
            else
            {
                if (burningParticle.isPlaying)
                {
                    burningParticle.Stop();
                    burningHitTimer = burningHitTime;
                    burningGoTimer = burningGoTime;
                }
            }
            
            
            distanceToPlayer = (transform.position - GameManager.Instance.playerSpecial.transform.position).sqrMagnitude;
        
            if (distanceToPlayer <= 75 && enemyStance != EnemyStance.Chasing)
            {
                enemyStance = EnemyStance.Chasing;
                StartChasing();
            }
            else if (distanceToPlayer > 125 && enemyStance == EnemyStance.Chasing)
            {
                FreeThink();
                enemyStance = EnemyStance.Patrolling;
            }
        
        
        
            if (enemyStance == EnemyStance.Patrolling)
            {
                patrolTimer -= Time.deltaTime * 5;
                if (patrolTimer <= 0 || aiPath.reachedDestination)
                {
                    FreeThink();
                }
            }
        
            if (onMove)
            {
                switch (enemyType)
                {
                    case EnemyType.CommonZombie:
                        anim.SetBool("ZombieWalk", true);
                        break;
                    case EnemyType.FastZombie:
                        anim.SetBool("ZombieRun", true);
                        break;
                }
            }
            else
            {
                switch (enemyType)
                {
                    case EnemyType.CommonZombie:
                        anim.SetBool("ZombieWalk", false);
                        break;
                    case EnemyType.FastZombie:
                        anim.SetBool("ZombieRun", false);
                        break;
                }
            }

            if (hpBar)
            {
                hpImage.fillAmount = hp / startingHP;
                hpBar.transform.LookAt(Camera.main.transform);

                if (hpBarTimer < 0)
                {
                    hpBar.gameObject.SetActive(false);
                }
                else
                {
                    hpBarTimer -= Time.deltaTime * 10;
                }
            }
        }
      
    }

    private void FreeThink()
    {
        int randomInt = Random.Range(0, 2);
        if (randomInt == 0)
        {
            aiPath.enabled = true;
            onMove = true;
            destinationSetter.target = currentEnvironment.points[Random.Range(0, currentEnvironment.points.Count)];
        }
        else
        {
            onMove = false;
            aiPath.enabled = false;
        }
        
        patrolTimer = patrolTime;
       
    }
    
    public void TakeDamage(float damage)
    {
        if (hp > 0)
        {
            hp -= damage;
            
            GetComponent<Rigidbody>().AddForce(-transform.forward * GameManager.Instance.playerSpecial.weaponHolster.currentWeapon.knockBackPower / 5, ForceMode.Impulse);
        
            bodyMeshRenderer.material.DOColor(Color.white, 0f).OnComplete(() =>
            {
                bodyMeshRenderer.material.DOColor(defColor, 0.1f);
            });
        
            transform.DOPunchScale(new Vector3(0.05f,-0.05f,0.05f), 0.1f).OnComplete(() =>
            {
                transform.DOScale(defScale, 0.1f);
            });

            HpBarActivate();

            if (hp <= 0)
            {
                Die();
            }
        }
    }

    public void Burn()
    {
        burning = true;
    }

    private void StartChasing()
    {
        aiPath.enabled = true;
        onMove = true;
        destinationSetter.target = GameManager.Instance.playerSpecial.transform;
        print("ChasePlayer!");
    }

    private void Die()
    {
        currentEnvironment.enemyLeft--;
        if (currentEnvironment.enemyLeft <= 0)
        {
            currentEnvironment.Cleared();
        }
        
        if (transform.GetChild(0).GetComponent<OutlineQ>())
        {
            Destroy(transform.GetChild(0).GetComponent<OutlineQ>());
        }

        
        
        
        Destroy(hpBar.gameObject);
        
        gameObject.layer = 7;

        hp = 0;
        
        foreach (Transform child in GetComponentsInChildren<Transform>(true))  
        {
            child.gameObject.layer = 7;
        }
        
        foreach (Collider childCol in GetComponentsInChildren<Collider>(true))
        {
            childCol.enabled = true;
        }
        
        foreach (Rigidbody childRb in GetComponentsInChildren<Rigidbody>(true))
        {
            childRb.isKinematic = false;
        }

        isAlive = false;
        enemyStance = EnemyStance.Dead;
        GameManager.Instance.currentLevel.enemies.Remove(transform);
        GameManager.Instance.playerSpecial.nearestEnemy = null;
        
        if (burning)
        {
            TimeManager.Instance.transform.DOMoveX(0, 3f).OnComplete(() =>
            {
               burningParticle.Stop();
            });
            
            spine.AddForce(-transform.forward * 15, ForceMode.Impulse);
        }
        else
        {
            spine.AddForce(-transform.forward * GameManager.Instance.playerSpecial.weaponHolster.currentWeapon.knockBackPower, ForceMode.Impulse);
        }
        
        spine.AddForce(transform.right * Random.Range(-150,150), ForceMode.Impulse);
        anim.enabled = false;
        Destroy(aiPath);
        Destroy(destinationSetter);
        Destroy(GetComponent<Collider>());
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        Destroy(GetComponent<Rigidbody>());
        bodyMeshRenderer.material.DOColor(Color.gray, 0.2f);

        CreateExpSouls(2);
        
    }

    private void CreateExpSouls(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var soul = Instantiate(soulObj, transform.position, Quaternion.identity);
            soul.transform.DOLocalMove(new Vector3(transform.position.x + Random.Range(-1f, 1f), transform.position.y + Random.Range(2, 4), transform.position.z + Random.Range(-1f, 1f)), 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                soul.GetComponent<EnemySoul>().canGo = true;
            });
        }
    }

    private void HpBarActivate()
    {
        hpBarTimer = 30;
        hpBar.gameObject.SetActive(true);
    }
    
}
