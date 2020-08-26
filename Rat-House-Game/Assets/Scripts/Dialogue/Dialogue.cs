using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    //Animator for the text box
    public Animator anim;

    //Text Boxes
    public TextMeshProUGUI dia;
    public TextMeshProUGUI speakerName;

    //Array of the dialogue
    [TextArea(3, 5)]
    public string[] sentences;
    private string text = "";

    //Image of the person speaking
    public Image speakerHead;
    public Sprite[] heads;

    //Where we are in the sentences array
    public int index;

    //How fast the text appears on screen (lower the number, the faster it is)
    public float typingSpeed;

    //bool to see if the text has finsihed displaying
    public bool isTyping = false;


    //At the start of a conversation...
    public void StartDialogue()
    {
        Debug.Log("starts");

        dia.text = "";
        text = "";
        index = 0;
        SetDialogue();
    }

    //Set the speaker tag, image and text
    public void SetDialogue()
    {
        string[] set = sentences[index].Split(':');

        speakerName.text = set[0];
        text = set[1];

        int head = GetSpeakerHead(set[0]);
        speakerHead.sprite = heads[head];

        StartCoroutine(Type());
    }

    int GetSpeakerHead(string name)
    {
        int head = -1;

        switch (name)
        {
            case "Joe":
                head = 0;
                break;
            case "Intern":
                head = 1;
                break;
            case "Bill":
                head = 2;
                break;
            case "Robert":
                head = 3;
                break;
            case "???":
                head = 4;
                break;
            case "Susan":
                head = 5;
                break;
            case "Wilbur":
                head = 6;
                break;
            case "Jan":
                head = 7;
                break;
            default:
                head = 0;
                break;
        }

        return head;
    }

    //For each new sentence...
    public void NextSentence()
    {
        dia.text = "";
        text = "";

        //if there are more sentences...
        if (index < sentences.Length - 1)
        {
            //load the next sentence, erase the previous one, and start typing
            index++;
            SetDialogue();
        }
        //If there are no more sentences, close the dialogue box
        else
        {
            anim.SetBool("isOpen", false);
            speakerHead.sprite = heads[0];
            speakerName.text = "Joe";
        }
    }

    public IEnumerator Type()
    {
        isTyping = true;

        //Type each letter in the sentence one at a time at a speed of one letter per unit of typingSpeed
        foreach (char letter in text.ToCharArray())
        {
            dia.text += letter;

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

}
