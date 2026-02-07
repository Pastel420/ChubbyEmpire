using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    [Header("ÊÂ¼þ¼àÌý")]
    public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;

    public AudioSource BGMSource;
    public AudioSource FXSource;

    private void OnEnable()
    {

     
    }
}
