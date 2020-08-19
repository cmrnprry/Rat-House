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



        if ((h > 0 || h < 0) && h!= 0)
        {
            anim.SetBool("Left", true);

            sr.flipX = h < 0 ? true : false;
        }
        else
        {
            anim.SetBool("Left", false);

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
        else if (Input.GetButton("OpenInventory"))
        {
            GameManager.instance.OpenInventory();
            yield return new WaitForSecondsRealtime(0.2f);
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
