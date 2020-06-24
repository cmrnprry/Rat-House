using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SpawnCastles : MonoBehaviour
{
    public GameObject spawnObject;
    private int timer=0;
    public int timeToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + 1;
        if (timer >= timeToSpawn)
        {
            SpawnObject();
            timer = 0;
        }
    }

    void SpawnObject()
    {
        UnityEngine.Vector3 position = new UnityEngine.Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
        Instantiate(spawnObject, position, UnityEngine.Quaternion.identity);
    }
}
