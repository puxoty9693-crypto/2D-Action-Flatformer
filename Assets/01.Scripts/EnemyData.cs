using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyId;
    public GameObject enemyPrefab;
    public int poolSize = 5;
}
