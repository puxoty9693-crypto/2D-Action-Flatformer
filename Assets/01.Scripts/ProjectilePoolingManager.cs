using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolingManager : MonoBehaviour
{
    public static ProjectilePoolingManager instance;

    [SerializeField] private Transform poolRoot;
    [SerializeField] private ProjectileData[] allProjectileData;

    private class PoolInfo 
    {
        public string projectileId;
        public IPoolable poolable;
    }

    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<GameObject, PoolInfo> instanceInfo = new Dictionary<GameObject, PoolInfo>();
    private Dictionary<string, ProjectileData> dataLookup = new Dictionary<string, ProjectileData>();

    
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }

        BuildPools();
    }

    private void BuildPools() 
    {
        foreach (var data in allProjectileData)
        {
            if(data == null || data.projectilePrefab == null) continue;

            dataLookup[data.projectileId] = data;

            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < data.poolSize; i++) 
            {
                GameObject obj = CreateNewInstance(data);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            pools[data.projectileId] = queue;
        }
    }

    private GameObject CreateNewInstance(ProjectileData data) 
    {
        GameObject obj = Instantiate(data.projectilePrefab, poolRoot);
        instanceInfo[obj] = new PoolInfo
        {
            projectileId = data.projectileId,
            poolable = obj.GetComponent<IPoolable>()
        };
        return obj;
    }

    public ProjectileData GetData(string projectileId) 
    {
        dataLookup.TryGetValue(projectileId, out ProjectileData data);
        return data;
    }

    public GameObject Spawn(string projectileId, Vector3 position, Quaternion rotation)
    {
        if (!pools.TryGetValue(projectileId, out Queue<GameObject> queue))
        {
            return null;
        }


        GameObject obj;
        if (queue.Count > 0) 
        {
            obj = queue.Dequeue();
        }

        else 
        {
            ProjectileData data = dataLookup[projectileId];
            obj = CreateNewInstance(data);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        instanceInfo[obj].poolable?.OnSpawn();

        return obj;

    }

    public void Return(GameObject obj) 
    {
        if(!instanceInfo.TryGetValue(obj, out PoolInfo info)) 
        {
            Destroy(obj);
            return;
        }


        info.poolable?.OnDespawn();

        obj.SetActive(false);
        obj.transform.SetParent(poolRoot);
        pools[info.projectileId].Enqueue(obj);
    }

}
