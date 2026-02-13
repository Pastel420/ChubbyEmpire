using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("背景音乐")]
    public AudioClip plainsMusic;
    public AudioClip castleMusic;
    public AudioClip menuMusic;

    [Header("音频混合器")]
    public AudioMixer audioMixer;

    [Header("参数名称")]
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private string currentMusic = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.outputAudioMixerGroup = GetMixerGroup("Music"); // 确保走Mixer

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.outputAudioMixerGroup = GetMixerGroup("SFX");
    }

    AudioMixerGroup GetMixerGroup(string groupName)
    {
        if (audioMixer == null) return null;
        return audioMixer.FindMatchingGroups(groupName)[0];
    }

    void Start()
    {
        LoadVolumeSettings(); // 加载保存的音量
        PlayMusic("menu");
    }

    public void PlayMusic(string sceneType)
    {
        if (currentMusic == sceneType) return;

        AudioClip clip = null;
        switch (sceneType.ToLower())
        {
            case "menu":
                clip = menuMusic;
                break;
            case "plains":
            case "mountain":
                clip = plainsMusic;
                break;
            case "castle":
            case "boss":
                clip = castleMusic;
                break;
        }

        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
            currentMusic = sceneType;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        currentMusic = "";
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 设置主音量 (0-1)
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        // 线性转dB
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(masterVolumeParam, dB);
        }

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save(); // 立即保存
    }

    /// <summary>
    /// 设置音乐音量 (0-1)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(musicVolumeParam, dB);
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 设置音效音量 (0-1)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(sfxVolumeParam, dB);
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 加载保存的音量设置
    /// </summary>
    public void LoadVolumeSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 应用设置（不触发保存）
        if (audioMixer != null)
        {
            float masterDB = master > 0.0001f ? 20f * Mathf.Log10(master) : -80f;
            float musicDB = music > 0.0001f ? 20f * Mathf.Log10(music) : -80f;
            float sfxDB = sfx > 0.0001f ? 20f * Mathf.Log10(sfx) : -80f;

            audioMixer.SetFloat(masterVolumeParam, masterDB);
            audioMixer.SetFloat(musicVolumeParam, musicDB);
            audioMixer.SetFloat(sfxVolumeParam, sfxDB);
        }

        Debug.Log($"加载音量设置 - Master:{master}, Music:{music}, SFX:{sfx}");
    }

    /// <summary>
    /// 获取当前音量（用于UI显示）
    /// </summary>
    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 1f);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
}