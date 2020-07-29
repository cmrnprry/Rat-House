using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dia;
    public TextMeshProUGUI speakerName;

    public string[] speakers;
    public string[] sentences;

    [HideInInspector]
    public int index;

    public float typingSpeed;
    public bool isTyping = false;

    public Animator anim;

    //At the start of a conversation...
    public void StartDialogue()
    {
        //Set the first speaker's name, set the text to empty, start at the first sentence, and start typing
        speakerName.text = speakers[0];
        dia.text = "";
        index = 0;
        StartCoroutine(Type());
    }

    //For each new sentence...
    public void NextSentence()
    {
        //if there are more sentences...
        if(index < sentences.Length - 1)
        {
            //change who's speaking...
            if (speakerName.text == speakers[0])
            {
                speakerName.text = speakers[1];
            }
            else if (speakerName.text == speakers[1])
            {
                speakerName.text = speakers[0];
            }

            //load the next sentence, erase the previous one, and start typing
            index++;
            dia.text = "";
            StopAllCoroutines();
            StartCoroutine(Type());
        }
        //If there are no more sentences, close the dialogue box
        else
        {
            anim.SetBool("isOpen", false);
            dia.text = "";
        }
    }

    public IEnumerator Type()
    {
        isTyping = true;
        Debug.Log(isTyping);

        //Type each letter in the sentence one at a time at a speed of one letter per unit of typingSpeed
        foreach (char letter in sentences[index].ToCharArray())
        {
            dia.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        Debug.Log(isTyping);
    }
    
}
