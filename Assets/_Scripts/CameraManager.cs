using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public sealed class CameraManager : MonoBehaviour
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

    private void Start()
    {
        playCamPerlin = playCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            playCamPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
        }

        playTargetGroup.m_Targets[1].target = GameManager.Instance.playerSpecial.nearestEnemy != null ? GameManager.Instance.playerSpecial.nearestEnemy : null;
    }
    
    public void ShakeCam(float amount, float time)
    {
        playCamPerlin.m_AmplitudeGain = amount;

        startingIntensity = amount;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
}
