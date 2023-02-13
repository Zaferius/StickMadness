using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public sealed class PlayerSpecial : MonoBehaviour
{
    public Transform nearestEnemy;
    public float distance;

    [Header("HP")] 
    public float hp;
    private float startingHP;
    public Transform playerIndicator;

    [Header("Stance")] 
    public Transform expGoPos;
    public int level;

    [Header("Condition")] 
    public bool moving;
    public bool canShoot;
    private bool canSmoke;
    
    
    [Header("Settings")] 
    public float moveForce;
    

    [Header("Components")] 
    private Animator anim;
    private Rigidbody rb;
    public WeaponHolster weaponHolster;
    
    [Header("MeasuringSpeed")] 
    public float speed;
    private Vector3 oldPosition;
    private static readonly int Property = Animator.StringToHash("Input Magnitude");

    [Header("PlayerUI")] 
    public Transform hpBar;
    public Image hpImage;

    [Header("Playable Object Check")] 
    public bool playableObjectThicked;

    [Header("PlayerParts")] 
    public Transform RFootPos;
    public Transform LFootPos;

    private void Awake()
    {
        GameManager.Instance.playerSpecial = this;
    }


    void Start()
    {
        startingHP = hp;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
       HandleIndicator();
       HandleHPBar();
       HandleCombat();
       
        if (Input.GetMouseButtonDown(0))
        {
            if (speed < 0.1f)
            {
                RaycastToObjects();
            }
           
        }
        
      
    }

    private void FixedUpdate()
    {
        MeasureSpeed();
        
        if (Input.GetMouseButton(0))
        {
            HandleMovement();
        }
        else
        {
           PlayerStop();
        }

    }

    private void HandleMovement()
    {
        moving = true;
        ShootingAnimsSetter(false);
        weaponHolster.currentUnUsedWeapon.gameObject.SetActive(true);
        
        rb.velocity = new Vector3(UIManager.Instance.fj.Horizontal * (moveForce), rb.velocity.y,
            UIManager.Instance.fj.Vertical * (moveForce));

        Vector3 direction = Vector3.forward * UIManager.Instance.fj.Vertical + Vector3.right * UIManager.Instance.fj.Horizontal;
        rb.AddForce(direction * (moveForce) * Time.fixedDeltaTime, ForceMode.VelocityChange);
            
        var targetRotation = Quaternion.LookRotation(transform.forward);
        transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, targetRotation, 50 * Time.deltaTime);

        if (rb.velocity.magnitude > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(rb.velocity.x, 0f, rb.velocity.z)), Time.deltaTime * 60);
            anim.SetBool("Moving", true);
        }
        
        if (rb.velocity.magnitude > 3)
        {
            canSmoke = true;
        } else
        {
            canSmoke = false;
        }
        
        if (rb.velocity.magnitude > 1)
        {
            playableObjectThicked = false;
               
        }
        
    }

    private void HandleIndicator()
    {
        if (playerIndicator)
        {
            playerIndicator.transform.parent = null;
            playerIndicator.transform.localRotation = Quaternion.Euler(0,0,0);
            playerIndicator.transform.position = Vector3.Lerp(playerIndicator.transform.position, new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), Time.deltaTime * 35f);
        }
    }

    private void HandleCombat()
    {
        if(GameManager.Instance.currentLevel.enemies.Count > 0 && !playableObjectThicked)
        {
            FindNearestEnemy();
        }

        if (nearestEnemy)
        {
            distance = Mathf.Sqrt((transform.position - nearestEnemy.position).sqrMagnitude);
        }

        if (distance <= weaponHolster.currentWeapon.range && !moving && canShoot)
        {
            if (nearestEnemy != null )
            {
                rb.velocity = Vector3.zero;
                var targetRotation = Quaternion.LookRotation(nearestEnemy.transform.position - transform.position);
                transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, targetRotation, 60 * Time.deltaTime);

                ShootingAnimsSetter(true);

            }
            else
            {
                ShootingAnimsSetter(false);
            }
        }
        else
        {
            ShootingAnimsSetter(false);
        }
    }

    private void HandleHPBar()
    {
        if (hpBar)
        {
            hpImage.fillAmount = hp / startingHP;
            hpBar.transform.LookAt(Camera.main.transform);
        }
    }

    private void PlayerStop()
    {
        moving = false;
        rb.velocity = Vector3.zero;
        anim.SetBool("Moving", false);
    }

    void MeasureSpeed()
    {
        speed = Vector3.Magnitude(rb.velocity);
        
        if (speed > 0)
        {
            anim.SetFloat(Property, speed / 4);
        }
      
    }
    
    public void FindNearestEnemy()
    {
        float minimumDistance = 10;

        nearestEnemy = null;

        foreach (Transform enemy in GameManager.Instance.currentLevel.enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    nearestEnemy = enemy.transform;
                }
               
            }
        }
    }

    public void Shoot()
    {
        var bullet =  Instantiate(weaponHolster.currentWeapon.pistolBullet, weaponHolster.currentWeapon.firePoint.position, weaponHolster.currentWeapon.firePoint.transform.rotation);
        
        if (weaponHolster.currentWeapon.recoil)
        {
            weaponHolster.currentWeapon.firePoint.transform.localRotation = Quaternion.Euler(0, 90 + Random.Range(-5,5), 0);
        }
        
        bullet.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.firePoint.forward)* weaponHolster.currentWeapon.shootForce, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().damage = weaponHolster.currentWeapon.damage;
        Destroy(Instantiate(ParticleManager.Instance.muzzleFlash, weaponHolster.currentWeapon.firePoint.position, weaponHolster.currentWeapon.transform.rotation), 3);
        
        
        weaponHolster.currentWeapon.bulletShellPos.transform.localRotation = Quaternion.Euler(0, -180 + Random.Range(-40,40), 0);
        var bulletShell = Instantiate(weaponHolster.currentWeapon.bulletShell, weaponHolster.currentWeapon.bulletShellPos.position, Quaternion.identity);
        bulletShell.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.bulletShellPos.forward) * 1, ForceMode.Impulse);
        bulletShell.GetComponent<Rigidbody>().AddForce(Vector3.up * 4, ForceMode.Impulse);
        bulletShell.GetComponent<Rigidbody>().AddTorque(Random.Range(180,360),Random.Range(180,360),Random.Range(180,360),ForceMode.Impulse);
        Destroy(bulletShell, 8);

        CameraManager.Instance.ShakeCam(2f,0.2f);
        
    }

    public void ShotgunShoot()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                Quaternion testRot;
                testRot = weaponHolster.currentWeapon.firePoint.transform.localRotation = Quaternion.Euler(0, 90 - 20, 0);
                var bullet = Instantiate(weaponHolster.currentWeapon.pistolBullet, weaponHolster.currentWeapon.firePoint.position, testRot);
                bullet.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.firePoint.forward)* weaponHolster.currentWeapon.shootForce, ForceMode.Impulse);
                bullet.GetComponent<Bullet>().damage = weaponHolster.currentWeapon.damage;
                Destroy(Instantiate(ParticleManager.Instance.muzzleFlash, weaponHolster.currentWeapon.firePoint.position, Quaternion.identity), 3);
            }

            if (i == 1)
            {
                Quaternion testRot;
                testRot = weaponHolster.currentWeapon.firePoint.transform.localRotation = Quaternion.Euler(0, 90 - 10, 0);
                var bullet = Instantiate(weaponHolster.currentWeapon.pistolBullet, weaponHolster.currentWeapon.firePoint.position, testRot);
                bullet.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.firePoint.forward)* weaponHolster.currentWeapon.shootForce, ForceMode.Impulse);
                bullet.GetComponent<Bullet>().damage = weaponHolster.currentWeapon.damage;
                Destroy(Instantiate(ParticleManager.Instance.muzzleFlash, weaponHolster.currentWeapon.firePoint.position, Quaternion.identity), 3);
            }
            
            if (i == 2)
            {
                Quaternion testRot;
                testRot = weaponHolster.currentWeapon.firePoint.transform.localRotation = Quaternion.Euler(0, 90, 0);
                var bullet = Instantiate(weaponHolster.currentWeapon.pistolBullet, weaponHolster.currentWeapon.firePoint.position, testRot);
                bullet.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.firePoint.forward)* weaponHolster.currentWeapon.shootForce, ForceMode.Impulse);
                bullet.GetComponent<Bullet>().damage = weaponHolster.currentWeapon.damage;
                Destroy(Instantiate(ParticleManager.Instance.muzzleFlash, weaponHolster.currentWeapon.firePoint.position, Quaternion.identity), 3);
            }
            
            if (i == 3)
            {
                Quaternion testRot;
                testRot = weaponHolster.currentWeapon.firePoint.transform.localRotation = Quaternion.Euler(0, 90 + 10, 0);
                var bullet = Instantiate(weaponHolster.currentWeapon.pistolBullet, weaponHolster.currentWeapon.firePoint.position, testRot);
                bullet.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.firePoint.forward)* weaponHolster.currentWeapon.shootForce, ForceMode.Impulse);
                bullet.GetComponent<Bullet>().damage = weaponHolster.currentWeapon.damage;
                Destroy(Instantiate(ParticleManager.Instance.muzzleFlash, weaponHolster.currentWeapon.firePoint.position, Quaternion.identity), 3);
            }
            
            if (i == 4)
            {
                Quaternion testRot;
                testRot = weaponHolster.currentWeapon.firePoint.transform.localRotation = Quaternion.Euler(0, 90 + 20, 0);
                var bullet = Instantiate(weaponHolster.currentWeapon.pistolBullet, weaponHolster.currentWeapon.firePoint.position, testRot);
                bullet.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.firePoint.forward)* weaponHolster.currentWeapon.shootForce, ForceMode.Impulse);
                bullet.GetComponent<Bullet>().damage = weaponHolster.currentWeapon.damage;
                Destroy(Instantiate(ParticleManager.Instance.muzzleFlash, weaponHolster.currentWeapon.firePoint.position, Quaternion.identity), 3);
            }
           
        }
        
        CameraManager.Instance.ShakeCam(7f,0.2f);
    }

    public void ShotgunShellUp()
    {
        weaponHolster.currentWeapon.bulletShellPos.transform.localRotation = Quaternion.Euler(0, -180 + Random.Range(-40,40), 0);
        var bulletShell = Instantiate(weaponHolster.currentWeapon.bulletShell, weaponHolster.currentWeapon.bulletShellPos.position, Quaternion.identity);
        bulletShell.GetComponent<Rigidbody>().AddForce((weaponHolster.currentWeapon.bulletShellPos.forward) * 1, ForceMode.Impulse);
        bulletShell.GetComponent<Rigidbody>().AddForce(Vector3.up * 7, ForceMode.Impulse);
        bulletShell.GetComponent<Rigidbody>().AddTorque(Random.Range(180,360),Random.Range(180,360),Random.Range(180,360),ForceMode.Impulse);
        Destroy(bulletShell, 8);
    }

    public void FlamethrowerFire()
    {
        weaponHolster.flamethrowerParticle.Play();
    }
    
    //-MeleeSection

    public void MeleeStartVoid()
    {
        weaponHolster.currentWeapon.trailObj.SetActive(true);
    }

    public void MeleeEndVoid()
    {
        weaponHolster.currentWeapon.trailObj.SetActive(false);
    }

    public void MeleeHitCheck()
    {
        weaponHolster.currentWeapon.weaponCol.enabled = true;
    }

    public void MeleeHitCheckOff()
    {
        weaponHolster.currentWeapon.weaponCol.enabled = false;
    }

    public void GainExp(float expAmount)
    {
        UIManager.Instance.expBarCircle.fillAmount += expAmount / 100;
        if (UIManager.Instance.expBarCircle.fillAmount >= 1)
        {
            level++;
            UIManager.Instance.levelText.text = level.ToString();
            UIManager.Instance.expBarCircle.fillAmount = 0;

        }
    }

    public void ShootingAnimsSetter(bool isTrue)
    {
        if (isTrue)
        {
            if (weaponHolster.weaponType == WeaponHolster.WeaponType.Pistol)
            {
                anim.SetBool("PistolShooting",true);
            }
                
            if (weaponHolster.weaponType == WeaponHolster.WeaponType.SMG)
            {
                anim.SetBool("RifleShooting",true);
            }
            
            if (weaponHolster.weaponType == WeaponHolster.WeaponType.Axe)
            {
                anim.SetBool("AxeSlash1",true);
            }
            
            if (weaponHolster.weaponType == WeaponHolster.WeaponType.Shotgun)
            {
                anim.SetBool("ShotgunShooting",true);
            }
            
            if (weaponHolster.weaponType == WeaponHolster.WeaponType.Flamethrower)
            {
                anim.SetBool("FlamethrowerShooting",true);
            }
            
            weaponHolster.currentUnUsedWeapon.gameObject.SetActive(false);
            weaponHolster.currentWeapon.gameObject.SetActive(true);
        }
        else
        {
            
                anim.SetBool("PistolShooting",false);
                anim.SetBool("RifleShooting",false);
                anim.SetBool("ShotgunShooting",false);
                anim.SetBool("AxeSlash1",false);
                anim.SetBool("AxeSlash2",false);
                anim.SetBool("FlamethrowerShooting",false);
                
                weaponHolster.flamethrowerParticle.Stop();
                
                
                weaponHolster.currentUnUsedWeapon.gameObject.SetActive(true);
                weaponHolster.currentWeapon.gameObject.SetActive(false);
            
        }
    }

    public void RaycastToObjects()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform.CompareTag("ExplosiveBarrel"))
            {
                SetTarget(hit.transform);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        playableObjectThicked = true;
        nearestEnemy = target;
    }

    public void MoveSmokeL()
    {
        if (canSmoke)
        {
            Destroy(Instantiate(ParticleManager.Instance.moveSmoke, LFootPos.position, Quaternion.identity), 2);
        }
      
    }
    
    public void MoveSmokeR()
    {
        if (canSmoke)
        {
            Destroy(Instantiate(ParticleManager.Instance.moveSmoke, RFootPos.position, Quaternion.identity), 2);
        }
      
    }
    public void GainSoul(int soulAmount)
    {
        //Soul gain
    }
    
    
}
