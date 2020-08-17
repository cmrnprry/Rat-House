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
    //Beat Maps
    public List<BeatMapStruct> playerBeatMap = new List<BeatMapStruct>();
    public List<BeatMapStruct> enemyBeatMap = new List<BeatMapStruct>();

    [Header("Sound Effects")]
    public AudioSource SFX;
    public List<AudioClip> attackSFX;
    public List<AudioClip> dodgeSFX;
    public List<AudioClip> enemySFX;
    public List<AudioClip> UISFX;

    [Header("Background Music")]
    //Background Combat Music
    public AudioSource bgMusic;
    public List<AudioClip> bgClips;

    [Header("BeatMap Beats")]
    public bool startAction;
    public bool startDodge;
    public List<float> chosenEnemyAttack = new List<float>();

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

    //Totals beats in map
    public float totalBeats;

    //How many seconds have passed since the song started
    public float dspMapTime;

    //Tracks where you are in the beat map
    // Stays between 0 and 1
    public float mapProgression = 0;

    //start moving slider
    public bool hasStarted;


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
            // Do not destroy this object, when we load a new scene.
            DontDestroyOnLoad(this.gameObject);
        }
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

        //Sets the total beats for the first clip
        totalBeats = 8.5f;
    }

    //Start BG music when a fight starts
    public void StartCombatMusic()
    {
        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        //Start the background music
        bgMusic.clip = bgClips[0];
        bgMusic.Play();

        //Start the update loop
        StartCoroutine(UpdateBeats());
    }

    //Stops BG music when a fight ends
    public void StopCombatMusic()
    {
        //Start the background music
        bgMusic.clip = bgClips[1];
        bgMusic.Play();

        //Start the update loop
        StopAllCoroutines();
    }

    //Updates the BG song position and some other variables
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
    public IEnumerator SetAttackMap(int action)
    {
        //want currPos to round up to the next 1/2 second/next beat
        double currPos = Math.Round(songPosition, MidpointRounding.AwayFromZero);

        Debug.Log("currPos: " + currPos);

        // Wait until the next second
        //Wait until the 2nd next whole second
        //((so if at .5 -> 2))
        // ((or if at 1 -> 3))
        yield return new WaitUntil(() => (currPos + (beatsPerSec * 2) <= songPosition));

        dspMapTime = (float)AudioSettings.dspTime;

        yield return new WaitForFixedUpdate();

        startAction = true;
        StartCoroutine(StartMapUpdates());
    }

    //Waits a second before starting the dodge music
    public IEnumerator SetDodgeMap(int action = 0)
    {
        //TODO: Change this to have 

        float currPos = songPositionInBeats;

        // Wait until the next second
        yield return new WaitUntil(() => (currPos + (beatsPerSec * 2) <= songPositionInBeats));

        dspMapTime = (float)AudioSettings.dspTime;

        yield return new WaitForFixedUpdate();

        startDodge = true;
        Debug.Log("Play dodge");
        StartCoroutine(StartMapUpdates());
    }

    //Keeps track of the position of the attack music
    //Will also possibly deal with handling Hit/Misses of the player but probably not
    //After each attack there should be a check if all the enemies have been defeated
    public IEnumerator StartMapUpdates()
    {

        mapPosition = (float)(AudioSettings.dspTime - dspMapTime) < 0 ? 0 : (float)(AudioSettings.dspTime - dspMapTime);
        mapPositionInBeats = mapPosition / mapSecPerBeat;
        mapProgression = mapPositionInBeats / totalBeats;

        if (!startAction && !startDodge)
        {
            Debug.Log("reset beats");

            mapPosition = 0;
            mapPositionInBeats = 0;
            mapProgression = 0;

            yield break;
        }

        yield return new WaitForFixedUpdate();
        StartCoroutine(StartMapUpdates());
    }

    public void SetBeatMaps(List<BeatMapStruct> player, List<BeatMapStruct> enemy)
    {
        playerBeatMap = player;
        enemyBeatMap = enemy;
    }
}
