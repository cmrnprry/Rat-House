using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Dialogue : MonoBehaviour
{
    //Animator for the text box
    public Animator anim;
    public GameObject enterText;

    //Text Boxes
    public TextMeshProUGUI dia;
    public TextMeshProUGUI speakerName;

    //Array of the dialogue
    [TextArea(3, 5)]
    public string[] sentences;
    private string text = "";

    //Image of the person speaking
    public Image speakerHead;
    public Sprite[] joeheads;
    public Sprite[] waterheads;
    public Sprite[] coffeeheads;
    public Sprite[] internheads;
    public Sprite[] npcOneheads;
    public Sprite[] npcTwoheads;
    public Sprite[] susanheads;
    public Sprite[] computerheads;

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
        string[] speaker = set[0].Split('_');

        text = set[1];

        ChooseHead(speaker[0], Int32.Parse(speaker[1]));

        //int head = GetSpeakerHead(set[0]);
        //speakerHead.sprite = heads[head];

        StartCoroutine(Type());
    }

    void ChooseHead(string name, int type = 0)
    {
        speakerName.text = name;
        switch (name)
        {
            case "Joe":
                speakerHead.sprite = joeheads[type];
                break;
            case "Intern":
                speakerHead.sprite = internheads[type];
                break;
            case "Bill":
                speakerHead.sprite = waterheads[type];
                break;
            case "Robert":
                speakerHead.sprite = coffeeheads[type];
                break;
            case "???":
                speakerHead.sprite = computerheads[type];
                break;
            case "Susan":
                speakerHead.sprite = susanheads[type];
                break;
            case "Wilbur":
                speakerHead.sprite = npcOneheads[type];
                break;
            case "Jan":
                speakerHead.sprite = npcTwoheads[type];
                break;
            default:
                speakerHead.sprite = null;
                break;
        }
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
            speakerHead.sprite = joeheads[0];
            speakerName.text = "Joe";
        }
    }

    public IEnumerator Type()
    {
        isTyping = true;
        dia.alpha = 0;
        dia.text = text;

        yield return new WaitForSecondsRealtime(typingSpeed);


        dia.ForceMeshUpdate();
        TMP_TextInfo textInfo = dia.textInfo;
        int totalCharacters = dia.textInfo.characterCount;

        for (int i = 0; i < totalCharacters; i++)
        {
            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            var newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Set all to full alpha
            newVertexColors[vertexIndex + 0].a = 255;
            newVertexColors[vertexIndex + 1].a = 255;
            newVertexColors[vertexIndex + 2].a = 255;
            newVertexColors[vertexIndex + 3].a = 255;

            dia.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        dia.alpha = 225;
        yield return new WaitForSecondsRealtime(typingSpeed);


        isTyping = false;
    }

}
