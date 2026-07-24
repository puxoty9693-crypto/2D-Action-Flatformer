using UnityEngine;

public class ArcherSkill : MonoBehaviour, IAttackBehaviour
{
    [SerializeField] private string projectileId;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float range = 15f;
    [SerializeField] private LayerMask targetLayer;

    private float lastFireTime = -999f;

    public bool IsFiring { get; private set; }

    public bool CanUse()
    {
        return Time.time >= lastFireTime + fireRate;
    }

    public void Attack(Vector3 spawnPos, Vector2 direction) 
    {
        if(!CanUse()) return;

        ProjectileData data = ProjectilePoolingManager.instance.GetData(projectileId);
        if(data == null) return;

        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        Vector2 targetPos = (Vector2)spawnPos + dir * range;

        GameObject obj = ProjectilePoolingManager.instance.Spawn(projectileId, spawnPos, Quaternion.identity);
        if (obj == null) return;

        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.Init(spawnPos, targetPos, data.damage, data.speed, data.lifetime, targetLayer, arcHeight: 0f, fixedAngle: data.fixedAngle);

        lastFireTime = Time.time;
    }
}
