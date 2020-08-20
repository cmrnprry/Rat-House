using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableText : MonoBehaviour
{
    public string[] itemComments;

    public bool playerInRange;
    private int lastRand;

    IEnumerator ShowInteractabeText()
    {
        //If you press space when the player is close enough...
        if (Input.GetButton("SelectAction") && !GameManager.instance.dialogueInProgress && GameManager.instance.GetGameState() != GameState.Battle)
        {
            GameManager.instance.dialogueInProgress = true;
            GameManager.instance.dialogueOver = false;

            GameManager.instance.diaAnim.SetBool("isOpen", true);
            GameManager.instance.dialogue.speakerName.text = "Joe";
            GameManager.instance.dialogue.dia.text = itemComments[RandNumber()];
            //GameManager.instance.dialogue.dia.text = itemComments[Random.Range(0, itemComments.Length)];

            yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

            //end dialogue
            GameManager.instance.diaAnim.SetBool("isOpen", false);
            GameManager.instance.dialogueInProgress = false;
            GameManager.instance.dialogueOver = true;

            playerInRange = false;
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
        return rand;
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
            GameManager.instance.diaAnim.SetBool("isOpen", false);
        }
    }
}
