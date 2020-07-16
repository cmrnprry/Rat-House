using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        StartCoroutine(PlayerMovement());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //An Enumerator that controls the player movement
    IEnumerator PlayerMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h * speed, 0.0f, v * speed);

        _rb.velocity = movement;

        yield return new WaitForEndOfFrame();
        StartCoroutine(PlayerMovement());
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            Debug.Log("Press Space to Start Combat");
        }
    }

    void OnCollisionStay(Collision collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine(PlayerMovement());
                GameManager.instance.StartBattle();
            }
        }
    }

    void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            Debug.Log("No Combat");
        }
    }

}
