using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] protected LayerMask targetLayer;

    protected Vector2 startPos;
    protected Vector2 targetPos;
    protected float speed;
    protected int damage;
    protected float lifetime;
    protected float elapsed;
    protected float arcHeight;
    protected float duration;
    protected float fixedAngle;

    public virtual void Init(Vector2 start, Vector2 target, int damage, float speed, float lifetime, LayerMask targetLayer, float arcHeight = 0f, float fixedAngle = 0f) 
    {
        this.startPos = start;
        this.targetPos = target;
        this.speed = speed;
        this.lifetime = lifetime;
        this.targetLayer = targetLayer;
        this.elapsed = 0f;
        this.arcHeight = arcHeight;
        this.fixedAngle = fixedAngle;
        this.damage = damage;

        float distance = Vector2.Distance(start, target);
        this.duration = Mathf.Max(0.1f, distance/speed);


        transform.position = start;
    }

    protected virtual void Update() 
    {
        elapsed += Time.deltaTime;
        Move();

        if (elapsed >= lifetime) 
        {
            ReturnToPool();
        }
    }

    protected abstract void Move();

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();
        

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        ReturnToPool();
    }

    protected void ReturnToPool() 
    {
        if (ProjectilePoolingManager.instance != null) 
        {
            ProjectilePoolingManager.instance.Return(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
   
    public virtual void OnSpawn() 
    {
        elapsed = 0f;
    }

    public virtual void OnDespawn() 
    {
        
    }
  
}
