using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{

    [SerializeField] private float invincibleDuration = 1f;
    [SerializeField] private FormManager formManager;
    private bool isInvincible;


    public override void TakeDamage(int amount) 
    {
        if (isInvincible) return;

        base.TakeDamage(amount);

        if(amount > 0 && currentHP > 0) 
        {
            formManager?.PlayAnimation(PlayerState.DAMAGED, 0);
            StartCoroutine(InvincibleTime());
        }
    }

    private IEnumerator InvincibleTime() 
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    protected override void Die()
    {
        base.Die();
        formManager?.PlayAnimation(PlayerState.DEATH, 0);
    }
}
