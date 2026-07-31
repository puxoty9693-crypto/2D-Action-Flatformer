using UnityEngine;

public class RangedAttack : IAttackBehaviour
{
    private string projectileId;
    private LayerMask targetLayer;
    private float range;

    public RangedAttack(string projectileId, LayerMask targeLayer, float range = 8f)
    {
        this.projectileId = projectileId;
        this.targetLayer = targeLayer;
        this.range = range;
    }

    public void Attack(Vector3 spawnPos, Vector2 direction)
    {
        if (ProjectilePoolingManager.instance == null || string.IsNullOrEmpty(projectileId))
            return;
        ProjectileData data = ProjectilePoolingManager.instance.GetData(projectileId);
        if (data == null) return;

        GameObject projObj = ProjectilePoolingManager.instance.Spawn(projectileId, spawnPos, Quaternion.identity);
        Projectile proj = projObj?.GetComponent<Projectile>();
        if (proj != null)
        {
            Vector2 target = ResolveTarget(spawnPos, direction); 
            proj.Init(spawnPos, target, data.damage, data.speed, data.lifetime, targetLayer, data.arcHeight, data.fixedAngle);
        }
    }

    private Vector2 ResolveTarget(Vector3 spawnPos, Vector2 direction)
    {
        Collider2D nearest = Physics2D.OverlapCircle(spawnPos, range, targetLayer);
        if (nearest != null)
        {
            return nearest.transform.position;
        }
        return (Vector2)spawnPos + direction.normalized * range; 
    }
}