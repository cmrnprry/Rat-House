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

    //Image of the person speaking
    public Image speakerHead;

    //Array of speaker head sprites
    public Sprite[] heads;

    //Array of the dialogue
    [TextArea(3, 5)]
    public string[] sentences;
    private string text = "";

    //Where we are in the sentences array
    public int index;

    //How fast the text appears on screen (lower the number, the faster it is)
    public float typingSpeed;

    //bool to see if the text has finsihed displaying
    public bool isTyping = false;


    //At the start of a conversation...
    public void StartDialogue()
    {
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
        }


        return head;
    }

    //For each new sentence...
    public void NextSentence()
    {
        //if there are more sentences...
        if (index < sentences.Length - 1)
        {
            //load the next sentence, erase the previous one, and start typing
            index++;
            dia.text = "";
            text = "";

            SetDialogue();
        }
        //If there are no more sentences, close the dialogue box
        else
        {
            anim.SetBool("isOpen", false);
            dia.text = "";
            text = "";
        }
    }

    public IEnumerator Type()
    {
        isTyping = true;

        //Type each letter in the sentence one at a time at a speed of one letter per unit of typingSpeed
        foreach (char letter in text.ToCharArray())
        {
            dia.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

}
