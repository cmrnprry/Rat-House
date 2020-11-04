using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    private EnemyController enemyController;
    private EnemyMovement enemyMovement;

    public GameObject player;
    public GameObject exclamation;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.tempWait && !enemyController.isBeaten)
        {
            enemyMovement.canMove = true;
        }
        else if (other.gameObject == player && !GameManager.instance.dialogueInProgress && !GameManager.instance.transition)
        {
            Debug.Log("here");
            player.GetComponent<PlayerController>().StopPlayerMovement();
            exclamation.SetActive(true);
            enemyMovement.canMove = false;

            if (!enemyController.isBeaten)
            {
                Debug.Log("not beaten");
                //Set battle enemies
                CombatController.instance.SetEnemies(enemyController.enemiesInBattle);
                GameManager.instance.currEnemy = this.gameObject;

                //Set dialogue and enable the ability to start battle
                StartCoroutine(GameManager.instance.SetEnemyDialogue(enemyController.preBattleDialogue));
                StartCoroutine(StartBattle());
            }
            else if (enemyController.isBeaten)
            {
                Debug.Log("beaten");
                StartCoroutine(GameManager.instance.SetEnemyDialogue(enemyController.beatenBattleDialogue));
                StartCoroutine(GivePlayerBack());
            }

        }
    }

    IEnumerator StartBattle()
    {
        Debug.Log("start battle");
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => GameManager.instance.dialogueOver && !GameManager.instance.dialogueInProgress);

        exclamation.SetActive(false);
        GameManager.instance.SetGameState(GameState.Battle);
    }

    IEnumerator GivePlayerBack()
    {
        yield return new WaitUntil(() => GameManager.instance.dialogueOver && !GameManager.instance.dialogueInProgress);
        exclamation.SetActive(false);
        StartCoroutine(player.GetComponent<PlayerController>().PlayerMovement());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            exclamation.SetActive(false);
            GameManager.instance.tempWait = false;
        }
    }
}
