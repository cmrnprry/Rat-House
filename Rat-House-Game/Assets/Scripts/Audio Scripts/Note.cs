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

    //Point to reset the slider
    public Vector3 restartPoint;

    //Current action selected
    private ActionType _curAction;

    //List of the beats to be hit
    private List<float> beats = new List<float>();

    //Starting X position of the slider
    private float _startX;


    private void Start()
    {
        _startX = this.gameObject.transform.localPosition.x;
        ShowBeats();
    }

    // Update is called once per frame
    void Update()
    {
        //if the attact music is playing, then the player is playing the rhythm mini-game
        if (AudioManager.instance.attackMusic.isPlaying)
        {
            //transform.position = Vector3.Lerp(
            //    gameObject.transform.position,
            //    destroyPoint,
            //    (AudioManager.instance.mapBeatsPerSec)
            //);

            transform.position += new Vector3(AudioManager.instance.mapBeatsPerSec * Time.deltaTime, 0f, 0f);

            if (gameObject.transform.localPosition.x <= restartPoint.x)
            {
                Debug.Log("Stop Attack Music");
                AudioManager.instance.attackMusic.Stop();
                gameObject.transform.localPosition = new Vector3(_startX, this.gameObject.transform.localPosition.y, this.gameObject.transform.localPosition.z);

                //Calculate Damage
                CombatController.instance.DealDamage();
            }

        }
        else if (CombatController.instance.selectedAction != _curAction)
        {
            ClearBeats();

            _curAction = CombatController.instance.selectedAction;

            ShowBeats();
        }
    }

    private void ClearBeats()
    {
        Debug.Log("clear");
        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        beats = new List<float>();
    }

    private void ShowBeats()
    {
        if (_curAction != ActionType.Item)
        {
            beats = AudioManager.instance.beatMap[(int)_curAction].beatsToHit;
            CombatStats._totalHits = beats.Count;
        }


        var spawnPoint = this.gameObject.transform.position;

        foreach (var beat in beats)
        {
            spawnPoint.x = this.gameObject.transform.position.x + beat + 1f;
            var note = Instantiate(this.note, spawnPoint, Quaternion.identity);
            note.transform.parent = noteParent.transform;
        }

        //gameObject.transform.localPosition = new Vector3(4.9f, this.gameObject.transform.localPosition.y, 3.5f);
    }



}
