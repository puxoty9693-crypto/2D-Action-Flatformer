using UnityEngine;


[CreateAssetMenu(fileName = "ProjectileData", menuName = "Game/ProjectileData")]

public class ProjectileData : ScriptableObject
{
    public string projectileId;
    public GameObject projectilePrefab;
    public int poolSize = 10;

    public int damage = 10;
    public float speed = 8f;
    public float lifetime = 5f;
    public float arcHeight = 0f;
    public float fixedAngle = 225f;
}
