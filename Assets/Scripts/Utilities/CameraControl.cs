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
    [Header("场景加载")]
    public SceneLoadEventSO sceneLoadedEvent; // 新增
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += onCameraShakeEvent;
        sceneLoadedEvent.OnSceneLoaded += OnSceneLoaded; // 订阅场景加载事件
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= onCameraShakeEvent;
        sceneLoadedEvent.OnSceneLoaded -= OnSceneLoaded; // 取消订阅
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

    // 每次场景加载完成后调用
    private void OnSceneLoaded(GameSceneSO scene)
    {
        Debug.Log("场景加载完成: " + scene.name + "，重新获取Camera Bounds");
        GetNewCameraBounds();
    }
}
