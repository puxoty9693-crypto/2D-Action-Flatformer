using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class RangedAttack : IAttackBehaviour
{

    private string projectileId;
    private int damage;
    private LayerMask targeLayer;

    public RangedAttack(string projectileId, int damage, LayerMask targeLayer)
    {
        this.projectileId = projectileId;
        this.damage = damage;
        this.targeLayer = targeLayer;
    }

    public void Attack(Vector3 spawnPos, Vector2 direction) 
    {
        if (ProjectilePoolingManager.instance == null || string.IsNullOrEmpty(projectileId))
            return;

        ProjectileData data = ProjectilePoolingManager.instance.GetData(projectileId);
        if(data == null) return;

        
        GameObject projObj = ProjectilePoolingManager.instance.Spawn(projectileId,spawnPos,Quaternion.identity);
        Projectile proj = projObj?.GetComponent<Projectile>();

        if (proj != null)
        {
            Vector2 target = (Vector2)spawnPos + direction.normalized * 20f;
            proj.Init(spawnPos, target, damage, data.speed, data.lifetime, targeLayer, data.arcHeight, data.fixedAngle);
        }
    } 
    


   
}
