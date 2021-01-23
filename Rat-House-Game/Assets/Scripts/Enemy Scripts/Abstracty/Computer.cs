using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Computer : EnemyCombatBehaviour
{
    private List<BeatMapStruct> computerBeats = new List<BeatMapStruct>();

    private void Start()
    {
        anim = gameObject.transform.GetChild(0).GetComponent<Animator>();

        var beats = AudioManager.instance.enemyBeatMap;
        _currentHealth = _maxHealth;

        //Set the enemy types to have the correct beats
        computerBeats = beats.GetRange(5, 3);

        _attackAnim = GameObject.FindGameObjectWithTag(effectName).GetComponent<ParticleSystem>();
        _attackAnim.gameObject.transform.position = gameObject.transform.position;
    }

    public override IEnumerator AttackPlayer()
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
        _turnOver = true;
    }

}
