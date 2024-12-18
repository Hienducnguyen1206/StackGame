using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -10f)
        {
            CubePooling.instance.ReturnCubeToPool(gameObject);
        }
    }

    public void ReturnToPool()
    {
        CubePooling.instance.ReturnCubeToPool(gameObject);
    }
}
