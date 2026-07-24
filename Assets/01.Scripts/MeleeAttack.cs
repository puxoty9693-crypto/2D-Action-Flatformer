using UnityEngine;
using System.Collections;

public class MeleeAttack : IAttackBehaviour
{
    private WeaponHitbox hitbox;
    private int damage;
    private float activeDuration;
    private MonoBehaviour runner;

    public MeleeAttack(WeaponHitbox hitbox, int damage, float activeDuration,MonoBehaviour runner) 
    {
        this.hitbox = hitbox;
        this.damage = damage;
        this.activeDuration = activeDuration;
        this.runner = runner;
    }

    public void Attack(Vector3 orign, Vector2 direction) 
    {
        if (hitbox == null) 
            return;
        runner.StartCoroutine(ActivateHitBox());

    }

    private IEnumerator ActivateHitBox() 
    {

        hitbox.Activate(damage);
        yield return new WaitForSeconds(activeDuration);
        hitbox.Deactivate();
    }

}
