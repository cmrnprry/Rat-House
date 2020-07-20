using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Movement for note was written by Yu Chao
//I may have changed the naming of things, but the core funcionality came from them
//https://www.gamasutra.com/blogs/YuChao/20170316/293814/Music_Syncing_in_Rhythm_Games.php
public class Note : MonoBehaviour
{
    //Number of beats shown in advance
    private float beatsShownInAdvance;

    //Notes to be hit
    public GameObject note;

    //Parent holder for the notes
    public GameObject noteParent;

    //Bool to tell if you can hit the 

    public Vector3 destroyPoint;
    //beat of this note
    private float beat;
    private ActionType _curAction;
    private List<float> beats = new List<float>();


    private void Start()
    {
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

            if (gameObject.transform.localPosition.x <= destroyPoint.x)
            {
                Debug.Log("Stop Attack Music");
                AudioManager.instance.attackMusic.Stop();
                gameObject.transform.localPosition = new Vector3(4.9f, this.gameObject.transform.localPosition.y, 3.5f);

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
        Debug.Log("Show Beats");
        Debug.Log(_curAction != ActionType.Item);
        if (_curAction != ActionType.Item)
        {
            beats = AudioManager.instance.beatMap[(int)_curAction].beatsToHit;
        }


        var spawnPoint = this.gameObject.transform.position;

        foreach (var beat in beats)
        {
            Debug.Log("add");
            spawnPoint.x = this.gameObject.transform.position.x + beat + 1f;
            var note = Instantiate(this.note, spawnPoint, Quaternion.identity);
            note.transform.parent = noteParent.transform;
        }
    }


   
}
