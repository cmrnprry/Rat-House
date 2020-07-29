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

    // Update is called once per frame
    void Update()
    {
        //If you press space when the player is close enough...
        if (Input.GetButtonDown("SelectAction") && playerInRange)
        {
            //close the text box if it's open...
            if (anim.GetBool("isOpen") == true)
            {
                anim.SetBool("isOpen", false);
            }
            //or open it if it's closed
            else
            {
                anim.SetBool("isOpen", true);
                dialogueText.text = itemComments[Random.Range(0, itemComments.Length - 1)];
            }
        }
    }

    //Check if the player is close enough
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    //If the player moves too far...
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //close the text box
            playerInRange = false;
            anim.SetBool("isOpen", false);
        }
    }
}