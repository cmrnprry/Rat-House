using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeReference]
    private float _maxHealth;
    public float _currentHealth;

    //status effect tracking
    private StatusEffect effect;
    public bool hasEffect = false;
    private int turnsUntilEffectOver;

    [SerializeReference]
    private float _baseAttack;

    [HideInInspector]
    public Slider healthSlider;

    [Header("Attacks")]
    //Number of attacks and chances of said attacks hitting. They follow the order in the spread sheet
    public int numberOfAttacks;
    public int[] chancesOfHitting;

    private bool _turnOver = false;

    [Header("Animations")]
    private Animator anim;
    public bool animOver = true;

    public string effectName;
    private ParticleSystem _attackAnim;
    private List<BeatMapStruct> coffeeBeats = new List<BeatMapStruct>();
    private List<BeatMapStruct> waterBeats = new List<BeatMapStruct>();
    private List<BeatMapStruct> internBeats = new List<BeatMapStruct>();
    private List<BeatMapStruct> computerBeats = new List<BeatMapStruct>();

    private void Start()
    {
        anim = gameObject.transform.GetChild(0).GetComponent<Animator>();

        var beats = AudioManager.instance.enemyBeatMap;
        _currentHealth = _maxHealth;

        //Set the enemy types to have the correct beats
        waterBeats = beats.GetRange(0, 2);
        coffeeBeats = beats.GetRange(2, 2);
        internBeats = beats.GetRange(4, 1);
        computerBeats = beats.GetRange(5, 3);

        _attackAnim = GameObject.FindGameObjectWithTag(effectName).GetComponent<ParticleSystem>();
        _attackAnim.gameObject.transform.position = gameObject.transform.position;
    }

    //Handles a single enemy's turn
    public void AttackPlayer(EnemyType e)
    {
        switch (e)
        {
            case EnemyType.Coffee:
                Debug.Log("Coffe man attack");
                StartCoroutine(CoffeeAttack());
                break;
            case EnemyType.Computer_Man:
                Debug.Log("Computer Man attack");
                StartCoroutine(ComputerAttack());
                break;
            case EnemyType.Water_Cooler:
                Debug.Log("water man attack");
                StartCoroutine(WaterAttack());
                break;
            case EnemyType.Intern:
                Debug.Log("Intern attack");
                StartCoroutine(InternAttack());
                break;
            case EnemyType.Tutorial_Intern:
                Debug.Log("Tutorial Intern attack");
                StartCoroutine(TutorialInternAttack());
                break;
            default:
                Debug.LogError("Error in Enemy Attack");
                break;
        }
    }

    int CalculateChance(int upperbound)
    {
        return Random.Range(0, upperbound);
    }

    //Handles attacks for the Coffee Man
    IEnumerator CoffeeAttack()
    {
        //pick which attack is made via the chance
        int attackChance = CalculateChance(100);
        int music = 0;

        if (attackChance >= 70)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = coffeeBeats[0].beatsToHit;
            CombatStats.totalHits = coffeeBeats[0].beatsToHit.Count;

            //Set the base attack
            _baseAttack = coffeeBeats[0].base_damage;

            //set the music clip number
            music = 2;
        }
        else
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = coffeeBeats[1].beatsToHit;
            CombatStats.totalHits = coffeeBeats[1].beatsToHit.Count;

            //Set the base attack
            _baseAttack = coffeeBeats[1].base_damage;

            //set the music clip number
            music = 3;
        }

        Note.showDodge = true;

        yield return new WaitForEndOfFrame();

        StartCoroutine(AudioManager.instance.SetDodgeMap(music));

        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        yield return new WaitUntil(() => AudioManager.instance.startDodge);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.startDodge);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    //Handles attacks for Intern
    IEnumerator InternAttack()
    {
        //pick which attack is made via the chance
        int chance = CalculateChance(100);
        int music = 0;

        if (chance >= 0)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = internBeats[0].beatsToHit;
            CombatStats.totalHits = internBeats[0].beatsToHit.Count;

            //Set the base attack
            _baseAttack = internBeats[0].base_damage;

            //set the music clip number
            music = 4;
        }
        else
        {
            //TODO: Implement this
            //haha funny joke intern doesn't want to fight
        }

        Note.showDodge = true;

        yield return new WaitForEndOfFrame();

        StartCoroutine(AudioManager.instance.SetDodgeMap(music));

        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        yield return new WaitUntil(() => AudioManager.instance.startDodge);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.startDodge);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    IEnumerator WaterAttack()
    {
        //pick which attack is made via the chance
        int chance = CalculateChance(100);
        int music = 0;

        if (chance >= 60)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = waterBeats[0].beatsToHit;
            CombatStats.totalHits = waterBeats[0].beatsToHit.Count;

            //Set the base attack
            _baseAttack = waterBeats[0].base_damage;

            //set the music clip number
            music = 0;
        }
        else
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = waterBeats[1].beatsToHit;
            CombatStats.totalHits = waterBeats[1].beatsToHit.Count;

            //Set the base attack
            _baseAttack = waterBeats[1].base_damage;

            //set the music clip number
            music = 1;
        }

        Note.showDodge = true;

        yield return new WaitForEndOfFrame();

        StartCoroutine(AudioManager.instance.SetDodgeMap(music));

        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        yield return new WaitUntil(() => AudioManager.instance.startDodge);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.startDodge);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    IEnumerator ComputerAttack()
    {
        //pick which attack is made via the chance
        int chance = CalculateChance(100);
        int music = 0;

        if (chance >= 50)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = computerBeats[1].beatsToHit;
            CombatStats.totalHits = computerBeats[1].beatsToHit.Count;

            //Set the base attack
            _baseAttack = computerBeats[1].base_damage;

            //set the music clip number
            music = 1;
        }
        else if (chance >= 30)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = computerBeats[2].beatsToHit;
            CombatStats.totalHits = computerBeats[2].beatsToHit.Count;

            //Set the base attack
            _baseAttack = computerBeats[2].base_damage;

            //set the music clip number
            music = 2;
        }
        else
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = computerBeats[0].beatsToHit;
            CombatStats.totalHits = computerBeats[0].beatsToHit.Count;

            //Set the base attack
            _baseAttack = computerBeats[0].base_damage;

            //set the music clip number
            music = 0;
        }

        Note.showDodge = true;

        yield return new WaitForEndOfFrame();

        StartCoroutine(AudioManager.instance.SetDodgeMap(music));

        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        yield return new WaitUntil(() => AudioManager.instance.startDodge);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.startDodge);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    //Handles attacks for Intern
    IEnumerator TutorialInternAttack()
    {
        //pick which attack is made via the chance
        int music = 0;

        //Set the beats to hit and the total hits
        AudioManager.instance.chosenEnemyAttack = internBeats[0].beatsToHit;
        CombatStats.totalHits = internBeats[0].beatsToHit.Count;

        //Set the base attack
        _baseAttack = internBeats[0].base_damage;

        //set the music clip number
        music = 4;

        Note.showDodge = true;

        yield return new WaitForEndOfFrame();

        StartCoroutine(AudioManager.instance.SetDodgeMap(music));

        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        yield return new WaitUntil(() => AudioManager.instance.startDodge);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.startDodge);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    public void UpdateHealth(float dmg)
    {

        _currentHealth -= dmg;

        healthSlider.value = (_currentHealth / _maxHealth);
    }

    public void SetStatusEffect(Items item)
    {
        effect = item.effect;
        hasEffect = true;

        Color color = new Color();
        ColorUtility.TryParseHtmlString(CombatController.instance.GetColor(effect), out color);
        Debug.Log("Color: " + color.ToString());
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;

        //For now, they will all last 3 turns
        turnsUntilEffectOver = 3;
    }

    public void UpdateEffect()
    {
        if (hasEffect)
        {
            turnsUntilEffectOver -= 1;
            UpdateHealth((int) effect);

            if (turnsUntilEffectOver <= 0)
                RemoveEffect();
        }
    }

    public void RemoveEffect()
    {
        hasEffect = false;
        effect = StatusEffect.None;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void EnemyHit()
    {
        anim.SetTrigger("Hit");
    }

    public void EnemyDeath()
    {
        anim.SetTrigger("Dead");
    }

    public float GetBaseAttack()
    {
        return _baseAttack;
    }

    //Getter for the player starting heath
    public float GetStartingHealth()
    {
        return _maxHealth;
    }

    public bool IsTurnOver()
    {
        return _turnOver;
    }

    public void SetIsTurnOver(bool over)
    {
        _turnOver = over;
    }
}
