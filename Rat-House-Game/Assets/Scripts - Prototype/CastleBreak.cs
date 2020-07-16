using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class CastleBreak : MonoBehaviour
{
    public bool canKick = false;
    private int kickCount = 0;
    SpriteRenderer castleSprite;

    void Start()
    {
        castleSprite = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (canKick == false)
            {
                canKick = true;
                Debug.Log("Player close enough to kick");
            }            
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(canKick == true)
            {
                canKick = false;
                Debug.Log("Player too far to kick");
            }            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(canKick == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                kickCount = kickCount + 1;
            }
            if (kickCount == 1)
            {
                castleSprite.color = Color.yellow;
                Debug.Log("Kicked once");
            }
            else if (kickCount == 2)
            {
                castleSprite.color = Color.red;
                Debug.Log("Kicked twice");
            }
            else if (kickCount == 3)
            {
                Debug.Log("Broke castle");
                Destroy(this.gameObject);
            }
        }        
    }
}
