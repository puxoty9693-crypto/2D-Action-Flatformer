using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EnemyPoolingManager : MonoBehaviour
{
    public static EnemyPoolingManager instance;

    [SerializeField] private Transform poolRoot;
    [SerializeField] private EnemyData[] allEnemyData;

    private class PoolInfo 
    {
        public string enemyId;
        public IPoolable poolable;
    }


    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<GameObject, PoolInfo> instanceInfo = new Dictionary<GameObject, PoolInfo>();
    private Dictionary<string, EnemyData> dataLookup = new Dictionary<string, EnemyData>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
        {
            Destroy(instance);
            return;
        }
        BuildPools();

    }

    private void BuildPools() 
    {
        foreach(var data in allEnemyData) 
        {
            if(data == null || data.enemyPrefab == null) continue;

            Queue<GameObject> queue = new Queue<GameObject>();
            for(int i = 0; i < data.poolSize; i++) 
            {
                GameObject obj = CreateNewInstance(data);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            pools[data.enemyId] =queue;

        }
    }

    private GameObject CreateNewInstance(EnemyData data) 
    {
        GameObject obj = Instantiate(data.enemyPrefab, poolRoot);

        instanceInfo[obj] = new PoolInfo
        {
            enemyId = data.enemyId,
            poolable = obj.GetComponent<IPoolable>()
        };


        return obj;
    }

    public EnemyData GetData(string enemyId) 
    {
        dataLookup.TryGetValue(enemyId, out EnemyData data);
        return data;
    }



    public GameObject Spawn(string enemyId, Vector3 position, Quaternion rotation) 
    {


        if (!pools.TryGetValue(enemyId, out Queue<GameObject> queue))
        {

            return null;
        }

        GameObject obj;
        if(queue.Count > 0) 
        {
            obj = queue.Dequeue();
        }

        else 
        {
            EnemyData data = dataLookup[enemyId];
            obj = CreateNewInstance(data);
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        instanceInfo[obj].poolable?.OnDespawn();

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
        pools[info.enemyId].Enqueue(obj);

    }
   



}
