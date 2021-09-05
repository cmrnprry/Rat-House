    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PleasantTalk : MonoBehaviour
{
    private bool playerInRange = false;
    private int _index = 0;

    public GameObject player;
    public GameObject talking;

    [TextArea(3, 5)]
    public string[] aNiceConversation;

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
        GameManager.instance.dialogue.sentences = aNiceConversation;
        GameManager.instance.dialogue.StartDialogue();
        StartCoroutine(HaveANiceConversation());
    }

    IEnumerator HaveANiceConversation()
    {
        yield return new WaitUntil(() => GameManager.instance.dialogue.isTyping == false);
        yield return new WaitUntil(() => Input.GetButton("SelectAction") == false);

        GameManager.instance.dialogue.enterText.SetActive(true);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));
        GameManager.instance.dialogue.enterText.SetActive(false);

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == GameManager.instance.dialogue.sentences.Length - 1)
        {
            //Lower the text box
            TurnOffDialogue();

            //reset the index to 0
            _index = 0;

            StartCoroutine(player.GetComponent<PlayerController>().PlayerMovement());

            yield break;
        }

        //increase the index
        _index++;

        //load next sentence
        GameManager.instance.dialogue.NextSentence();

        //Restart the coroutine
        StartCoroutine(HaveANiceConversation());
        yield break;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            playerInRange = true;
            talking.SetActive(true);
            SetNPCDialogue();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TurnOffDialogue();
        }
    }

    void TurnOffDialogue()
    {
        playerInRange = false;
        talking.SetActive(false);
        GameManager.instance.diaAnim.SetBool("isOpen", false);
        GameManager.instance.dialogueInProgress = false;
        GameManager.instance.dialogueOver = true;

        StopAllCoroutines();
    }
}
