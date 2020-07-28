using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public bool playerInRange;
    public bool convoInProgress = false;

    public Animator anim;

    public Dialogue dialogue;

    // Update is called once per frame
    void FixedUpdate()
    {
        //If the player is close enough and they're not already talking...
        if(playerInRange == true && convoInProgress == false)
        {
            //when you hit space...
            if (Input.GetButtonDown("SelectAction"))
            {
                //open the text box and start dialogue
                anim.SetBool("isOpen", true);
                dialogue.StartDialogue();
                convoInProgress = true;
            }
        }
        //If the player is close enough and already talking...
        else if (playerInRange == true && convoInProgress == true)
        {
            //when you press space...
            if (Input.GetButtonDown("SelectAction"))
            {
                //load next sentence
                dialogue.NextSentence();
            }
        }
    }

    //Check if the player is close enough
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }

    //Check if the player is too far
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Reset convo and close text box
            playerInRange = false;
            convoInProgress = false;
            anim.SetBool("isOpen", false);
        }
    }
}
