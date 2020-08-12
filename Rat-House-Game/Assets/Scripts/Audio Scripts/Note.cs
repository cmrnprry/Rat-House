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
    public GameObject whole;
    public GameObject half;

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

    public static bool showDodge = false;


    private void Start()
    {
        rend = GetComponent<Renderer>();
        //startPoint = this.gameObject.transform.position;
        ShowAttackBeats();
        _length = Vector3.Distance(startPoint, restartPoint);

        //half the width
        offsetSlider = -rend.bounds.size.x * .5f;
        // transform.position += new Vector3(offsetSlider, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        //if the attact music is playing, then the player is playing the rhythm mini-game
        if (AudioManager.instance.startAction)
        {
            //Moves the block based on where we are in the the music in BEATS
            gameObject.transform.position = Vector3.Lerp(startPoint, restartPoint, AudioManager.instance.mapProgression);

            if (gameObject.transform.position.x >= restartPoint.x)
            {
                Debug.Log("action over");

                ClearBeats();

                AudioManager.instance.startAction = false;

                Flip();

                //IF WE ARE NOT IN THE TUTORIAL DEAL DAMAGE
                if (GameManager.instance.GetGameState() != GameState.Tutorial)
                {
                    //Calculate Damage
                    CombatController.instance.DealDamage();
                }
            }

        }
        else if (AudioManager.instance.startDodge)
        {
            //Moves the block based on where we are in the the music in BEATS
            gameObject.transform.position = Vector3.Lerp(restartPoint, startPoint, AudioManager.instance.mapProgression);

            if (gameObject.transform.position.x <= startPoint.x)
            {
                ClearBeats();

                AudioManager.instance.startDodge = false;

                gameObject.transform.position = restartPoint;
            }
        }
        else if (CombatController.instance.selectedAction != _curAction)
        {
            ClearBeats();

            _curAction = CombatController.instance.selectedAction;


            if (_curAction != ActionType.Item)
            {
                CombatStats.hitList.Sort();
                ShowAttackBeats();
            }

        }

        if (CombatController.instance.enemyTurnOver)
        {
            ShowAttackBeats();
            Flip(1);
            gameObject.transform.position = startPoint;
        }

        if (showDodge)
        {
            Flip();
            ShowDodgeBeats();
            showDodge = false;
        }
    }

    public void ShowDodgeBeats()
    {
        Vector3 spawnPoint = gameObject.transform.position;

        CombatStats.totalHits = AudioManager.instance.chosenEnemyAttack.Count;

        foreach (var beat in AudioManager.instance.chosenEnemyAttack)
        {
            //Want each beat in terms of the map progression
            var beatFraction = (beat + 1) / AudioManager.instance.totalBeats;

            //Spawn the Beat based on the start point, length and position of the beat
            spawnPoint = Vector3.Lerp(restartPoint, startPoint, beatFraction);

            bool isSquare = (beat % 1 == 0) ? true : false;
            var type = (beat % 1 == 0) ? whole : half;
            //add the "perfect" hit point to the list
            var b = new Beat(spawnPoint.x, isSquare);
            CombatStats.hitList.Add(b);

            //Create a note object and position it correctly
            var note = Instantiate(type, noteParent.transform, true);

            //make sure the rotation is 0 and put the parent in the right place
            note.transform.rotation = Quaternion.Euler(0, 0, 0);
            note.transform.position = spawnPoint;// new Vector3(spawnPoint.x, note.transform.localPosition.y, 0f);
            note.transform.parent = noteParent.transform;
        }
    }

    private void ShowAttackBeats()
    {
        if (_curAction != ActionType.Item)
        {
            beats = AudioManager.instance.playerBeatMap[(int)_curAction].beatsToHit;
            CombatStats.totalHits = beats.Count;
        }

        var spawnPoint = this.gameObject.transform.position;

        foreach (var beat in beats)
        {
            //Want each beat in terms of the map progression
            var beatFraction = (beat + 1) / AudioManager.instance.totalBeats;

            //Spawn the Beat based on the start point, length and position of the beat
            spawnPoint = Vector3.Lerp(startPoint, restartPoint, beatFraction);

            bool isSquare = (beat % 1 == 0) ? true : false;
            var type = (beat % 1 == 0) ? whole : half;
            //add the "perfect" hit point to the list
            var b = new Beat(spawnPoint.x, isSquare);
            CombatStats.hitList.Add(b);

            //Create a note object and position it correctly
            var note = Instantiate(type, noteParent.transform, true);

            //make sure the rotation is 0 and put the parent in the right place
            note.transform.rotation = Quaternion.Euler(0, 0, 0);
            note.transform.position = spawnPoint;
            note.transform.parent = noteParent.transform;
        }
    }

    private void ClearBeats()
    {
        CombatController.instance.hitDetectionText.gameObject.SetActive(false);

        Debug.Log("clear");
        foreach (Transform child in noteParent.transform)
        {
            Destroy(child.gameObject);
        }

        beats = new List<float>();

        //Reset the Combat Stats to empty/0
        CombatStats.hitList = new List<Beat>();
        CombatStats.index = 0;
        CombatStats.hitNote = false;
    }

    //flip the fangs when dodging
    // num is either positive or negative depending on which way it sould be facing
    public void Flip(int num = -1)
    {
        Vector3 theScale = gameObject.transform.GetChild(0).localScale;
        Vector3 thePos = gameObject.transform.GetChild(0).localPosition;
        theScale.x = num;

        thePos.x = (num < 0 ? -0.35f : 0.35f);
        transform.GetChild(0).localScale = theScale;
        transform.GetChild(0).localPosition = thePos;
    }
}
