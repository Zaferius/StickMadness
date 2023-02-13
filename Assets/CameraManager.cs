using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera playCam;
    public CinemachineTargetGroup playTargetGroup;
    private CinemachineBasicMultiChannelPerlin playCamPerlin;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    
    public static CameraManager Instance;
    private void InstanceMethod()
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

    private void Awake()
    {
        InstanceMethod();
    }

    void Start()
    {
        playCamPerlin = playCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            playCamPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
        }

        if (GameManager.Instance.playerSpecial.nearestEnemy != null)
        {
            playTargetGroup.m_Targets[1].target = GameManager.Instance.playerSpecial.nearestEnemy;
        }
        else
        {
            playTargetGroup.m_Targets[1].target = null;
        }
        
    }
    
    public void ShakeCam(float amount, float time)
    {
        playCamPerlin.m_AmplitudeGain = amount;

        startingIntensity = amount;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
}
