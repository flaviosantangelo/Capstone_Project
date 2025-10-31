using UnityEngine;
using System.Collections.Generic;


public class PoolingManager : MonoBehaviour
{
    public static PoolingManager _instance { get; private set; }
    private Dictionary<string, Queue<GameObject>> _poolDictionary;
    private Dictionary<GameObject, string> _prefabLookup;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();
            _prefabLookup = new Dictionary<GameObject, string>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
    public void CreatePool(GameObject prefab, int size)
    {
        string prefabName = prefab.name;
        if (!_poolDictionary.ContainsKey(prefabName))
        {
            _poolDictionary[prefabName] = new Queue<GameObject>();
            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                _prefabLookup[obj] = prefabName;
                _poolDictionary[prefabName].Enqueue(obj);
            }
        }
    }
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string prefabName = prefab.name;

        if (!_poolDictionary.ContainsKey(prefabName))
        {
            CreatePool(prefab, 1); 
        }

        if (_poolDictionary[prefabName].Count > 0)
        {
            GameObject objToSpawn = _poolDictionary[prefabName].Dequeue();

            objToSpawn.SetActive(true);
            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;

            return objToSpawn;
        }
        else
        {
            GameObject newObj = Instantiate(prefab, position, rotation);
            _prefabLookup[newObj] = prefabName; 
            return newObj;
        }
    }


    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);

        if (_prefabLookup.ContainsKey(objectToReturn))
        {
            string prefabName = _prefabLookup[objectToReturn];
            _poolDictionary[prefabName].Enqueue(objectToReturn);
        }
        else
        {
            Destroy(objectToReturn);
        }
    }
}