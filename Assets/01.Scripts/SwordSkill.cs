using UnityEngine;

public class SwordSkill : Projectile
{
    [SerializeField] private float hoverDuration = 3f;
    [SerializeField] private float impactRadius = 1.5f;
    [SerializeField] private float skyOffset = 8f;
    [SerializeField] private float horizontalOffset = 4f;
    [SerializeField] private float damagePercent = 0.15f;
    [SerializeField] private float groundOffset = 0.3f;
    [SerializeField] private float safetyTimeout = 20f;


    private enum BigSwordState { Hover, Falling }
    private BigSwordState state;
    private float hoverTimer;
    private SpriteRenderer sr;
    private float safetyTimer;

    public override void Init(Vector2 start, Vector2 target, int damage, float speed, float lifetime, LayerMask targetLayer, float arcHeight = 0f, float fixedAngle = 0f)
    {
        Vector2 correctedTarget = FindGroundPoint(target);
        base.Init(start, correctedTarget, damage, speed, lifetime, targetLayer, arcHeight, fixedAngle);
        state = BigSwordState.Hover;
        hoverTimer = 0f;
        safetyTimer = 0f;
        float direction = correctedTarget.x >= start.x ? -1f : 1f;
        Vector2 spawnPoint = new Vector2(correctedTarget.x + horizontalOffset * direction, correctedTarget.y + skyOffset);
        transform.position = spawnPoint;
        float travelDirX = correctedTarget.x - spawnPoint.x;
        if (sr != null)
        {
            sr.flipX = travelDirX < 0;
        }
    }

    protected override void Update()
    {
        safetyTimer += Time.deltaTime;
        Move();

        if(safetyTimer >= safetyTimeout) 
        {
            ReturnToPool();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        
    }


    protected override void Move()
    {
        switch (state)
        {
            case BigSwordState.Hover:
                hoverTimer += Time.deltaTime;
                if (hoverTimer >= hoverDuration)
                {
                    state = BigSwordState.Falling;
                }
                break;
            case BigSwordState.Falling:
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
        int mask = LayerMask.GetMask("Ground");
       

        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, 50f, mask);
       

        if (hit.collider != null)
        {
            return hit.point + Vector2.up * groundOffset;
        }
        return approxTarget;
    }

    private void Impact()
    {
        

        Collider2D[] allNearby = Physics2D.OverlapCircleAll(transform.position, impactRadius);
        
        

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, impactRadius, targetLayer);
        

        foreach (var hit in hits)
        {
            Health health = hit.GetComponentInParent<Health>();
            
            if (health != null)
            {
                int calculatedDamage = Mathf.RoundToInt(health.GetMaxHP() * damagePercent);
                
                health.TakeDamage(calculatedDamage);
            }
        }
        ReturnToPool();
    }


    public override void OnSpawn()
    {
        base.OnSpawn();
        state = BigSwordState.Hover;
        hoverTimer = 0f;
        safetyTimer = 0f;
    }
}
