using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PleasantTalk : MonoBehaviour
{
    private bool playerInRange = false;

    private int _index = 0;

    //public Animator anim;
    private Dialogue dialogue;

    //public TextMeshProUGUI nameText;
    //public TextMeshProUGUI dialogueText;

    public GameObject player;

    [TextArea(3, 5)]
    public string[] aNiceConversation;

    private void Start()
    {
        dialogue = GetComponent<Dialogue>();
    }

    public void SetNPCDialogue()
    {
        //if there's no dialogue to be set
        if (aNiceConversation.Length <= 0)
        {
            GameManager.instance.dialogueOver = true;
            GameManager.instance.dialogueInProgress = false;
            return;
        }

        //stop playermovement and set bools
        player.GetComponent<PlayerController>().StopPlayerMovement();
        GameManager.instance.dialogueInProgress = true;
        GameManager.instance.dialogueOver = false;

        //turn on textbox and start dialogue
        GameManager.instance.diaAnim.SetBool("isOpen", true);
        dialogue.sentences = aNiceConversation;
        dialogue.StartDialogue();
        StartCoroutine(HaveANiceConversation());
    }

    IEnumerator HaveANiceConversation()
    {
        //Waits for the text to stop typing
        yield return new WaitUntil(() => dialogue.isTyping == false);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == dialogue.sentences.Length)
        {
            //Lower the text box
            GameManager.instance.diaAnim.SetBool("isOpen", false);

            //reset the index to 0
            _index = 0;

            yield return new WaitForSecondsRealtime(.2f);

            GameManager.instance.dialogueOver = true;
            GameManager.instance.dialogueInProgress = false;

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
            SetNPCDialogue();

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            StopAllCoroutines();
        }
    }
}
