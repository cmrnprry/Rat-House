using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScript : MonoBehaviour
{
    //Where in the dialogue to show
    private int _index = 0;
    private bool hasHit = false;

    public Animator anim;

    public Dialogue dialogue;

    public IEnumerator ShowStartDialogue()
    {
        Debug.Log("Show Dialogue");

        while (dialogue.isTyping)
        {
            Debug.Log("Wait until done typing");
            yield return null;
        }

        //wait for the player to press enter/space
        while (!Input.GetButton("SelectAction"))
        {
            Debug.Log("waiting for player to hit enter/space");
            yield return null;
        }

        Debug.Log(dialogue.isTyping);


        //when you press space...
        if (Input.GetButton("SelectAction"))
        {

            if (_index == dialogue.sentences.Length)
            {
                //Lower the text box
                anim.SetBool("isOpen", false);

                StartTutorialBattle();
                yield break;
            }

            Debug.Log("Next Line");

            if (_index == 0 || _index == 5)
            {
                //trick the code into thinking the other person is talking
                dialogue.speakerName.text = dialogue.speakers[1];
            }

            _index++;

            //load next sentence
            dialogue.NextSentence();
            // dialogue.index += 1;

            StartCoroutine(ShowStartDialogue());
            yield break;
        }
    }

    void StartTutorialBattle()
    {
        //load correct scene
        Debug.Log("start Tutorial Battle");
        SceneManager.LoadScene("Tutorial_Battle-FINAL");
    }
}
