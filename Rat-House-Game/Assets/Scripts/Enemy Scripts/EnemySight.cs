using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    private EnemyController enemyController;
    private EnemyMovement enemyMovement;

    public GameObject player;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && !GameManager.instance.dialogueInProgress)
        {
            enemyMovement.canMove = false;

            player.GetComponent<PlayerController>().StopPlayerMovement();

            if (!GameManager.instance.postBattle)
            {
                if (!enemyController.isBeaten)
                {
                    //Set battle enemies
                    CombatController.instance.SetEnemies(enemyController.enemiesInBattle);
                    GameManager.instance.currEnemy = this.gameObject;

                    //Set dialogue and enable the ability to start battle
                    GameManager.instance.SetEnemyDialogue(enemyController.preBattleDialogue);
                    StartCoroutine(StartBattle());
                    GameManager.instance.postBattle = true;
                }
                else if (enemyController.isBeaten)
                {
                    GameManager.instance.SetEnemyDialogue(enemyController.beatenBattleDialogue);
                    StartCoroutine(GivePlayerBack());
                }
            }
        }
    }

    IEnumerator StartBattle()
    {
        yield return new WaitUntil(() => GameManager.instance.dialogueOver);
        GameManager.instance.SetGameState(GameState.Battle);
    }

    IEnumerator GivePlayerBack()
    {
        yield return new WaitUntil(() => GameManager.instance.dialogueOver);
        StartCoroutine(player.GetComponent<PlayerController>().PlayerMovement());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            GameManager.instance.diaAnim.SetBool("isOpen", false);
        }
    }
}
