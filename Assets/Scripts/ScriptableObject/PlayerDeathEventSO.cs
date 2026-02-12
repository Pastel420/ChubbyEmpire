// PlayerDeathEventSO.cs
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/PlayerDeathEventSO")]
public class PlayerDeathEventSO : ScriptableObject
{
    public UnityAction OnPlayerDeath;

    public void RaiseEvent()
    {
        OnPlayerDeath?.Invoke();
    }
}