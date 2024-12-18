using System.Collections.Generic;
using UnityEngine;

public class CubePooling : MonoBehaviour
{
    public static CubePooling instance;
    public int initSize = 10;
    public int maxSize = 100;
    public GameObject cubePrefabs;
    private Queue<GameObject> cubepools = new Queue<GameObject>();
    private Vector3 cubeScale;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitPool(initSize);
        cubeScale = new Vector3(1f, 0.25f, 1f);
    }

    public void InitPool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(cubePrefabs);
            obj.SetActive(false);
            cubepools.Enqueue(obj);
        }
    }

    public GameObject GetCubeFromPool()
    {
        if (cubepools.Count == 0 && cubepools.Count < maxSize)
        {
            GameObject obj = Instantiate(cubePrefabs);
            return obj;
        }
        else if (cubepools.Count > 0)
        {
            GameObject obj = cubepools.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning("Pool size exceeded");
            return null;
        }
    }

    public void ReturnCubeToPool(GameObject obj)
    {
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.Euler(Vector3.zero);
        obj.GetComponent<Rigidbody>().useGravity = false;
        obj.GetComponent<Rigidbody>().isKinematic = false;
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
       
        obj.SetActive(false);

        cubepools.Enqueue(obj);
    }
}
