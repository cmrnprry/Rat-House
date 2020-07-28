//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public enum ConvoState { START, PLAYERTALK, OTHERTALK, END }
//public class DialogueManager : MonoBehaviour
//{
//    public ConvoState state;

//    public Animator anim;

//    public Text nameText;
//    public Text dialogueText;

//    //private
//    private Queue<string> sentences;

//    // Start is called before the first frame update
//    void Start()
//    {
//        sentences = new Queue<string>();
//    }

//    public void StartDialogue(Dialogue dialogue)
//    {
//        state = ConvoState.START;

//        anim.SetBool("isOpen", true);
//        //nameText.text = dialogue.name;

//        sentences.Clear();

//        //foreach (string sentence in dialogue.sentences)
//        //{
//        //    sentences.Enqueue(sentence);
//        //}

//        //state = ConvoState.OTHERTALK;
//        //DisplayNextSentence();
//        if (Input.GetKeyDown("space"))
//        {
//            Debug.Log("Hit space");
//        }

//        state = ConvoState.OTHERTALK;
//        DisplayEvilSentence(dialogue);
//    }

//    public void DisplayEvilSentence(Dialogue dialogue)
//    {
//        nameText.text = dialogue.enemyName;

//        foreach (string sentence in dialogue.sentences)
//        {
//            sentences.Enqueue(sentence);
//        }

//        if (sentences.Count == 0)
//        {
//            state = ConvoState.END;
//            EndDialogue();
//        }
//    }

//    public void EndDialogue()
//    {
//        anim.SetBool("isOpen", false);
//    }

//    //public void DisplayNextSentence()
//    //{
//    //    if (sentences.Count == 0)
//    //    {
//    //        EndDialogue();
//    //        return;
//    //    }

//    //    string sentence = sentences.Dequeue();
//    //    StopAllCoroutines();
//    //    StartCoroutine(TypeSentence(sentence));
//    //}

//    //IEnumerator TypeSentence(string sentence)
//    //{
//    //    dialogueText.text = "";
//    //    foreach (char letter in sentence.ToCharArray())
//    //    {
//    //        dialogueText.text += letter;
//    //        yield return null;
//    //    }
//    //}
//    //public void EndDialogue()
//    //{
//    //    anim.SetBool("isOpen", false);
//    //}
//}
