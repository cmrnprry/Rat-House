using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Movement for note was written by Yu Chao
//I may have changed the naming of things, but the core funcionality came from them
//https://www.gamasutra.com/blogs/YuChao/20170316/293814/Music_Syncing_in_Rhythm_Games.php
public class Note : MonoBehaviour
{
    //Point where the note will spawn
    private Vector3 spawnPoint;

    //Point where the note will be destroyed
    private Vector3 destroyPoint;

    //Number of beats shown in advance
    private float beatsShownInAdvance;

    //beat of this note
    private float beat;

    //sets the values for the Note
    public void SetValues(Vector3 spawn, Vector3 destroy, float b, float show = 3)
    {
        Debug.Log("Set");

        spawnPoint = spawn;
        destroyPoint = destroy;
        beat = b;
        beatsShownInAdvance = show;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector2.Lerp(
            spawnPoint,
            destroyPoint,
            (beatsShownInAdvance - (beat - AudioManager.instance.songPositionInBeats)) / beatsShownInAdvance
        );

        if (transform.position.y <= destroyPoint.y)
        {
            Debug.Log("Destroy");
            Destroy(this.gameObject);
        }
    }
}
