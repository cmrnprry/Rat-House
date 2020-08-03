using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Movement for note was written by Yu Chao
//I may have changed the naming of things, but the core funcionality came from them
//https://www.gamasutra.com/blogs/YuChao/20170316/293814/Music_Syncing_in_Rhythm_Games.php
public class Note : MonoBehaviour
{
    //Notes to be hit
    public GameObject note;

    //Parent holder for the notes
    public GameObject noteParent;

    //Starting X position of the slider
    public Vector3 startPoint;

    //Point to reset the slider
    public Vector3 restartPoint;

    //Current action selected
    private ActionType _curAction;

    //List of the beats to be hit
    private List<float> beats = new List<float>();


    Renderer rend;
    private float offsetSlider;


    private float _length;


    private void Start()
    {
        rend = GetComponent<Renderer>();
        //startPoint = this.gameObject.transform.position;
        ShowBeats();
        _length = Vector3.Distance(startPoint, restartPoint);

        //half the width
        offsetSlider = -rend.bounds.size.x * .5f;
        // transform.position += new Vector3(offsetSlider, 0f, 0f);

        Debug.Log("start: " + startPoint);
        Debug.Log("end: " + restartPoint);
        Debug.Log("Local Pos: " + Vector3.Lerp(startPoint, restartPoint, 0));
        Debug.Log("Local Pos: " + Vector3.Lerp(startPoint, restartPoint, 0.5f));
        Debug.Log("Local Pos: " + Vector3.Lerp(startPoint, restartPoint, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        //if the attact music is playing, then the player is playing the rhythm mini-game
        if (AudioManager.instance.attackMusic.isPlaying)
        {
            //Moves the block based on where we are in the the music in BEATS
            gameObject.transform.position = Vector3.Lerp(startPoint, restartPoint, AudioManager.instance.mapProgression);

            if (gameObject.transform.position.x >= restartPoint.x)
            {
                AudioManager.instance.attackMusic.Stop();

                gameObject.transform.position = startPoint;

                //  transform.position += new Vector3(offsetSlider, 0f, 0f);

                //IF WE ARE NOT IN THE TUTORIAL DEAL DAMAGE
                if (GameManager.instance.GetGameState() != GameState.Tutorial)
                {
                    //Calculate Damage
                    CombatController.instance.DealDamage();
                }
            }

        }
        else if (CombatController.instance.selectedAction != _curAction)
        {
            ClearBeats();

            _curAction = CombatController.instance.selectedAction;

            ShowBeats();

            if (_curAction != ActionType.Item)
            {
                CombatStats.hitList.Sort();
            }

        }
    }

    private void ClearBeats()
    {
        Debug.Log("clear");
        foreach (Transform child in noteParent.transform)
        {
            Destroy(child.gameObject);
        }

        beats = new List<float>();
        CombatStats.hitList = new List<float>();
    }

    //Display the BeatMap in game
    private void ShowBeats()
    {
        if (_curAction != ActionType.Item)
        {
            //TODO: Implement other attacks
            if (_curAction == ActionType.Kick)
            {
                _curAction = ActionType.Punch;
            }
            beats = AudioManager.instance.beatMap[(int)_curAction].beatsToHit;
            CombatStats._totalHits = beats.Count;
        }

        var spawnPoint = this.gameObject.transform.position;

        foreach (var beat in beats)
        {
            //Want each beat in terms of the map progression
            var beatFraction = (beat + 1) / AudioManager.instance.totalBeats;

            //Spawn the Beat based on the start point, length and position of the beat
            spawnPoint = Vector3.Lerp(startPoint, restartPoint, beatFraction);

            //add the "perfect" hit point to the list
            CombatStats.hitList.Add(spawnPoint.x);

            //Create a note object and position it correctly
            var note = Instantiate(this.note, noteParent.transform, true);

            //make sure the rotation is 0 and put the parent in the right place
            note.transform.rotation = Quaternion.Euler(0, 0, 0);
            note.transform.localPosition = spawnPoint;// new Vector3(spawnPoint.x, note.transform.localPosition.y, 0f);
            note.transform.parent = noteParent.transform;
        }
    }
}
