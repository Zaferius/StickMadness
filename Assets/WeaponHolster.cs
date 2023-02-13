using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public sealed class WeaponHolster : MonoBehaviour
{
    public Weapon currentWeapon;
    public GameObject currentUnUsedWeapon;
    [Header("Weapons")] 
    public List<Transform> weapons = new List<Transform>();
    public List<GameObject> unUsedWeapons = new List<GameObject>();
    [Header("ParticleWeapons")] 
    public ParticleSystem flamethrowerParticle;
    public enum WeaponType
    {
        Axe,
        Pistol,
        SMG,
        Shotgun,
        Flamethrower
        
    }

    public WeaponType weaponType;
    void Start()
    {
       SetStartingWeapon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(3);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchWeapon(4);
        }
    }

    private void SetStartingWeapon()
    {
        foreach (var weapon in weapons)
        {
          weapon.gameObject.SetActive(false);  
        }
        
        switch (weaponType)
        {
            case WeaponType.Axe:
                currentWeapon = weapons[0].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(0);
                break;
            case WeaponType.Pistol:
                currentWeapon = weapons[1].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(1);
                break;
            case WeaponType.SMG:
                currentWeapon = weapons[2].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(2);
                break;
            case WeaponType.Shotgun:
                currentWeapon = weapons[3].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(3);
                weaponType = WeaponType.Shotgun;
                break;
            case WeaponType.Flamethrower:
                currentWeapon = weapons[4].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(4);
                weaponType = WeaponType.Flamethrower;
                break;
        }
        
        currentWeapon.gameObject.SetActive(true);
    }

    private void SwitchWeapon(int weaponIndex)
    {
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);  
        }
        
        switch (weaponIndex)
        {
            case 0:
                currentWeapon = weapons[0].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(0);
                weaponType = WeaponType.Axe;
                break;
            case 1:
                currentWeapon = weapons[1].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(1);
                weaponType = WeaponType.Pistol;
                break;
            case 2:
                currentWeapon = weapons[2].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(2);
                weaponType = WeaponType.SMG;
                break;
            case 3:
                currentWeapon = weapons[3].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(3);
                weaponType = WeaponType.Shotgun;
                break;
            case 4:
                currentWeapon = weapons[4].GetComponent<Weapon>();
                SetUnUsedWeaponObjects(4);
                weaponType = WeaponType.Flamethrower;
                break;
        }
        
        currentWeapon.gameObject.SetActive(true);
        GameManager.Instance.playerSpecial.ShootingAnimsSetter(false);
      
    }

    private void SetUnUsedWeaponObjects(int weaponIndex)
    {
        foreach (var unWeapon in unUsedWeapons)
        {
            unWeapon.gameObject.SetActive(false);
        }
        
        unUsedWeapons[weaponIndex].gameObject.SetActive(true);
        currentUnUsedWeapon = unUsedWeapons[weaponIndex].gameObject;
    }
}
