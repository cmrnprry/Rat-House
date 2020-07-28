using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Animator anim;

    public Text nameText;
    public Text dialogueText;

    //private List<string> randomSentences;
    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        //randomSentences = new List<string>();
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        anim.SetBool("isOpen", true);
        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    //public void StartRandomDialogue(Dialogue dialogue)
    //{
    //    anim.SetBool("isOpen", true);
    //    nameText.text = dialogue.name;

    //    sentences.Clear();

    //    foreach (string sentence in dialogue.sentences)
    //    {
    //        sentences.Enqueue(sentence);
    //    }

    //    CloseItemBox();
    //}

    public void CloseItemBox()
    {
        anim.SetBool("isOpen", false);
        string sentence = sentences.Dequeue();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }
    public void EndDialogue()
    {
        anim.SetBool("isOpen", false);
    }
}
