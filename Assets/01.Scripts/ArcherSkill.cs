using NUnit.Framework.Interfaces;
using System.Collections;
using UnityEngine;

public class ArcherSkill : IAttackBehaviour
{
    private string projectileId;
    private LayerMask targetLayer;
    private float range;
    private int burstCount;
    private float burstInterval;
    private MonoBehaviour runner;

    public ArcherSkill(string projectileId, LayerMask targetLayer, float range, int burstCount, float burstInterval, MonoBehaviour runner)
    {
        this.projectileId = projectileId;
        this.targetLayer = targetLayer;
        this.range = range;
        this.burstCount = burstCount;
        this.burstInterval = burstInterval;
        this.runner = runner;
    }

    public void Attack(Vector3 spawnPos, Vector2 direction)
    {
        runner.StartCoroutine(BurstRoutine(spawnPos, direction));
    }

    private IEnumerator BurstRoutine(Vector3 spawnPos, Vector2 direction)
    {
        for (int i = 0; i < burstCount; i++)
        {
            FireOne(spawnPos, direction);
            yield return new WaitForSeconds(burstInterval);
        }
    }

    private void FireOne(Vector3 spawnPos, Vector2 direction)
    {
        if (ProjectilePoolingManager.instance == null || string.IsNullOrEmpty(projectileId))
            return;

        ProjectileData data = ProjectilePoolingManager.instance.GetData(projectileId);
        if (data == null) return;

        Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        Vector2 targetPos = (Vector2)spawnPos + dir * range;

        GameObject projObj = ProjectilePoolingManager.instance.Spawn(projectileId, spawnPos, Quaternion.identity);
        if (projObj == null) return;

        Projectile projectile = projObj.GetComponent<Projectile>();
        projectile?.Init(spawnPos, targetPos, data.damage, data.speed, data.lifetime, targetLayer, arcHeight: 0f, fixedAngle: data.fixedAngle);
    }


}
