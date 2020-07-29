using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScript : MonoBehaviour
{
    //Where in the dialogue to show
    private int _index = 0;
    private bool hasHit = false;

    //Checks if the battle action is done
    private bool _isFinished = false;

    public Animator anim;

    public Dialogue dialogue;

    //Shows the Opening Dialogue for the tutorial
    public IEnumerator ShowOpeningDialogue()
    {
        Debug.Log("Show Openinng Dialogue");

        //Waits for the text to stop typing
        while (dialogue.isTyping)
        {
            Debug.Log("Wait until done typing");
            yield return null;
        }

        //wait for the player to press enter/space
        while (!Input.GetButton("SelectAction"))
        {
            Debug.Log("waiting for player to hit enter/space");
            yield return null;
        }

        //when you press space...
        if (Input.GetButton("SelectAction"))
        {
            //When we're at the end of the intro dialogue
            if (_index == 0)//dialogue.sentences.Length)
            {
                //Lower the text box
                anim.SetBool("isOpen", false);
                _index = 0;

                yield return new WaitForSecondsRealtime(.2f);

                //play some sort of screen wipe

                //load correct scene
                SceneManager.LoadScene("Tutorial_Battle-FINAL");

                yield return new WaitForFixedUpdate();

                //Set up the battle scene
                SetUpTutorialBattle();

                yield return new WaitForFixedUpdate();

                StartTutorialBattle();
                yield break;
            }

            Debug.Log("Next Line");

            if (_index == 0 || _index == 5)
            {
                //trick the code into thinking the other person is talking
                dialogue.speakerName.text = dialogue.speakers[1];
            }

            _index++;

            //load next sentence
            dialogue.NextSentence();
            // dialogue.index += 1;

            StartCoroutine(ShowOpeningDialogue());
            yield break;
        }
    }

    void StartTutorialBattle()
    {
        //Start Music
        AudioManager.instance.StartCombatMusic();

        Debug.Log("start Tutorial Battle");

        //Set som stuff
        var d = GameObject.FindGameObjectsWithTag("Dialogue");

        anim = d[0].GetComponent<Animator>();
        dialogue = d[1].GetComponent<Dialogue>();
        anim.SetBool("isOpen", true);

        //Start dialogue
        dialogue.StartDialogue();
        StartCoroutine(ShowBattleDialogue());
    }

    IEnumerator ReturnControlToPlayer()
    {
        Debug.Log("Do action");

        //do action depending on where we are in dialogue
        switch (_index)
        {
            case 2:
                StartCoroutine(Punch());
                break;
            case 4:
                StartCoroutine(StartAttackMusic(0));
                break;
            default:
                Debug.LogError("Something has gone wrong :(");
                break;
        }

        //Waiting for player to finish an action
        while (!_isFinished)
        {
            Debug.Log("waiting for player action to finish");
            yield return null;
        }

        //reset the bool to fase
        _isFinished = false;

        anim.SetBool("isOpen", true);
        dialogue.NextSentence();

        StartCoroutine(ShowBattleDialogue());
    }

    IEnumerator StartAttackMusic(int selected)
    {
        Debug.Log("Start Slider");
        StartCoroutine(AudioManager.instance.SetMap(selected));

        yield return new WaitForSecondsRealtime(1f);

        while (AudioManager.instance.attackMusic.isPlaying)
        {
            Debug.Log("waiting attack to be over");
            yield return null;
        }

        CombatController.instance.battleMenu.SetActive(true);
        CombatController.instance.HighlightMenuItem();

        yield return new WaitForSecondsRealtime(1f);

        _isFinished = true;
    }

    IEnumerator Punch()
    {
        Debug.Log("Punch");

        //wait for the player to press enter/space
        while (!Input.GetButtonDown("SelectAction"))
        {
            Debug.Log("waiting for player to hit enter/space");
            yield return null;
        }

        //yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();

        //when you hit space...
        CombatController.instance.TurnOffHighlight();
        CombatController.instance.HighlightEnemy();

        StartCoroutine(SelectEnemy());

    }

    IEnumerator SelectEnemy()
    {
        Debug.Log("Select");

        //wait for the player to press enter/space
        while (!Input.GetButtonDown("SelectAction"))
        {
            Debug.Log("waiting for player to hit enter/space");
            yield return null;
        }

        CombatController.instance.TurnOffHighlight();

        _isFinished = true;
    }

    //Shows the Battle Dialogue for the tutorial
    public IEnumerator ShowBattleDialogue()
    {
        Debug.Log("Show Battle Dialogue");
        Debug.Log("index: " + _index);

        //Waits for the text to stop typing
        while (dialogue.isTyping)
        {
            Debug.Log("Wait until done typing");
            yield return null;
        }

        //wait for the player to press enter/space
        while (!Input.GetButton("SelectAction"))
        {
            Debug.Log("waiting for player to hit enter/space");
            yield return null;
        }

        //when you press space...
        if (Input.GetButton("SelectAction"))
        {
            //When we're at the end of the intro dialogue
            if (_index % 2 == 1 && _index != 0)
            {
                //Lower the text box
                anim.SetBool("isOpen", false);

                yield return new WaitForSecondsRealtime(.2f);
                _index++;

                StartCoroutine(ReturnControlToPlayer());
                yield break;
            }

            Debug.Log("Next Line");

            _index++;

            //load next sentence
            dialogue.NextSentence();
            StartCoroutine(ShowBattleDialogue());
            yield break;
        }
    }

    //Sets up the battle scene
    void SetUpTutorialBattle()
    {
        Debug.Log("Add Enemy");

        //Adds enemy and sets up placement
        CombatController.instance.enemyList.Add(EnemyType.Tutorial_Intern);
        CombatController.instance.TutorialSetUp();

    }
}
