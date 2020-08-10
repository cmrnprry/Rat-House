using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeReference]
    private float _maxHealth;

    [SerializeReference]
    private float _baseAcuracy;

    [SerializeReference]
    private float _baseAttack;

    [Header("Attacks")]
    //Number of attacks and chances of said attacks hitting. They follow the order in the spread sheet
    public int numberOfAttacks;
    public int[] chancesOfHitting;

    private bool _turnOver = false;

    public string effectName;
    private ParticleSystem _attackAnim;
    private List<BeatMapStruct> coffeeBeats = new List<BeatMapStruct>();
    private List<BeatMapStruct> waterBeats = new List<BeatMapStruct>();
    private List<BeatMapStruct> internBeats = new List<BeatMapStruct>();
    private List<BeatMapStruct> computerBeats = new List<BeatMapStruct>();

    private void Start()
    {
        var beats = AudioManager.instance.enemyBeatMap;

        //Set the enemy types to have the correct beats
        waterBeats = beats.GetRange(0, 2);
        coffeeBeats = beats.GetRange(2, 2);
        internBeats = beats.GetRange(4, 1);
        computerBeats = beats.GetRange(5, 3);

        _attackAnim = GameObject.FindGameObjectWithTag(effectName).GetComponent<ParticleSystem>();
        _attackAnim.gameObject.transform.position = transform.position;
        //Instasiate the enmy of Type
        // GameObject enemy = Instantiate(Resources.Load("Enemies/" + e.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);
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

    int CalculateChance()
    {
        return Random.Range(0, 100);
    }

    //Handles attacks for the Coffee Man
    IEnumerator CoffeeAttack()
    {
        //pick which attack is made via the chance
        int chance = CalculateChance();
        int music = 0;

        if (chance >= 70)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = coffeeBeats[0].beatsToHit;
            CombatStats._totalHits = coffeeBeats[0].beatsToHit.Count;

            //Set the base attack
            _baseAttack = coffeeBeats[0].base_damage;
            
            //set the music clip number
            music = 2;
        }
        else
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = coffeeBeats[1].beatsToHit;
            CombatStats._totalHits = coffeeBeats[1].beatsToHit.Count;

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

        yield return new WaitUntil(() => AudioManager.instance.dodgeMusic.isPlaying);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.dodgeMusic.isPlaying);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    //Handles attacks for Intern
    IEnumerator InternAttack()
    {
        //pick which attack is made via the chance
        int chance = CalculateChance();
        int music = 0;

        if (chance >= 0)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = internBeats[0].beatsToHit;
            CombatStats._totalHits = internBeats[0].beatsToHit.Count;

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

        yield return new WaitUntil(() => AudioManager.instance.dodgeMusic.isPlaying);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.dodgeMusic.isPlaying);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    IEnumerator WaterAttack()
    {
        //pick which attack is made via the chance
        int chance = CalculateChance();
        int music = 0;

        if (chance >= 60)
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = waterBeats[0].beatsToHit;
            CombatStats._totalHits = waterBeats[0].beatsToHit.Count;

            //Set the base attack
            _baseAttack = waterBeats[0].base_damage;

            //set the music clip number
            music = 0;
        }
        else
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = waterBeats[1].beatsToHit;
            CombatStats._totalHits = waterBeats[1].beatsToHit.Count;

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

        yield return new WaitUntil(() => AudioManager.instance.dodgeMusic.isPlaying);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.dodgeMusic.isPlaying);

        _attackAnim.Stop();

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }


    //Handles attacks for Intern
    IEnumerator TutorialInternAttack()
    {
        //Play some animation
        _attackAnim.Play();
        Debug.Log("Play attack animation");

        //while the animation is playing wait

        while (!_attackAnim.isStopped)
        {
            Debug.Log("attack animation Over: " + _attackAnim.isStopped);
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        _turnOver = true;
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
