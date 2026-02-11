using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI组件")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("显示文本")]
    public Text masterVolumeText;
    public Text musicVolumeText;
    public Text sfxVolumeText;

    [Header("按钮")]
    public Button resumeButton;
    public Button backToMenuButton;

    private bool isPaused = false;

    void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        resumeButton.onClick.AddListener(ResumeGame);
        backToMenuButton.onClick.AddListener(BackToMenu);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                OpenSettings();
        }
    }

    public void OpenSettings()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PauseMusic();
    }

    public void ResumeGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeMusic();
    }

    void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
        UpdateVolumeText(masterVolumeText, value);
    }

    void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        UpdateVolumeText(musicVolumeText, value);
    }

    void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        UpdateVolumeText(sfxVolumeText, value);
    }

    void UpdateVolumeText(Text textComponent, float value)
    {
        if (textComponent != null)
            textComponent.text = Mathf.RoundToInt(value * 100) + "%";
    }
}