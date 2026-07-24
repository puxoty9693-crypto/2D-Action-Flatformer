using System;
using UnityEngine;

public abstract class Health : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHP = 100;
    [SerializeField] protected int currentHP;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    protected virtual void Awake() 
    {
        currentHP = maxHP;
    }
    public virtual void TakeDamage(int amount) 
    {
        if (currentHP <= 0) return;
        
            currentHP -= amount;
            currentHP = Math.Max(currentHP, 0);

            OnHealthChanged?.Invoke(currentHP, maxHP);
        
        if(currentHP <= 0) 
        {
            Die();
        }
    }

    public virtual void SetMaxHP(int newMaxHP) 
    {
        if(newMaxHP <= 0) 
        {
            return;
        }
        
        float ratio = maxHP > 0 ? (float)currentHP / maxHP : 1f;

        maxHP = newMaxHP;
        currentHP = Mathf.RoundToInt(maxHP * ratio);
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        OnHealthChanged?.Invoke(currentHP, maxHP);

    }

    protected virtual void Die() 
    {
        OnDeath?.Invoke();
    }
    public int GetCurrentHP() => currentHP;
    public int GetMaxHP() => maxHP;
    
}
