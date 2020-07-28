using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableText : MonoBehaviour
{
    public Animator anim;

    public Text nameText;
    public Text dialogueText;

    public string[] itemComments;

    public bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playerInRange)
        {
            if (anim.GetBool("isOpen") == true)
            {
                anim.SetBool("isOpen", false);
            }
            else
            {
                anim.SetBool("isOpen", true);
                dialogueText.text = itemComments[Random.Range(0, itemComments.Length)];
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            anim.SetBool("isOpen", false);
        }
    }
}
