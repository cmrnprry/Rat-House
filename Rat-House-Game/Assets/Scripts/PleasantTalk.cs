﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PleasantTalk : MonoBehaviour
{
    private bool playerInRange = false;

    public bool dialogueOver = false;
    public bool dialogueInProgress = false;

    private int _index = 0;

    public Animator anim;

    public Dialogue dialogue;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public GameObject player;

    [TextArea(3, 5)]
    public string[] aNiceConversation;

    public void SetNPCDialogue(string[] dia)
    {
        //if there's no dialogue to be set
        if (dia.Length <= 0)
        {
            dialogueOver = true;
            dialogueInProgress = false;
            return;
        }

        dialogueInProgress = true;
        anim.SetBool("isOpen", true);
        dialogue.sentences = dia;
        dialogue.StartDialogue();
        StartCoroutine(HaveANiceConversation());
    }

    IEnumerator HaveANiceConversation()
    {
        player.GetComponent<PlayerController>().StopPlayerMovement();

        //Waits for the text to stop typing
        yield return new WaitUntil(() => dialogue.isTyping == false);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == dialogue.sentences.Length)
        {
            //Lower the text box
            anim.SetBool("isOpen", false);

            //reset the index to 0
            _index = 0;

            yield return new WaitForSecondsRealtime(.2f);

            dialogueOver = true;
            dialogueInProgress = false;

            //yield return new WaitUntil(() => GameManager.instance.dialogueOver && !GameManager.instance.dialogueInProgress);
            StartCoroutine(player.GetComponent<PlayerController>().PlayerMovement());

            yield break;
        }

        //increase the index
        _index++;

        //load next sentence
        dialogue.NextSentence();

        //Restart the coroutine
        StartCoroutine(HaveANiceConversation());
        yield break;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            playerInRange = true;
            SetNPCDialogue(aNiceConversation);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopAllCoroutines();
            //anim.SetBool("isOpen", false);
        }
    }
}