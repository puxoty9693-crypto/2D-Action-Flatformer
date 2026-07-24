using UnityEngine;

public class ArcProjectile : Projectile
{

    protected override void Move()
    {
        float t = Mathf.Clamp01(elapsed / duration);

        Vector2 linearPos = Vector2.Lerp(startPos, targetPos, t);
        float heightOffset = arcHeight * Mathf.Sin(Mathf.PI * t);
        Vector2 newPos = linearPos + Vector2.up * heightOffset;

        Vector2 moveDir = newPos - (Vector2)transform.position;
        if(moveDir.sqrMagnitude > 0.0001f) 
        { 
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + fixedAngle);
        }

        transform.position = newPos;

        if (t >= 1f) 
        {
            ReturnToPool();
        }
    }
}
