using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableText : MonoBehaviour
{
    public string[] itemComments;

    public bool playerInRange;
    public GameObject pressEnter;
    private int lastRand;

    private Animator diaAnim;
    private Dialogue dialogue;


    private void Start()
    {
        diaAnim = GameManager.instance.diaAnim;
        dialogue = GameManager.instance.dialogue;
    }

    IEnumerator ShowInteractabeText()
    {
        //If you press space when the player is close enough...
        if (Input.GetButton("SelectAction") && !GameManager.instance.dialogueInProgress && GameManager.instance.GetGameState() != GameState.Battle)
        {
            TurnOnDialogue();

            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

            //end dialogue
            TurnOffDialogue();

            yield break;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(ShowInteractabeText());
    }

    int RandNumber()
    {
        int rand = Random.Range(0, itemComments.Length);

        if (Random.Range(0, itemComments.Length) == lastRand)
        {
            return RandNumber();
        }

        Debug.Log("rand number: " + rand);
        lastRand = rand;
        return rand;
    }

    //Check if the player is close enough
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            pressEnter.SetActive(true);
            StartCoroutine(ShowInteractabeText());
        }
    }

    //If the player moves too far...
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //close the text box
            TurnOffDialogue();
        }
    }

    void TurnOffDialogue()
    {
        pressEnter.SetActive(false);
        playerInRange = false;
        diaAnim.SetBool("isOpen", false);
        GameManager.instance.dialogueInProgress = false;
        GameManager.instance.dialogueOver = true;

        StopAllCoroutines();
    }

    void TurnOnDialogue()
    {
        GameManager.instance.dialogueInProgress = true;
        GameManager.instance.dialogueOver = false;

        diaAnim.SetBool("isOpen", true);
        dialogue.speakerName.text = "Joe";
        dialogue.dia.text = itemComments[RandNumber()];
    }
}
