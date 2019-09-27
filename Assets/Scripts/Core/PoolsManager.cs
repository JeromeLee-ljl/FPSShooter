using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PoolInfo
{
    public GameObject prefab;
    [Range(0, 100)] public int initCount = 5;
    [Range(0, 100)] public int maxCount = 10;
    [HideInInspector] public Queue<GameObject> pool; //对象池
    [HideInInspector] public Transform holder; //对象池中所有GameObject都在holder的层级下
}

public class PoolsManager : MonoBehaviour
{
    public static PoolsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
//        DontDestroyOnLoad(gameObject);
        InitPools();
    }

    // 仅仅在初始化时使用
    public List<PoolInfo> poolsInfo;

    //维护所有的对象池  用Dictionary通过名字来索引pool
    private Dictionary<string, PoolInfo> poolsDict;

    private void InitPools()
    {
        poolsDict = new Dictionary<string, PoolInfo>();
        foreach (var pool in poolsInfo)
        {
            AddPool(pool);
        }
    }

    // 通过prefabName获取实例
    public GameObject Get(string prefabName)
    {
        if (!poolsDict.ContainsKey(prefabName))
        {
            Debug.Log("PoolManager have no " + prefabName);
            return null;
        }

        PoolInfo poolInfo = poolsDict[prefabName];
        if (poolInfo.pool.Count == 0) // 对象池空，实例化一个
            return Instantiate(poolInfo.prefab, poolInfo.holder);
        GameObject instance = poolInfo.pool.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    /// <summary>
    /// 如果prefabName为null 则把T作为prefabName
    /// </summary>
    public T Get<T>(string prefabName = null)
    {
        if (prefabName == null)
            prefabName = typeof(T).Name;
        GameObject obj = Get(prefabName);
        return obj.GetComponent<T>();
    }

    // 返还实例给pool
    public void Recycle(GameObject instance)
    {
        if (instance == null)
        {
            Debug.Log("the instance is Null, don`t need recycle");
            return;
        }

        string prefabName = instance.name;
        if (prefabName.EndsWith("(Clone)"))
            prefabName = prefabName.Substring(0, prefabName.Length - 7);

        if (poolsDict.ContainsKey(prefabName) && poolsDict[prefabName].pool.Count < poolsDict[prefabName].maxCount)
        {
            instance.SetActive(false);
            poolsDict[prefabName].pool.Enqueue(instance);
        }
        else
        {
            Destroy(instance);
        }
    }

    public void AddPool(GameObject obj, int initCount, int maxCount)
    {
        AddPool(new PoolInfo {prefab = obj, initCount = initCount, maxCount = maxCount});
    }

    // 添加新的对象池
    private void AddPool(PoolInfo poolInfo)
    {
        if (poolInfo.prefab == null)
        {
            Debug.Log("pool manager: attempt add null");
            return;
        }

        if (poolsDict.ContainsKey(poolInfo.prefab.name))
        {
            Debug.Log($"pool manager: 尝试添加已存在的 {poolInfo.prefab.name} prefab");
            return;
        }

        //用poolHolder来组织实例, 并将它放在PoolManager节点下
        poolInfo.holder = new GameObject(poolInfo.prefab.name + "Pool").transform;
        poolInfo.holder.SetParent(transform);
        poolInfo.pool = new Queue<GameObject>();
        for (int i = 0; i < poolInfo.initCount; i++)
        {
            GameObject instance = Instantiate(poolInfo.prefab, poolInfo.holder);
            instance.SetActive(false);
            poolInfo.pool.Enqueue(instance);
        }

        poolsDict.Add(poolInfo.prefab.name, poolInfo);
    }
}