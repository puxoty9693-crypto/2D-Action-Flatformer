using UnityEngine;

public class EnemyHealth : Health, IPoolable
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }

    public override void TakeDamage(int amount)
    {
        int before = currentHP;
        base.TakeDamage(amount);
        if (currentHP < before && currentHP > 0)
            enemy?.PlayDamage();
    }

    protected override void Die()
    {
        enemy?.PlayDeath();
        base.Die();
        EnemyPoolingManager.instance.Return(gameObject);
    }

    public void OnSpawn() => currentHP = maxHP;
    public void OnDespawn() { }
}
