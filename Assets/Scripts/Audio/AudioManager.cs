using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("±≥æ∞“Ù¿÷")]
    public AudioClip plainsMusic;
    public AudioClip castleMusic;
    public AudioClip menuMusic;

    [Header("“Ù∆µªÏ∫œ∆˜")]
    public AudioMixer audioMixer;

    [Header("≤Œ ˝√˚≥∆")]
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

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    void Start()
    {
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

    public void SetMasterVolume(float volume)
    {
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(masterVolumeParam, dB);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(musicVolumeParam, dB);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(sfxVolumeParam, dB);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void LoadVolumeSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1);
    }
}