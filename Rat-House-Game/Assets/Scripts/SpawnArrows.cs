using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArrows : MonoBehaviour
{
    public float spawnTime;
    public Vector3 spawnPoint;
    public GameObject obj;

    private float prev, curr = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
       // Debug.Log("Curr: " + curr);
        Debug.Log("Prev: " + prev);
        Debug.Log("Math: " + Math.Truncate(GameManager.instance.songPositionInBeats));
        Debug.Log("Math + 3: " + Math.Truncate(GameManager.instance.songPositionInBeats + 3));
        //curr % GameManager.instance.beatsPerSec == 0.0f && 

        if (prev == Math.Truncate(GameManager.instance.songPositionInBeats))
        {
            //curr = (float)Math.Truncate(GameManager.instance.songPosition);
            Instantiate(obj, spawnPoint, Quaternion.identity);
            Debug.Log("another one");
         
            prev += 3;
        }
       
    }

    IEnumerator SpawnEnemy()
    {
        var e = Instantiate(obj, spawnPoint, Quaternion.identity);
        var curr = Math.Truncate(GameManager.instance.songPosition);
        Debug.Log("Curr: " + curr);
        Debug.Log("Math: " + Math.Truncate(GameManager.instance.songPosition));

        yield return new WaitUntil(() => curr % GameManager.instance.beatsPerSec == 0.0f && prev != Math.Truncate(GameManager.instance.songPosition));
        Debug.Log("another one");
        prev = (float) curr;
        StartCoroutine(SpawnEnemy());
    }
}
