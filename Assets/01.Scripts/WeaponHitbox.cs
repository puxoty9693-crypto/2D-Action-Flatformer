using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    private int damage;
    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();
    private Collider2D myCollider;


    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        myCollider.enabled = false;
    }

    public void Activate(int dmg)
    {
        damage = dmg;
        hitTargets.Clear();
        myCollider.enabled = true;

    }

    public void Deactivate()
    {
        myCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitTargets.Contains(collision))
            return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            hitTargets.Add(collision);
        }
    }
}