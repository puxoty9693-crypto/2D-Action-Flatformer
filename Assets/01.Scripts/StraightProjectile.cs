using UnityEngine;

public class StraightProjectile : Projectile
{
    private Vector2 direction;

    public override void Init(Vector2 start, Vector2 target, int damage, float speed , float lifetime, LayerMask targetLayer, float arcHeight = 0f, float fixedAngle = 0f) 
    {
        base.Init(start, target, damage, speed, lifetime, targetLayer, arcHeight, fixedAngle);
        direction = (target - start).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle + fixedAngle);

    }

    protected override void Move()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
   
}
