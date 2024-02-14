using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size; //Max num units that can be active at once.
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    #region Singleton
    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        if (poolDictionary[tag].Count != 0)
        {
            GameObject objToSpawn = poolDictionary[tag].Dequeue();

            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.SetActive(true);

            IPooledObject pooledObj = objToSpawn.GetComponent<IPooledObject>();
            if (pooledObj != null)
            {
                pooledObj.OnSpawn();
            }

            return objToSpawn;
        }

        return null;
    }

    public void ReAddObjectToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        IPooledObject pooledObj = obj.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.Reset();
        }
        else
        {
            Debug.LogWarning("Couldn't reset pooled object: ", obj);
        }

        ResetObjectVelocity(obj);

        if (!poolDictionary[tag].Contains(obj))
        {
            poolDictionary[tag].Enqueue(obj);
        }
    }

    private void ResetObjectVelocity(GameObject obj)
    {
        Rigidbody objRigidBody = obj.GetComponent<Rigidbody>();
        if (objRigidBody != null)
        {
            objRigidBody.velocity = Vector3.zero;
            objRigidBody.angularVelocity = Vector3.zero;
        }
    }
}
