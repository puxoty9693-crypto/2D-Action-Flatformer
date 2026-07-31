using System.Collections;
using UnityEngine;

public class EnemyAI : Enemy
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackHitDelay = 0.3f;


    private float lastAttackTime;
    private Coroutine attackHitCoroutine;

    protected override void OnAttack()
    {
        base.OnAttack();
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        if (player != null)
        {
            lastAttackTime = Time.time;
            PlayAttackAnimation();

            if (attackHitCoroutine != null)
                StopCoroutine(attackHitCoroutine);
            attackHitCoroutine = StartCoroutine(AttackHitFrameCoroutine());
        }
    }

    private IEnumerator AttackHitFrameCoroutine() 
    {
        yield return new WaitForSeconds(attackHitDelay);
        OnAttackHitFrame();
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