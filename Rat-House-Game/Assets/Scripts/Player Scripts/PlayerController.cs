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
    private bool _canFight = false;
    private bool _defeatedEnemy = false;
    private bool _wait = false;
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
        Debug.Log("smooving");
        Debug.Log("can Fight?: " + _canFight);
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h * speed, 0.0f, v * speed);
        if (h > 0 || h < 0)
        {
            anim.SetBool("Left", true);
            anim.SetBool("Down", false);
            anim.SetBool("Up", false);

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
            if (_canFight)
            {

                _canFight = false;
                if (_defeatedEnemy)
                {
                    StartCoroutine(GameManager.instance.ShowBeatenDialogue());
                    yield break;
                }
                else
                {
                    GameManager.instance.SetGameState(GameState.Battle);
                    Debug.Log("stop moving");
                    yield break;
                }
            }
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(PlayerMovement());
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            //tells the Combat Manager which enemies the player could possibly fight
            CombatController.instance.SetEnemies(collider.gameObject.GetComponent<EnemyController>().enemiesInBattle);
            GameManager.instance.currEnemy = collider.gameObject;
            _defeatedEnemy = collider.gameObject.GetComponent<EnemyController>().isBeaten;
            _canFight = true;
        }
    }

    void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            _canFight = false;
            
        }
    }


    protected void Flip()
    {
        _isFacingRight = !_isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
