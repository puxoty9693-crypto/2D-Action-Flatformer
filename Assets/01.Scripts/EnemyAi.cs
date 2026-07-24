using UnityEngine;

public class EnemyAI : Enemy
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;

    protected override void OnAttack()
    {
        base.OnAttack();
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        if (player != null)
        {
            lastAttackTime = Time.time;
            PlayAttackAnimation();
        }
    }

    public void OnAttackHitFrame()
    {
        if (player == null)
        {
            return;
        }
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detector.AttackRange)
        {
            IDamageable damageable = player.GetComponent<IDamageable>();
            damageable?.TakeDamage(attackDamage);
        }

    }
}