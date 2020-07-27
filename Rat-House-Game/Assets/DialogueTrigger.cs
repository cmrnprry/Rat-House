using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public List<string> randomDialogueList;
    public Dialogue dialogue;

    private bool canInteract = false;
    private bool isPerson = false;

    private void Start()
    {
        randomDialogueList = new List<string>();

        if(gameObject.tag == "Enemy")
        {
            isPerson = true;
        }
        else
        {
            isPerson = false;
        }
    }

    private void FixedUpdate()
    {
        if (canInteract == true)
        {
            if (Input.GetKeyDown("space"))
            {
                TriggerDialogue();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        canInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        canInteract = false;
    }

    public void TriggerDialogue()
    {
        if(isPerson == true)
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
        else if(isPerson == false)
        {
            FindObjectOfType<DialogueManager>().StartRandomDialogue(dialogue);
        }
    }
}
