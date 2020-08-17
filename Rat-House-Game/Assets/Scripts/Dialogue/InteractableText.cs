﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableText : MonoBehaviour
{
    public Animator anim;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public string[] itemComments;

    public bool playerInRange;


    IEnumerator ShowInteractabeText()
    {
        //If you press space when the player is close enough...
        if (Input.GetButton("SelectAction") && !GameManager.instance.dialogueInProgress && GameManager.instance.GetGameState() != GameState.Battle)
        {
            GameManager.instance.dialogueInProgress = true;
            GameManager.instance.dialogueOver = false;


            anim.SetBool("isOpen", true);
            nameText.text = "Joe";
            dialogueText.text = itemComments[Random.Range(0, itemComments.Length)];

            yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

            //end dialogue
            anim.SetBool("isOpen", false);
            GameManager.instance.dialogueInProgress = false;
            GameManager.instance.dialogueOver = true;

            playerInRange = false;
            yield break;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(ShowInteractabeText());
    }

    //Check if the player is close enough
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            StartCoroutine(ShowInteractabeText());
        }
    }

    //If the player moves too far...
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //close the text box
            playerInRange = false;
            StopAllCoroutines();
            anim.SetBool("isOpen", false);
        }
    }
}
