using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public SpriteRenderer sr;
    private GameObject arrow = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sr.color = Color.gray;
            //Debug.Log("Pressed Space");

            Hit();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            sr.color = Color.white;
           // Debug.Log("Released Space");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "arrow")
        {
            arrow = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "arrow")
        {
            Miss();
        }
    }

    void Hit()
    {
        if (arrow != null)
        {
            arrow.SetActive(false);
            arrow = null;
            Debug.Log("Hit");
        }
    }

    void Miss()
    {
        if (arrow != null)
        {
            arrow = null;
            Debug.Log("Miss");
        }
    }
}
