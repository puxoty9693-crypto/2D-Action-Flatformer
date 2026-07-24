using UnityEngine;

public class MeteorProjectile : Projectile
{
    [SerializeField] private float hoverDuration = 3f;
    [SerializeField] private float impactRadius = 1.5f;
    [SerializeField] private float skyOffset = 8f;
    [SerializeField] private float horizontalOffset = 4f;


    private enum MeteorState { Hover, Falling }
    private MeteorState state;
    private float hoverTimer;
    private SpriteRenderer sr;

    public override void Init(Vector2 start, Vector2 target, int damage, float speed, float lifetime, LayerMask targetLayer, float arcHeight = 0f, float fixedAngle = 0f)
    {

        Vector2 correctedTarget = FindGroundPoint(target);

        base.Init(start, correctedTarget, damage, speed, lifetime, targetLayer, arcHeight, fixedAngle);

        state = MeteorState.Hover;
        hoverTimer = 0f;

        float direction = correctedTarget.x >= start.x ? -1f : 1f;
        Vector2 spawnPoint = new Vector2(correctedTarget.x + horizontalOffset * direction, correctedTarget.y + skyOffset);
        transform.position = spawnPoint;

        float travelDirX = correctedTarget.x - spawnPoint.x;


        if (sr != null) 
        {
            sr.flipX = travelDirX < 0;
        }

    }

    protected override void Move()
    {
        switch (state)
        {
            case MeteorState.Hover:
                hoverTimer += Time.deltaTime;
                if (hoverTimer >= hoverDuration)
                {
                    state = MeteorState.Falling;
                }
                break;

            case MeteorState.Falling:
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, targetPos) < 0.15f)
                {
                    Impact();
                }
                break;
        }
    }

    private Vector2 FindGroundPoint(Vector2 approxTarget) 
    {
        Vector2 rayStart = new Vector2(approxTarget.x, approxTarget.y + 10f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 20f, LayerMask.GetMask("Ground"));

        if (hit.collider != null) 
        {
            float halfHeight = GetComponent<SpriteRenderer>()?.bounds.extents.y ?? 0f;
            return hit.point + Vector2.up * halfHeight;
        }
        return approxTarget;
    }


    private void Impact()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, impactRadius, targetLayer);
        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            damageable?.TakeDamage(damage);
        }

        ReturnToPool();
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        state = MeteorState.Hover;
        hoverTimer = 0f;
    }



}
