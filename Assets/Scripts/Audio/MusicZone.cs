using UnityEngine;

public class MusicZone : MonoBehaviour
{
    [Header("场景音乐类型")]
    [Tooltip("menu-菜单, plains-平原/山地, castle-城堡/Boss")]
    public string musicType = "plains";

    [Header("触发方式")]
    public bool triggerOnStart = true;
    public bool triggerOnEnter = false;

    void Start()
    {
        if (triggerOnStart && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(musicType);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnEnter && other.CompareTag("Player") && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(musicType);
        }
    }
}