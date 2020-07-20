﻿using System.Collections;
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

    //Track Player Damage
    private bool _canHit;
    private int _amountHit = 0;
    private int _totalHits = 0;

    //base damage that attacks can do
    private List<float> _attackDamage;

    // Start is called before the first frame update
    void Start()
    {
        _attackDamage = CombatController.instance.attackDamage;

        //for each enemy on the board add their health to the list
        foreach (var e in CombatController.instance.enemyList)
        {
            enemyHealth.Add(50f);
        }
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
        var damage = _attackDamage[(int)CombatController.instance.selectedAction];

        damage = DamageModifier(damage);

        Debug.Log("Damange Dealt: " + damage);

        enemyHealth[enemyAttacked] -= damage;

        Debug.Log("Enemy Health: " + enemyHealth[enemyAttacked]);


        if (enemyHealth[enemyAttacked] <= 0)
        {
            //play enemy death animiation

            //remove enemy from list
            enemyHealth.Remove(enemyHealth[enemyAttacked]);

            Debug.Log("Enemy Dead");

            //If there are no more enemies, return to overworld
            if (enemyHealth.Count <= 0)
            {
                StartCoroutine(GameManager.instance.BattleWon());
            }
        }
        else
        {
            //After the damage has been delt we want to switch to the enemies turn
            StartCoroutine(CombatController.instance.EnemyPhase());
        }

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
            _totalHits++;
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
