using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


//Base class was written by Graham Tattersall
//I may have changed the naming of things, but the core funcionality came from them
//https://www.gamasutra.com/blogs/GrahamTattersall/20190515/342454/Coding_to_the_Beat__Under_the_Hood_of_a_Rhythm_Game_in_Unity.php
public class AudioManager : MonoBehaviour
{
    [Header("Beats")]
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

    //How many seconds have passed since the song started
    public float dspSongTime;

    //The offset to the first beat of the song in seconds
    public float firstBeatOffset;

    //Peaks
    public List<float> peaks;

    [Header("Audio")]
    //Array of all the songs in the game
    public SongInfo[] songs;

    [HideInInspector]
    //the current song playing
    public SongInfo currSong;

    //an AudioSource attached to this GameObject that will play the music.
    private AudioSource musicSource;

    [Header("Loops")]
    //the number of beats in each loop
    public float beatsPerLoop;

    //the total number of loops completed since the looping clip first started
    public int completedLoops = 0;

    //The current position of the song within the loop in beats.
    public float loopPositionInBeats;

    //The current relative position of the song within the loop measured between 0 and 1.
    public float loopPositionInAnalog;

    [Header("Start")]
    //Ready Text
    public TextMeshProUGUI text;
    public float wait = 2f;
    public bool start = false;

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

        //TODO: Add more than one track
        //For not just use the 0 index
        musicSource = songs[0].source;
        currSong = songs[0];
        songBpm = songs[0].bpm;
    }
    void Start()
    {
        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        //Calculate the number of beats in each second
        beatsPerSec = songBpm / 60f;

        //number of beats per loop
        beatsPerLoop = musicSource.clip.length * beatsPerSec;

        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        StartCoroutine(WaitToStart());
    }

    //A way to delay starting the music
    IEnumerator WaitToStart()
    {
        //wait a few seconds before changing the text
        yield return new WaitForSecondsRealtime(wait);
        text.text = "Start!";

        //wait a few seconds before starting the game
        yield return new WaitForSecondsRealtime(wait);


        //Set Notes using the Peaks
        //SetNotes();

        //Turn off the text
        text.gameObject.SetActive(false);

        //Wait for the end of frame and startt the game
        yield return new WaitForEndOfFrame();

        //Set a bool to stay game has started
        start = true; 

        //Start the music
        musicSource.Play();

        //Start the update loop
        StartCoroutine(UpdateBeats());

    }

    //Serving as a more controlled Update loop
    IEnumerator UpdateBeats()
    {
        //determine how many seconds since the song started
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;

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

    //IMPORTANT: ALL BEATS/LOOPS ARE 0 INDEXED
    void SetNotes()
    {

        var peak = GameObject.FindGameObjectsWithTag("Peak");
        Debug.Log("Peaks: " + peaks.Count);


        currSong.notes = peaks.ToArray();
        SpawnNote.notes = peaks.ToArray();

    }
}
