using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ConvoState { START, PLAYERTALK, ENEMYTALK, END }
public class DialogueTrigger : MonoBehaviour
{
    //public List<string> randomDialogueList;
    public Animator anim;

    public bool playerInRange = false;

    public Array[] dialogue;

    public Text nameText;
    public Text dialogueText;

    public string enemyName;

    public ConvoState state;

    public void Start()
    {
        //dialogue = new string[];
    }
    private void FixedUpdate()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown("space"))
            {
                state = ConvoState.START;
                StartTalk();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }

    void StartTalk()
    {
        anim.SetBool("isOpen", true);

        state = ConvoState.ENEMYTALK;
        VillianTalk();
    }

    void VillianTalk()
    {
        nameText.text = enemyName;
    }
    //public Dialogue dialogue;

    //private bool canInteract = false;
    //public bool isPerson;

    //private void FixedUpdate()
    //{
    //    if (canInteract == true)
    //    {
    //        if (Input.GetKeyDown("space"))
    //        {
    //            TriggerDialogue();
    //        }
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    canInteract = true;
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    canInteract = false;
    //}

    //public void TriggerDialogue()
    //{
    //    if(isPerson == true)
    //    {
    //        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    //    }
    //}
}
