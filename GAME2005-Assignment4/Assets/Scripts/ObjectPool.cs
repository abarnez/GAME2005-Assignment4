using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public static ObjectPool Instance;

    public List<GameObject> tObjectPool;
    public GameObject tObject;
    public int maxObjects;

    public Transform parentObject;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        tObjectPool = new List<GameObject>();
        for (int i = 0; i < maxObjects; i++)
        {
            GameObject obj = (GameObject)Instantiate(tObject);
            obj.SetActive(false);
            tObjectPool.Add(obj);
            obj.transform.parent = parentObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < tObjectPool.Count; i++)
        {
            if(!tObjectPool[i].activeInHierarchy)
            {
                return tObjectPool[i];
            }
        }
        return null;
    }
}
