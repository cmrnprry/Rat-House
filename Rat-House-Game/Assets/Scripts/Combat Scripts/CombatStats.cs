using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public struct Beat
{
    //f = pos
    //b = true if it's a square
    public Beat(float f, bool b)
    {
        pos = f;
        isSquare = b;
    }

    public float pos { get; set; }
    public bool isSquare { get; set; }
}

public class CombatStats : MonoBehaviour
{
    //Player Stats
    public float playerHealth;
    public GameObject player;

    //status effect tracking
    private Enemy e;
    private StatusEffect effect;
    public bool hasEffect = false;
    private int turnsUntilEffectOver;

    //Enemy Stats
    [HideInInspector]
    public List<float> enemyHealth;
    private List<float> enemyBaseAccuracy;
    private int _enemiesLeft = 0;

    //Track Player Damage
    public static bool hitNote = false;
    public static float amountHit = 0;
    public static int totalHits = 0;
    public Items itemUsed;

    //List of the "perfect" hits of a given rhythm
    public static List<Beat> hitList = new List<Beat>();
    public static int index = 0;

    //If the player hit the note too late or early
    public float offset;
    public float delta;

    //base damage that attacks can do
    private List<float> _attackDamage;

    //list of attack sounds
    [HideInInspector]
    public AudioClip[] actionSounds;

