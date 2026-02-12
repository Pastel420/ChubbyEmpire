using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    [Header("事件")]
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    [Header("死亡事件")]
    public PlayerDeathEventSO deathEvent;  // 新增：死亡事件

    private void Start()
    {
        currentHealth = maxHealth;

        // 同步到 PlayerDataManager
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.currentHealth = currentHealth;
        }
        // 初始触发一次，更新UI
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            { 
                invulnerable = false;
            }
        }
    }
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;

            // 同步到 PlayerDataManager
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.currentHealth = currentHealth;
            }

            TriggerInvulnerable();

            OnTakeDamage?.Invoke(attacker.transform);
            OnHealthChange?.Invoke(this);
        }
        else
        {
            currentHealth = 0;//死了

            // 同步到 PlayerDataManager
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.currentHealth = 0;
            }

            OnDie?.Invoke();
            // 触发死亡事件
            deathEvent?.RaiseEvent();
            OnHealthChange?.Invoke(this);  // 触发血量变化（显示0血）
        }


    }

    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
}
