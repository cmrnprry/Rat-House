using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatStats : MonoBehaviour
{
    //Player Stats
    public float playerHealth;

    //Enemy Stats
    private List<float> enemyHealth;
    private List<float> enemyBaseAccuracy;
    private int _enemiesLeft = 0;

    //Track Player Damage
    public static bool hitNote = false;
    public static float amountHit = 0;
    public static int totalHits = 0;

    //List of the "perfect" hits of a given rhythm
    public static List<float> hitList = new List<float>();
    public static int index = 0;

    //If the player hit the note too late or early
    public float offset;
    public float delta;

    //base damage that attacks can do
    private List<float> _attackDamage;

    public int action = 0;
    public AudioClip[] actionSounds;
    public AudioSource source;

    public void SetStats()
    {
        enemyHealth = new List<float>();
        _attackDamage = CombatController.instance.attackDamage;

        //Player Stats
        playerHealth = 100f;
        CombatController.instance.playerHealthText.text = playerHealth + "%";

        //Update the player health slider
        CombatController.instance.playerHealthSlider.value = 1f;

        //for each enemy on the board add their health to the list
        foreach (GameObject e in CombatController.instance._inBattle)
        {
            enemyHealth.Add(e.GetComponent<Enemy>().GetStartingHealth());
            _enemiesLeft++;
        }
    }

    private void Start()
    {
        source = AudioManager.instance.attackMusic;
    }

    private void Update()
    {
        //if the attact music is playing, then check if the player has hit or miss
        if (AudioManager.instance.startAction && index < hitList.Count)
        {
            //inceasese the index if the player misses OR hits a note since the cannot do both
            if (Input.GetButtonDown("SelectAction"))
            {
                //if the slider is within the offset range
                //Basically if it's at the start bounds of being early and the far bounds of being 
                if (transform.position.x >= hitList[index] - offset && transform.position.x <= hitList[index] + offset && !hitNote)
                {
                    StartCoroutine(ShowText());
                    DetectAttackHit(transform.position);
                }
            }

            //if the note wasn't hit, then show that it was missed
            if (!hitNote && transform.position.x > hitList[index] + offset)
            {
                if (transform.position.x > hitList[index] + offset && !hitNote) //greater than the pos + offset
                {
                    //play MISS animation
                    CombatController.instance.hitDetectionText.text = "Miss!";
                    StartCoroutine(ShowText());
                    index++;
                }

                hitNote = false;
            }
            // if the note has been hit and is past the late offset
            else if (hitNote && transform.position.x > hitList[index - 1] + offset)
            {
                hitNote = false;
            }
        }
        else if (AudioManager.instance.startDodge && index < hitList.Count)
        {
            //inceasese the index if the player misses OR hits a note since the cannot do both
            if (Input.GetButtonDown("SelectAction"))
            {
                //if the slider is within the offset range
                //Basically if it's at the start bounds of being early and the far bounds of being 
                if (transform.position.x >= hitList[index] - offset && transform.position.x <= hitList[index] + offset && !hitNote)
                {
                    CombatController.instance.hitDetectionText.gameObject.SetActive(true);
                    DetectDodgeHit(transform.position);
                }
            }

            //if the note wasn't hit, then show that it was missed
            if (!hitNote && transform.position.x < hitList[index] - offset)
            {
                if (transform.position.x < hitList[index] - offset && !hitNote) //greater than the pos + offset
                {
                    //play MISS animation
                    CombatController.instance.hitDetectionText.text = "Miss!";
                    CombatController.instance.hitDetectionText.gameObject.SetActive(true);
                    Debug.Log("Miss!");
                    index++;
                }

                hitNote = false;
            }
            // if the note has been hit and is past the late offset
            else if (hitNote && transform.position.x < hitList[index - 1] - offset)
            {
                hitNote = false;
            }
        }
    }

    IEnumerator ShowText()
    {
        CombatController.instance.hitDetectionText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(AudioManager.instance.beatsPerSec);
        CombatController.instance.hitDetectionText.gameObject.SetActive(false);
    }
    
    private void DetectAttackHit(Vector3 pos)
    {
        //if the player hits late
        if (pos.x > hitList[index] + delta && pos.x <= hitList[index] + offset) //between the pos and offset
        {
            //play Late animation
            CombatController.instance.hitDetectionText.text = "Late!";
            Debug.Log("Late!");
            amountHit += .5f;
        }
        //if the player is "perfect"
        else if (pos.x <= hitList[index] + delta && pos.x >= hitList[index] - delta) //between the pos +/- delta
        {
            //play Perfect animation
            CombatController.instance.hitDetectionText.text = "Perfect!";
            Debug.Log("Perfect!");
            amountHit += 1;
        }
        //the player is early
        else if (pos.x < hitList[index] - delta && pos.x >= hitList[index] - offset) //between the pos and -offset
        {
            //play Early animation
            CombatController.instance.hitDetectionText.text = "Early!";
            Debug.Log("Early!");
            amountHit += .5f;
        }

        Debug.Log("Hit at: " + transform.position.x);
        Debug.Log("Beat to hit at: " + hitList[index]);
        PlayRandomAttackClip();
        index++;
        hitNote = true;
    }

    void PlayRandomAttackClip()
    {
        var random = Random.Range(0, 2);
        source.clip = actionSounds[random];
        source.Play();
    }

    private void DetectDodgeHit(Vector3 pos)
    {
        Debug.Log("hit detected");

        //if the player hits Early
        if (pos.x > hitList[index] + delta && pos.x <= hitList[index] + offset) //between the pos and offset
        {
            //play Late animation
            CombatController.instance.hitDetectionText.text = "Early!";
            Debug.Log("Early!");
            amountHit += .5f;
        }
        //if the player is "perfect"
        else if (pos.x <= hitList[index] + delta && pos.x >= hitList[index] - delta) //between the pos +/- delta
        {
            //play Perfect animation
            CombatController.instance.hitDetectionText.text = "Perfect!";
            Debug.Log("Perfect!");
            amountHit += 1;
        }
        //the player is late
        else if (pos.x < hitList[index] - delta && pos.x >= hitList[index] - offset) //between the pos and -offset
        {
            //play Early animation
            CombatController.instance.hitDetectionText.text = "Late!";
            Debug.Log("Late!");
            amountHit += .5f;
        }

        Debug.Log("Hit at: " + transform.position.x);
        Debug.Log("Beat to hit at: " + hitList[index]);
        index++;
        hitNote = true;
    }

    //Updates the player's health, both damage and healing
    public void UpdatePlayerHealth(float delta)
    {
        //If the player was hit
        if (delta < 0)
        {
            delta = (totalHits == 0 ? delta : EnemyDamageModifier(delta));
        }

        playerHealth += delta;

        Debug.Log("Damage Taken: " + delta);

        //Make the health a number between 0 and 1
        float health = playerHealth / 100;

        //Update the player health slider
        CombatController.instance.playerHealthSlider.value = health;
        CombatController.instance.playerHealthText.text = playerHealth + "%";

        totalHits = 0;
        amountHit = 0;

        //If the player dies
        if (playerHealth <= 0)
        {
            Debug.Log("Joe is Dead");
            StartCoroutine(GameManager.instance.BattleLost());
        }
    }

    public IEnumerator DealDamageToEnemy(int enemyAttacked = 0, bool isItem = false, float itemDmg = 0)
    {
        //TODO: Determine if it's a good splash screen or bad
        //TODO: Add in some animation to make this look cooler
        CombatController.instance.splashScreens[action].gameObject.SetActive(true);

        // if isItem is true set damage to item damage otherwise do the damage calculation
        var damage = (isItem == true ? itemDmg : PlayerDamageModifier(_attackDamage[(int)CombatController.instance.selectedAction]));

        Debug.Log("Damage: " + damage);

        enemyHealth[enemyAttacked] -= damage;
        CombatController.instance._inBattle[enemyAttacked].GetComponent<Enemy>().UpdateHealth(damage);

        Debug.Log("enemy Attacked: " + CombatController.instance.enemyList[enemyAttacked].ToString());
        Debug.Log("enemy health after attack: " + enemyHealth[enemyAttacked]);

        yield return new WaitForEndOfFrame();

        yield return new WaitForSecondsRealtime(1f);

        CombatController.instance.splashScreens[action].gameObject.SetActive(false);

        if (enemyHealth[enemyAttacked] <= 0)
        {
            EnemyDeath(enemyAttacked);

            //If there are no more enemies, return to overworld
            if (_enemiesLeft <= 0)
            {
                StartCoroutine(GameManager.instance.BattleWon());
                yield break;
            }
        }

        yield return new WaitForEndOfFrame();

        //Reset the # of total hits and amount it
        totalHits = 0;
        amountHit = 0;

        //After the damage has been delt we want to switch to the enemies turn
        StartCoroutine(CombatController.instance.EnemyPhase());
    }

    //Handles what happens when an enemy dies
    void EnemyDeath(int enemyAttacked)
    {
        CombatController.instance.enemyDeath.Play();

        //play enemy death animiation
        Debug.Log("Enemy Dead");

        //decrease the number o f enemies left
        _enemiesLeft -= 1;

        //Destroy Enemy
        Destroy(CombatController.instance._inBattle[enemyAttacked].gameObject);

        Debug.Log("Enemy Dead: " + CombatController.instance._inBattle[enemyAttacked].name);

        //remove enemy from list(s) && turn off health
        CombatController.instance._inBattle[enemyAttacked] = null;
        CombatController.instance.enemyHealthBars[enemyAttacked].gameObject.SetActive(false);
    }

    float EnemyDamageModifier(float dmg)
    {
        Debug.Log("amount hit: " + amountHit);
        Debug.Log("total hit: " + totalHits);

        var half = totalHits / 2;
        var quareter = totalHits / 4;

        //if hit all, take no damage
        if (amountHit == totalHits)
        {
            dmg = 0;
        }
        else if (amountHit >= half) //if half, take half damage
        {
            dmg *= .5f;
        }
        else if (amountHit >= quareter) //if quarter, take 3/4
        {
            dmg *= .75f;
        }

        return dmg;
    }

    float PlayerDamageModifier(float dmg)
    {
        Debug.Log("amount hit: " + amountHit);
        Debug.Log("total hit: " + totalHits);
        var half = totalHits / 2;
        var quareter = totalHits / 4;

        if (amountHit == totalHits)
        {
            dmg *= 1.5f;
        }
        else if (amountHit >= half)
        {
            dmg *= 1.2f;
        }
        else if (amountHit >= quareter)
        {
            dmg *= 1.1f;
        }
        else
        {
            dmg = 0;
        }


        return dmg;
    }
}
