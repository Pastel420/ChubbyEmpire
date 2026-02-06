using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
public class CameraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += onCameraShakeEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= onCameraShakeEvent;
    }

    private void onCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

    private void Start()
    {
        GetNewCameraBounds();
    }

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null)
            return;

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }
}
