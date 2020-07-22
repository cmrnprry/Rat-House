using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStats : MonoBehaviour
{
    //Player Stats
    private float playerHealth = 100f;
    private float playerBaseAccuracy = 100f;

    //Enemy Stats
    private List<float> enemyHealth = new List<float>();
    private List<float> enemyBaseAccuracy;
    private int _enemiesLeft = 0;

    //Track Player Damage
    private bool _canHit;
    private int _amountHit = 0;
    public static int _totalHits = 0;

    //Checks for player hitting on time
    private float _startHit;
    private float _emdHit;

    //base damage that attacks can do
    private List<float> _attackDamage;

    // Sets the stats
    public void SetStats()
    {
        _attackDamage = CombatController.instance.attackDamage;

        //for each enemy on the board add their health to the list
        foreach (GameObject e in CombatController.instance._inBattle)
        {
            enemyHealth.Add(e.GetComponent<Enemy>().GetStartingHealth());
            _enemiesLeft++;
        }

        Debug.Log("Enemy Count: " + enemyHealth.Count);
    }

    private void Update()
    {
        //if the attact music is playing, then check if the player has hit or miss
        if (AudioManager.instance.attackMusic.isPlaying)
        {
            if (Input.GetButtonDown("SelectAction"))
            {
                Debug.Log("Pressed Space");
                if (_canHit)
                {
                    Debug.Log("Hit!");
                    _amountHit += 1;
                }
            }
        }
    }

    public void DealDamageToEnemy(int enemyAttacked = 0)
    {
        var damage = DamageModifier(_attackDamage[(int)CombatController.instance.selectedAction]);

        Debug.Log("Damange Dealt: " + damage);

        enemyHealth[enemyAttacked] -= damage;

        Debug.Log("Enemy Health: " + enemyHealth[enemyAttacked]);

        _amountHit = 0;

        if (enemyHealth[enemyAttacked] <= 0)
        {
            //play enemy death animiation

            //decrease the number o f enemies left
            _enemiesLeft -= 1;

            //Destroy Enemy
            Destroy(CombatController.instance._inBattle[enemyAttacked].gameObject);

            Debug.Log("Enemy Dead: " + CombatController.instance._inBattle[enemyAttacked].name);

            //remove enemy from list(s)
            CombatController.instance._inBattle[enemyAttacked] = null;
            //CombatController.instance.enemyList.Remove(CombatController.instance.enemyList[enemyAttacked]);

            Debug.Log(CombatController.instance._inBattle[enemyAttacked]);

            //If there are no more enemies, return to overworld
            if (_enemiesLeft <= 0)
            {
                StartCoroutine(GameManager.instance.BattleWon());
                return;
            }
        }

        //After the damage has been delt we want to switch to the enemies turn
        StartCoroutine(CombatController.instance.EnemyPhase());
    }

    float DamageModifier(float dmg)
    {
        var half = _totalHits / 2;
        var quareter = _totalHits / 4;

        if (_amountHit == _totalHits)
        {
            dmg *= 1.5f;
        }
        else if (_amountHit >= half)
        {
            dmg *= 1.2f;
        }
        else if (_amountHit >= quareter)
        {
            dmg *= 1.1f;
        }
        else if (_amountHit < 1)
        {
            dmg = 0;
        }


        return dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Note")
        {
            _canHit = true;

            Debug.Log("Can hit");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Note")
        {
            _canHit = false;
            Debug.Log("Can NOT hit");
        }
    }
}
