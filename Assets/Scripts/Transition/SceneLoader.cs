using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;  // 添加这个
using UnityEngine.ResourceManagement.ResourceProviders; // 添加这个
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public SceneLoadEventSO sceneLoadedEvent; // 新增：场景加载完成事件
    public GameSceneSO firstLoadScene;
    [SerializeField] private GameSceneSO currentLoadScene;

    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;

    public float fadeTime = 0.5f;

    [Header("玩家管理")]
    public GameObject player;  // 拖入 Persistent 的玩家


    [Header("淡入淡出")]
    private CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");

        // 初始隐藏玩家
        if (player != null)
            player.SetActive(false);

        currentLoadScene = firstLoadScene;
        var operation = currentLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        operation.Completed += (op) => OnFirstSceneLoaded();
    }

    public GameSceneSO GetCurrentScene()
    {
        return currentLoadScene;
    }
    private void SetupFadePanel()
    {
        GameObject fadeObj = GameObject.Find("FadePanel");
        if (fadeObj == null)
        {
            fadeCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        else
        {
            fadeCanvasGroup = fadeObj.GetComponent<CanvasGroup>();
        }
    }

    private void OnFirstSceneLoaded()
    {
        // 只有非Start场景才激活玩家
        if (currentLoadScene.sceneType != GameSceneSO.SceneType.Start &&
            currentLoadScene.sceneType != GameSceneSO.SceneType.Teach)
        {
            ActivatePlayer();
        }
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        if (currentLoadScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            yield return StartCoroutine(FadeOut());
        }

        yield return new WaitForSeconds(0.2f);

        var unloadOperation = currentLoadScene.sceneReference.UnLoadScene();
        yield return unloadOperation;

        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadOperation = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        // 使用lambda表达式
        loadOperation.Completed += (op) => OnNewSceneLoaded();
    }

    private void OnNewSceneLoaded()
    {
        currentLoadScene = sceneToLoad;

        // 先处理玩家（激活并设置位置）
        HandlePlayerBySceneType();

        // 延迟一帧再触发事件，确保玩家准备好
        StartCoroutine(DelayedSceneLoadedEvent());

        if (fadeScreen)
            StartCoroutine(FadeIn());
    }
    System.Collections.IEnumerator DelayedSceneLoadedEvent()
    {
        yield return null; // 等待一帧
        sceneLoadedEvent?.RaiseEvent(currentLoadScene);
    }
    System.Collections.IEnumerator DelayedPlayerSetup()
    {
        yield return null; // 等待一帧，让UI先显示

        HandlePlayerBySceneType();
    }
    private void HandlePlayerBySceneType()
    {
        switch (currentLoadScene.sceneType)
        {
            case GameSceneSO.SceneType.Start:
            case GameSceneSO.SceneType.Teach:
                // UI场景：隐藏玩家
                if (player != null)
                {
                    player.SetActive(false);
                    Debug.Log("进入UI场景，隐藏玩家");
                }
                break;

            case GameSceneSO.SceneType.Level:
            case GameSceneSO.SceneType.Boss:
            case GameSceneSO.SceneType.End:
                // 游戏场景：显示玩家并传送
                ActivatePlayer(positionToGo);
                break;
        }
    }
    private void ActivatePlayer(Vector3 spawnPosition)
    {
        if (player == null)
        {
            Debug.LogError("玩家引用为空！");
            return;
        }

        // 激活玩家
        player.SetActive(true);

        // 设置位置
        player.transform.position = spawnPosition;
        Debug.Log("玩家激活，位置: " + spawnPosition);

        // 重置物理状态
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.Sleep();
            rb.WakeUp();
            Debug.Log("玩家物理状态已重置");
        }

        // 重置动画（如果有）
        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }
    private void MovePlayerToPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = positionToGo;
        }
    }

    private void InitializeSceneByType()
    {
        // 使用currentLoadScene.sceneType而不是SceneType
        switch (currentLoadScene.sceneType)
        {
            case GameSceneSO.SceneType.Level:
            case GameSceneSO.SceneType.Boss:
                break;
            case GameSceneSO.SceneType.Start:
                if (PlayerDataManager.Instance != null)
                    PlayerDataManager.Instance.ResetAllData();
                break;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void ActivatePlayer()
    {
        if (player == null) return;

        player.SetActive(true);
        player.transform.position = positionToGo;

        // 重置物理
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.Sleep(); // 让物理休眠一帧
            rb.WakeUp();
        }
    }
}