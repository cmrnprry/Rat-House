using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public GameObject sprite;
    public SpriteRenderer sr;
    public Animator anim;

    private Rigidbody _rb;
    private bool _isSusan = false;
    private bool _isFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        StartCoroutine(PlayerMovement());

        //Items the player starts with

        GameManager.instance.CollapseItemList(CombatController.instance.itemList);
    }

    //An Enumerator that controls the player movement
    public IEnumerator PlayerMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h * speed, 0.0f, v * speed);
        if (h > 0 || h < 0)
        {
            anim.SetBool("Left", true);
            anim.SetBool("Down", false);
            anim.SetBool("Up", false);

            if(h < 0)
            {
                sr.flipX = true;
            }
            else
            {
                sr.flipX = false;
            }
        }
        else if (h == 0 && v < 0)
        {
            anim.SetBool("Left", false);
            anim.SetBool("Down", true);
            anim.SetBool("Up", false);

        }
        else if (h == 0 && v > 0)
        {
            anim.SetBool("Left", false);
            anim.SetBool("Down", false);
            anim.SetBool("Up", true);
        }
        else
        {
            anim.SetBool("Left", false);
            anim.SetBool("Down", false);
            anim.SetBool("Up", false);
        }

        _rb.velocity = movement;

        if (Input.GetButton("SelectAction"))
        {
            if (_isSusan)
            {
                StopPlayerMovement();
                GameManager.instance.SetGameState(GameState.Susan);
                yield break;
            }
        }



        yield return new WaitForEndOfFrame();
        StartCoroutine(PlayerMovement());
    }

    public void StopPlayerMovement()
    {
        StopAllCoroutines();
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Susan")
        {
            _isSusan = true;
        }
    }

    void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Susan")
        {
            _isSusan = false;
        }
    }
}
