using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Coffee : EnemyCombatBehaviour
{
    private List<BeatMapStruct> coffeeBeats = new List<BeatMapStruct>();

    private void Start()
    {
        anim = gameObject.transform.GetChild(0).GetComponent<Animator>();

        var beats = AudioManager.instance.enemyBeatMap;
        _currentHealth = _maxHealth;

        //Set the enemy types to have the correct beats
        coffeeBeats = beats.GetRange(2, 2);

        _attackAnim = GameObject.FindGameObjectWithTag(effectName).GetComponent<ParticleSystem>();
        _attackAnim.gameObject.transform.position = gameObject.transform.position;
    }

    public override IEnumerator AttackPlayer()
    {
        //pick which attack is made via the chance
        int attackChance = CalculateChance(100);
        int music = 0;

        if (attackChance <= 70)
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

}
