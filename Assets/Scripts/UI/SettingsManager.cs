// SettingsManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject settingsPanel;        // 设置面板
    public Slider masterVolumeSlider;       // 主音量滑块
    public Slider musicVolumeSlider;        // 音乐音量滑块
    public Slider sfxVolumeSlider;          // 音效音量滑块
    public TextMeshProUGUI masterValueText; // 音量数值显示（可选）
    public TextMeshProUGUI musicValueText;
    public TextMeshProUGUI sfxValueText;
    public Button resumeButton;             // 继续游戏按钮
    public Button closeButton;              // 关闭按钮

    [Header("场景控制")]
    public SceneLoadEventSO loadEventSO;    // 返回主菜单用
    public GameSceneSO startScene;          // 主菜单场景

    private PlayerInputControl inputControl;
    private bool isInGameScene = false;     // 是否在关卡中

    void Awake()
    {
        inputControl = new PlayerInputControl();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    void OnEnable()
    {
        inputControl.Enable();
        inputControl.UI.Settings.performed += OnSettingsPerformed; // ESC键
    }

    void OnDisable()
    {
        inputControl.UI.Settings.performed -= OnSettingsPerformed;
        inputControl.Disable();
    }

    void Start()
    {
        // 加载保存的音量设置
        LoadSettings();

        // 绑定滑块事件
        SetupSliders();

        // 绑定按钮
        if (resumeButton != null)
            resumeButton.onClick.AddListener(CloseSettings);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
    }

    /// <summary>
    /// 按ESC键触发
    /// </summary>
    private void OnSettingsPerformed(InputAction.CallbackContext context)
    {
        // 检查当前场景类型
        var sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader != null)
        {
            var currentScene = sceneLoader.GetCurrentScene();
            if (currentScene != null)
            {
                // Start和Teach场景不响应ESC
                if (currentScene.sceneType == GameSceneSO.SceneType.Start ||
                    currentScene.sceneType == GameSceneSO.SceneType.Teach)
                {
                    return;
                }

                isInGameScene = currentScene.sceneType == GameSceneSO.SceneType.Level ||
                               currentScene.sceneType == GameSceneSO.SceneType.Boss ||
                               currentScene.sceneType == GameSceneSO.SceneType.End;
            }
        }

        ToggleSettings();
    }

    /// <summary>
    /// 从Start场景的设置按钮打开
    /// </summary>
    public void OpenSettingsFromMenu()
    {
        isInGameScene = false;
        OpenSettings();
    }

    void ToggleSettings()
    {
        if (settingsPanel.activeSelf)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }

    void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            // 根据场景显示/隐藏继续游戏按钮
            if (resumeButton != null)
            {
                resumeButton.gameObject.SetActive(isInGameScene);
            }

            // 关卡中暂停游戏
            if (isInGameScene)
            {
                Time.timeScale = 0f;
                AudioManager.Instance?.PauseMusic();
            }

            // 禁用玩家输入（关卡中）
            if (isInGameScene)
            {
                DisablePlayerInput();
            }
        }
    }

    void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);

            // 恢复游戏（关卡中）
            if (isInGameScene)
            {
                Time.timeScale = 1f;
                AudioManager.Instance?.ResumeMusic();

                // 恢复玩家输入
                EnablePlayerInput();
            }
        }
    }

    void DisablePlayerInput()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null && controller.inputControl != null)
            {
                controller.inputControl.Gameplay.Disable();
            }
        }
    }

    void EnablePlayerInput()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            if (controller != null && controller.inputControl != null && !controller.isDead)
            {
                controller.inputControl.Gameplay.Enable();
            }
        }
    }

    void SetupSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMasterVolume(value);
        if (masterValueText != null)
            masterValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        if (musicValueText != null)
            musicValueText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        if (sfxValueText != null)
            sfxValueText.text = Mathf.RoundToInt(value * 100) + "%";

        // 播放测试音效
        // AudioManager.Instance?.PlaySFX(testSound);
    }

    /// <summary>
    /// 加载保存的设置
    /// </summary>
    void LoadSettings()
    {
        if (AudioManager.Instance == null) return;

        float master = AudioManager.Instance.GetMasterVolume();
        float music = AudioManager.Instance.GetMusicVolume();
        float sfx = AudioManager.Instance.GetSFXVolume();

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = master;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = music;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfx;

        // 更新显示文本
        OnMasterVolumeChanged(master);
        OnMusicVolumeChanged(music);
        OnSFXVolumeChanged(sfx);
    }

    /// <summary>
    /// 返回主菜单（设置面板中的按钮）
    /// </summary>
    public void ReturnToMainMenu()
    {
        CloseSettings();

        // 确保时间恢复
        Time.timeScale = 1f;

        // 重置数据
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.ResetAllData();
        }

        // 切换场景
        if (loadEventSO != null && startScene != null)
        {
            loadEventSO.RaiseLoadRequestEvent(startScene, Vector3.zero, true);
        }
    }

    
}