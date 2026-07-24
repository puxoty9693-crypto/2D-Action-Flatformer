using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnPoint 
{
    public string enemyId;
    public Transform point;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnPoint[] spawnPoints;


    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool hasSpawned = false;

    public bool isCleared 
    {
        get 
        {
            if(!hasSpawned) return false;
            spawnedEnemies.RemoveAll(e => e == null);
            return spawnedEnemies.TrueForAll(e => !e.activeInHierarchy);
        }

    }

    public void SpawnEnemies()
    {
       
        if (hasSpawned) return;

      
        if (EnemyPoolingManager.instance == null) return;

        foreach (var sp in spawnPoints)
        {

            if (sp.point == null || string.IsNullOrEmpty(sp.enemyId))
            {

                continue;
            }

            GameObject enemy = EnemyPoolingManager.instance.Spawn(sp.enemyId, sp.point.position, Quaternion.identity);
            

            if (enemy != null)
                spawnedEnemies.Add(enemy);
        }

        hasSpawned = true;
    }

    private void OnDrawGizmos()
    {
        if(spawnPoints == null) return;
        Gizmos.color = Color.magenta;
        foreach(var sp in spawnPoints) 
        {
            if (sp.point != null)
                Gizmos.DrawWireSphere(sp.point.position, 0.3f);
        }
    }
}
