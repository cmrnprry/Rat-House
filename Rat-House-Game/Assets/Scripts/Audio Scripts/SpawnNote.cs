﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spawning of note was written by Yu Chao
//I may have changed the naming of things, but the core funcionality came from them
//https://www.gamasutra.com/blogs/YuChao/20170316/293814/Music_Syncing_in_Rhythm_Games.php
public class SpawnNote : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
  ////if there is another note to spawn AND the note has not already been spawned
  //          if (index < notes.Length && notes[index] < AudioManager.instance.songPositionInBeats + beatsShownInAdvance)
  //          {
  //              Debug.Log("Spawn");
  //              //create a note
  //              var note = Instantiate(obj, spawnPoint, Quaternion.identity);
  //              note.GetComponent<Note>().SetValues(spawnPoint, destroyPoint, notes[index]);

  //              index++;
  //          }

  //          //TODO: Probably dont want to repeat the note pattern,
  //          // but for now that's what we are going to do

  //          //If a new loop is started, reset the notes
  //          if (currLoop != AudioManager.instance.completedLoops)
  //          {
  //              currLoop++;
  //              index = 0;
  //          }

  //          //if (prev == Math.Truncate(AudioManager.instance.songPositionInBeats))
  //          //{
  //          //    //curr = (float)Math.Truncate(GameManager.instance.songPosition);
  //          //    Instantiate(obj, spawnPoint, Quaternion.identity);
  //          //    Debug.Log("another one");

  //          //    prev += 3;
  //          //}
    }
}