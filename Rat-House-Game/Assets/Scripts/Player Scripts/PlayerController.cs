using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private Rigidbody _rb;
    private bool _canFight = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        StartCoroutine(PlayerMovement());
    }

    private void Update()
    {
        //if (Input.GetButtonDown("SelectAction"))
        //{
        //    Debug.Log("pressed enter");
        //    if (_canFight)
        //    {
        //        Debug.Log("Stop Movement and Start Battle");

        //        GameManager.instance.StartBattle();
        //        //StopCoroutine(PlayerMovement());
        //        _canFight = false;

        //        //yield break;
        //    }

        //}
    }

    //An Enumerator that controls the player movement
    IEnumerator PlayerMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h * speed, 0.0f, v * speed);

        _rb.velocity = movement;

        if (Input.GetButton("SelectAction"))
        {
            if (_canFight)
            {
                Debug.Log("Stop Movement and Start Battle");

                GameManager.instance.SetGameState(GameState.Battle);
                _canFight = false;

                yield break;
            }

        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(PlayerMovement());
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            Debug.Log("Press Enter to Start Combat");

            //tells the Combat Manager which enemies the player could possibly fight
            CombatController.instance.SetEnemies(collider.gameObject.GetComponent<EnemyController>().enemiesInBattle);
            _canFight = true;
        }
    }

    void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            _canFight = false;
            Debug.Log("No Combat");
        }
    }

}
