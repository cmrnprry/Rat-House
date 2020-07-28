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
    private bool _hitNote = false;
    private bool _shownMiss = false;
    private int _amountHit = 0;
    public static int _totalHits = 0;

    //Checks for player hitting on time
    private float _startHit;
    private float _emdHit;

    //List of the "perfect" hits of a given rhythm
    public static List<float> hitList = new List<float>();
    private float _currNote;

    //If the player hit the note too late or early
    public float offset;
    public float delta;

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
            //Debug.Log("Pos: " + transform.position.x);
            //Debug.Log("Pos Note: " + _currNote);

            if (Input.GetButtonDown("SelectAction"))
            {
                Debug.Log("Pressed Space");

                if (_canHit && !_hitNote)
                {
                    DetectHit(transform.localPosition);
                }
            }

            if (!_canHit)
            {
                //Debug.Log("Pos Note - offset: " + (_currNote + offset));
                //Debug.Log("Pos: " + transform.localPosition.x);
                //Debug.Log("Pos Note: " + _currNote);
                //if the player missed the note
                if (transform.localPosition.x < _currNote + offset && !_hitNote && !_shownMiss) //greater than the pos + offset
                {
                    //play MISS animation
                    Debug.Log("Miss!");
                }

                _hitNote = false;
                _shownMiss = true;
            }
        }
    }

    private void DetectHit(Vector3 pos)
    {
        //Debug.Log("Pos Note - offset: " + (_currNote - offset));
        //Debug.Log("Pos Note - delta: " + (_currNote - delta));
        //Debug.Log("Pos: " + transform.localPosition.x);
        //Debug.Log("Pos Note: " + _currNote);
        //if the player hits late
        if (pos.x >= _currNote + delta && pos.x <= _currNote + offset) //between the pos and offset
        {
            //play Late animation
            Debug.Log("Late!");
            _hitNote = true;
        }
        //if the player is "perfect"
        else if (pos.x <= _currNote + delta && pos.x >= _currNote - delta) //between the pos +/- delta
        {
            //play Perfect animation
            Debug.Log("Perfect!");
            _amountHit += 1;
            _hitNote = true;
        }
        //the player is early
        else if (pos.x >= _currNote - delta && pos.x <= _currNote - offset) //between the pos and -offset
        {
            //play Early animation
            Debug.Log("Early!");
            _hitNote = true;
        }


        ////if the player missed the note
        //if (pos.x > hitList[_index].x + offset) //greater than the pos + offset
        //{
        //    //play MISS animation
        //    Debug.Log("Miss!");
        //}
        ////if the player hits late
        //else if (pos.x >= hitList[_index].x + delta && pos.x <= hitList[_index].x + offset) //between the pos and offset
        //{
        //    //play Late animation
        //    Debug.Log("Late!");
        //}
        ////if the player is "perfect"
        //else if (pos.x >= hitList[_index].x + delta && pos.x <= hitList[_index].x - delta) //between the pos +/- delta
        //{
        //    //play Perfect animation
        //    Debug.Log("Perfect!");
        //    _amountHit += 1;
        //}
        ////the player is early
        //else if (pos.x >= hitList[_index].x + delta && pos.x <= hitList[_index].x + offset) //between the pos and -offset
        //{
        //    //play Early animation
        //    Debug.Log("Early!");
        //}
    }

    //Updates the player's health, both damage and healing
    public void UpdatePlayerHealth(float delta)
    {
        playerHealth += delta;

        if (playerHealth <= 0)
        {

        }
    }

    public void DealDamageToEnemy(int enemyAttacked = 0, bool isItem = false, int itemDmg = 0)
    {
        // if isItem is true set damage to item damage otherwise do the damage calculation
        var damage = isItem == true ? itemDmg : DamageModifier(_attackDamage[(int)CombatController.instance.selectedAction]);

        Debug.Log("Damange Dealt: " + damage);

        enemyHealth[enemyAttacked] -= damage;

        Debug.Log("Enemy Health: " + enemyHealth[enemyAttacked]);

        _amountHit = 0;

        if (enemyHealth[enemyAttacked] <= 0)
        {
            //play enemy death animiation
            Debug.Log("Enemy Dead");

            //decrease the number o f enemies left
            _enemiesLeft -= 1;

            //Destroy Enemy
            Destroy(CombatController.instance._inBattle[enemyAttacked].gameObject);

            Debug.Log("Enemy Dead: " + CombatController.instance._inBattle[enemyAttacked].name);

            //remove enemy from list(s)
            CombatController.instance._inBattle[enemyAttacked] = null;
            CombatController.instance.enemyList[enemyAttacked] = EnemyType.NULL;

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
            _currNote = other.gameObject.transform.localPosition.x;
            _shownMiss = false;

            //Debug.Log("Can hit");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Note")
        {
            _canHit = false;
          //  Debug.Log("Can NOT hit");
        }
    }
}