    public int action = 0;

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
            var h = (e.name == "Susan(Clone)") ? GameManager.instance.susan.GetStartingHealth() : e.GetComponent<Enemy>().GetStartingHealth();
            enemyHealth.Add(h);
        }

        _enemiesLeft = CombatController.instance._inBattle.Count;
    }

    private void Update()
    {
        //if the attact music is playing, then check if the player has hit or miss
        if (AudioManager.instance.startAction && index < hitList.Count)
        {
            //inceasese the index if the player misses OR hits a note since the cannot do both
            if (Input.GetButtonDown("Circle") && !hitList[index].isSquare) //circle
            {
                Debug.Log("hit circle");
                //if the slider is within the offset range
                //Basically if it's at the start bounds of being early and the far bounds of being 
                if (transform.position.x >= hitList[index].pos - offset && transform.position.x <= hitList[index].pos + offset && !hitNote)
                {
                    StartCoroutine(ShowText());
                    DetectAttackHit(transform.position);
                }
            }

            //inceasese the index if the player misses OR hits a note since the cannot do both
            if (Input.GetButtonDown("Square") && hitList[index].isSquare) // square
            {
                Debug.Log("hit square");

                //if the slider is within the offset range
                //Basically if it's at the start bounds of being early and the far bounds of being 
                if (transform.position.x >= hitList[index].pos - offset && transform.position.x <= hitList[index].pos + offset && !hitNote)
                {
                    StartCoroutine(ShowText());
                    DetectAttackHit(transform.position);
                }
            }

            //if the note wasn't hit, then show that it was missed
            if (!hitNote && transform.position.x > hitList[index].pos + offset)
            {
                if (transform.position.x > hitList[index].pos + offset && !hitNote) //greater than the pos + offset
                {
                    //source.clip = missSound;
                    //   source.Play();

                    CombatController.instance.hitDetectionText.text = "Miss!";
                    StartCoroutine(ShowText());
                    index++;
                }

                hitNote = false;
            }
            // if the note has been hit and is past the late offset
            else if (hitNote && transform.position.x > hitList[index - 1].pos + offset)
            {
                hitNote = false;
            }
        }
        else if (AudioManager.instance.startDodge && index < hitList.Count)
        {
            //inceasese the index if the player misses OR hits a note since the cannot do both
            if (Input.GetButtonDown("Circle") && !hitList[index].isSquare) //circle
            {
                //if the slider is within the offset range
                //Basically if it's at the start bounds of being early and the far bounds of being 
                if (transform.position.x >= hitList[index].pos - offset && transform.position.x <= hitList[index].pos + offset && !hitNote)
                {
                    StartCoroutine(ShowText());
                    DetectDodgeHit(transform.position);
                }
            }

            //inceasese the index if the player misses OR hits a note since the cannot do both
            if (Input.GetButtonDown("Square") && hitList[index].isSquare) // square
            {

                //if the slider is within the offset range
                //Basically if it's at the start bounds of being early and the far bounds of being 
                if (transform.position.x >= hitList[index].pos - offset && transform.position.x <= hitList[index].pos + offset && !hitNote)
                {
                    StartCoroutine(ShowText());
                    DetectDodgeHit(transform.position);
                }
            }

            //if the note wasn't hit, then show that it was missed
            if (!hitNote && transform.position.x < hitList[index].pos - offset)
            {
                if (transform.position.x < hitList[index].pos - offset && !hitNote) //greater than the pos + offset
                {
                    //source.clip = missSound;
                    // source.Play();

                    CombatController.instance.hitDetectionText.text = "Miss!";
                    CombatController.instance.hitDetectionText.gameObject.SetActive(true);
                    index++;
                }

                hitNote = false;
            }
            // if the note has been hit and is past the late offset
            else if (hitNote && transform.position.x < hitList[index - 1].pos - offset)
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
        if (pos.x > hitList[index].pos + delta && pos.x <= hitList[index].pos + offset) //between the pos and offset
        {
            //play Late animation
            CombatController.instance.hitDetectionText.text = "Late!";
            Debug.Log("Late!");
            amountHit += .5f;
        }
        //if the player is "perfect"
        else if (pos.x <= hitList[index].pos + delta)// && pos.x >= hitList[index].pos - delta) //between the pos +/- delta
        {
            //play Perfect animation
            CombatController.instance.hitDetectionText.text = "Perfect!";
            Debug.Log("Perfect!");
            amountHit += 1;
        }
        //the player is early
        else if (pos.x < hitList[index].pos - delta && pos.x >= hitList[index].pos - offset) //between the pos and -offset
        {
            //play Early animation
            CombatController.instance.hitDetectionText.text = "Early!";
            Debug.Log("Early!");
            amountHit += .5f;
        }

        Debug.Log("Hit at: " + transform.position.x);
        Debug.Log("Beat to hit at: " + hitList[index]);
        Debug.Log("Beat in song: " + AudioManager.instance.songPositionInBeats);
        Debug.Log("Beat in song (sec): " + AudioManager.instance.songPosition);
        PlayRandomAttackClip();
        index++;
        hitNote = true;
    }

    void PlayRandomAttackClip()
    {
        var random = Random.Range(0, 2);
        AudioManager.instance.SFX.clip = actionSounds[random];
        AudioManager.instance.SFX.Play();
    }

    private void DetectDodgeHit(Vector3 pos)
    {
        //if the player hits Early
        if (pos.x > hitList[index].pos + delta && pos.x <= hitList[index].pos + offset) //between the pos and offset
        {
            //play Late animation
            CombatController.instance.hitDetectionText.text = "Early!";
            Debug.Log("Early!");
            amountHit += .5f;
        }
        //if the player is "perfect"
        else if (pos.x <= hitList[index].pos + delta && pos.x >= hitList[index].pos - delta) //between the pos +/- delta
        {
            //play Perfect animation
            CombatController.instance.hitDetectionText.text = "Perfect!";
            Debug.Log("Perfect!");
            amountHit += 1;
        }
        //the player is late
        else if (pos.x < hitList[index].pos - delta && pos.x >= hitList[index].pos - offset) //between the pos and -offset
        {
            //play Early animation
            CombatController.instance.hitDetectionText.text = "Late!";
            Debug.Log("Late!");
            amountHit += .5f;
        }

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
        else if (hasEffect && CanCureStatusEffect())
        {
            RemoveEffect();
        }

        playerHealth += delta;


        Debug.Log("Damage Taken: " + delta);

        //If player health goes up or done
        var sfx = delta < 0 ? AudioManager.instance.UISFX[1] : AudioManager.instance.UISFX[0];
        AudioManager.instance.SFX.clip = sfx;
        AudioManager.instance.SFX.Play();

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

    void ApplyItemEffect(bool isSusan)
    {
        if (isSusan && !GameManager.instance.susan.hasEffect)
        {
            GameManager.instance.susan.SetStatusEffect(itemUsed);
        }
        else if (!isSusan && !e.hasEffect)
        {
            e.SetStatusEffect(itemUsed);
        }
    }

    bool AttackingSusan(int enemyAttacked)
    {
        if (GameManager.instance.isSusanBattle && enemyAttacked == 0)
        {
            return true;
        }

        return false;
    }

    public IEnumerator DealDamageToEnemy(int enemyAttacked = 0, bool isItem = false)
    {
        //check if it we're using "good" or "bad" splash screens
        var splashScreen = amountHit >= (totalHits / 2) ? CombatController.instance.splashScreensGood : CombatController.instance.splashScreensBad;
        bool attackSusan = AttackingSusan(enemyAttacked);
        float damage = 0;

        //if we're attactking susan, set enemy to null
        _ = attackSusan ? e = null : e = CombatController.instance._inBattle[enemyAttacked].GetComponent<Enemy>();
        
        //if using an item, otherwise calculate damage and show splash screens
        if (isItem)
        {
            damage = itemUsed.delta;
            ApplyItemEffect(attackSusan);
        }
        else
        {
            damage = PlayerDamageModifier(_attackDamage[(int)CombatController.instance.selectedAction]);
            splashScreen[action].gameObject.SetActive(true);

            string animation = "Base Layer." + splashScreen[action].gameObject.name;
            CombatController.instance.SplashAnim.Play(animation, 0, 0f);

            yield return new WaitForSecondsRealtime(2f);

            splashScreen[action].gameObject.SetActive(false);
        }

        Debug.Log("Damage: " + damage);

        if (CombatController.instance.selectedAction == ActionType.Heal)
        {
            UpdatePlayerHealth(damage);
            yield return new WaitForSecondsRealtime(0.75f);
        }
        else if (attackSusan)
        {
            //Show hit Animation
            GameManager.instance.susan.EnemyHit();
            yield return new WaitForSecondsRealtime(0.25f);

            GameManager.instance.susan.UpdateHealth(damage);
            enemyHealth[enemyAttacked] -= damage;

            SwitchTurn();
            yield break;
        }
        else
        {
            //Show hit Animation
            e.EnemyHit();
            yield return new WaitForSecondsRealtime(0.25f);

            enemyHealth[enemyAttacked] -= damage;
            e.UpdateHealth(damage);
        }

        if (enemyHealth[enemyAttacked] <= 0)
        {
            //play enemy death animiation

            StartCoroutine(EnemyDeath(enemyAttacked, e));

            //If there are no more enemies, return to overworld
            if (_enemiesLeft <= 0)
            {
                StartCoroutine(GameManager.instance.BattleWon());
                yield break;
            }
        }

        yield return new WaitForEndOfFrame();

        SwitchTurn();

        StartCoroutine(CombatController.instance.EnemyPhase());
    }

    void SwitchTurn()
    {
        //Reset the # of total hits and amount it
        totalHits = 0;
        amountHit = 0;

        //After the damage has been delt we want to switch to the enemies turn
        AudioManager.instance.SFX.clip = AudioManager.instance.UISFX[2];
        AudioManager.instance.SFX.Play();
    }

    //Handles what happens when an enemy dies
    public IEnumerator EnemyDeath(int enemyAttacked, Enemy enemy)
    {
        //Death Sound
        AudioManager.instance.SFX.clip = AudioManager.instance.enemySFX[AudioManager.instance.enemySFX.Count - 1];
        AudioManager.instance.SFX.Play();

        //turin off health bar
        CombatController.instance.enemyHealthBars[enemyAttacked].gameObject.SetActive(false);

        enemy.EnemyDeath();
        yield return new WaitForSecondsRealtime(2f);

        Debug.Log("Enemy Dead");

        //decrease the number o f enemies left
        _enemiesLeft -= 1;

        //Destroy Enemy
        Destroy(CombatController.instance._inBattle[enemyAttacked].gameObject);

        Debug.Log("Enemy Dead: " + CombatController.instance._inBattle[enemyAttacked].name);

        //remove enemy from list(s)
        CombatController.instance._inBattle[enemyAttacked] = null;
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

    public void SetStatusEffect(StatusEffect se)
    {
        effect = se;
        hasEffect = true;

        Color color = new Color();
        ColorUtility.TryParseHtmlString(CombatController.instance.GetColor(se), out color);

        player.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

        //For now, they will all last 3 turns
        turnsUntilEffectOver = 3;
    }

    public void UpdateEffect()
    {
        if (hasEffect)
        {
            turnsUntilEffectOver -= 1;
            UpdatePlayerHealth(-1 * (int)effect);

            if (turnsUntilEffectOver <= 0)
                RemoveEffect();
        }
    }

    bool CanCureStatusEffect()
    {
        Debug.Log(itemUsed.effect.ToString());
        if ((int)itemUsed.effect <= 2)
        {
            if (itemUsed.effect.ToString().Substring(6) == effect.ToString())
            {
                return true;
            }
        }


        return false;
    }

    public void RemoveEffect()
    {
        hasEffect = false;
        effect = StatusEffect.None;

        player.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }
}
