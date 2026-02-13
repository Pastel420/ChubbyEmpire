// StartSettingsManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartSettingsManager : MonoBehaviour
{
    [Header("面板")]
    public GameObject startPanel;           // 开始界面面板
    public GameObject settingsPanel;        // 设置面板

    [Header("音量滑块")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("按钮")]
    public Button settingsButton;           // 打开设置
    public Button backButton;               // 返回开始界面

    void Start()
    {
        // 绑定按钮
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (backButton != null)
            backButton.onClick.AddListener(CloseSettings);

        // 绑定滑块
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 加载保存的设置
        LoadSettings();
    }

    void OpenSettings()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (startPanel != null)
            startPanel.SetActive(true);
    }

    void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMasterVolume(value);
    }

    void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }

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
    }
}