using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    private EnemyController enemyController;
    private EnemyMovement enemyMovement;

    public bool playerInSight = false;

    private GameObject player;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyMovement = GetComponent<EnemyMovement>();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            playerInSight = true;
            Debug.Log("I see the player");

            if (enemyController.isBeaten != true)
            {
                enemyMovement.canMove = false;
                GameManager.instance.SetEnemyDialogue(enemyController.preBattleDialogue);
            }
            else if (enemyController.isBeaten == true)
            {
                enemyMovement.canMove = false;
                GameManager.instance.SetEnemyDialogue(enemyController.beatenBattleDialogue);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            playerInSight = false;

            //if (GameManager.instance.diaAnim.GetBool("isOpen"))
            //{
                GameManager.instance.diaAnim.SetBool("isOpen", false);
            //}
        }
    }
}
