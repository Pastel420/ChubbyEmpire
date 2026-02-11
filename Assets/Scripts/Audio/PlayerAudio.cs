using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("玩家音效")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip jumpSound;
    public AudioClip itemSound;
    public AudioClip doorSound;

    

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAttackSound()
    {
        PlaySound(attackSound);
    }

    public void PlayHurtSound()
    {
        PlaySound(hurtSound);
    }

    public void PlayJumpSound()
    {
        PlaySound(jumpSound);
    }

    public void PlayItemSound()
    {
        PlaySound(itemSound);
    }


    public void PlayDoorSound()
    {
        PlaySound(doorSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}