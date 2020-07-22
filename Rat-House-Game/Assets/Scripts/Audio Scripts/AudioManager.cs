using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static BeatMapReader;
using System;


//Base class was written by Graham Tattersall
//I may have changed the naming of things, but the core funcionality came from them
//https://www.gamasutra.com/blogs/GrahamTattersall/20190515/342454/Coding_to_the_Beat__Under_the_Hood_of_a_Rhythm_Game_in_Unity.php
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    //Background Combat Music
    public AudioSource bgMusic;

    //Attack Audio Source
    public AudioSource attackMusic;

    [Header("BeatMap Beats")]
    //List of possible music to play
    public AudioClip[] attackClips;

    //map bmp
    public float mapBpm;

    //The number of seconds for each map beat
    public float mapSecPerBeat;

    //map Beats per second
    public float mapBeatsPerSec;

    //Current map position, in seconds
    public float mapPosition;

    //Current map position, in beats
    public float mapPositionInBeats;

    //How many seconds have passed since the song started
    public float dspMapTime;

    //offset
    public float offset;

    //start moving slider
    public bool hasStarted;

    //Beat Maps
    public List<BeatMapStruct> beatMap = new List<BeatMapStruct>();

    [Header("BG Beats")]
    //Song beats per minute
    public float songBpm;

    //The number of seconds for each song beat
    public float secPerBeat;

    //Beats per second
    public float beatsPerSec;

    //Current song position, in seconds
    public float songPosition;

    //Current song position, in beats
    public float songPositionInBeats;

    //Thd difference between the last frame songPosition and the current frame songPosition
    public float soundDeltaTime;

    //How many seconds have passed since the song started
    public float dspSongTime;

    [Header("Loops")]
    //the number of beats in each loop
    public float beatsPerLoop;

    //the total number of loops completed since the looping clip first started
    public int completedLoops = 0;

    //The current position of the song within the loop in beats.
    public float loopPositionInBeats;

    //The current relative position of the song within the loop measured between 0 and 1.
    public float loopPositionInAnalog;


    //Instance of the Audio Manager
    public static AudioManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(this.gameObject);
    }


    void Start()
    {
        //BACKGROUND

        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        //Calculate the number of beats in each second
        beatsPerSec = songBpm / 60f;

        //number of beats per loop
        beatsPerLoop = bgMusic.clip.length * beatsPerSec;

        //Calculate the number of seconds in each beat
        mapSecPerBeat = 60f / mapBpm;

        //Calculate the number of beats in each second
        mapBeatsPerSec = mapBpm / 60f;
    }

    //Start BG music when a fight starts
    public void StartCombatMusic()
    {
        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        Debug.Log("Start BG Music");
        //Start the background music
        bgMusic.Play();

        //Start the update loop
        StartCoroutine(UpdateBeats());
    }

    //Stops BG music when a fight ends
    public void StopCombatMusic()
    {
        //Start the background music
        bgMusic.Stop();

        //Start the update loop
        StopAllCoroutines();
    }

    //Updates the BG song position
    IEnumerator UpdateBeats()
    {
        var lastFrame = songPosition;

        //determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;

        soundDeltaTime = songPosition - lastFrame;


        //Handles the looping
        loopPositionInBeats = songPositionInBeats - completedLoops * beatsPerLoop;
        loopPositionInAnalog = loopPositionInBeats / beatsPerLoop;

        //calculate the loop position
        if (songPositionInBeats >= (completedLoops + 1) * beatsPerLoop)
        {
            completedLoops++;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(UpdateBeats());
    }

    //Waits a second before starting the attack music
    public IEnumerator SetMap(int action)
    {
        Debug.Log("Set Map");
        float currPos = songPositionInBeats;

        //TODO: Implement more attack clips when we have them
        // attackMusic.clip = attackClips[action];

        // Wait until the next second
        hasStarted = true;
        while (!(Math.Truncate(currPos) + beatsPerSec <= songPositionInBeats))
        {
            yield return null;
        }


        dspMapTime = (float)AudioSettings.dspTime;
        attackMusic.Play();
        StartCoroutine(StartBasicAttack());
    }

    //Keeps track of the position of the attack music
    //Will also possibly deal with handling Hit/Misses of the player but probably not
    //After each attack there should be a check if all the enemies have been defeated
    public IEnumerator StartBasicAttack()
    {
        mapPosition = (float)(AudioSettings.dspTime - dspMapTime);
        mapPositionInBeats = mapPosition / mapSecPerBeat;

        if (!attackMusic.isPlaying)
        {
            hasStarted = false;
            yield break;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(StartBasicAttack());
    }

    public void SetBeatMaps(List<BeatMapStruct> bm)
    {
        beatMap = bm;
    }
}
