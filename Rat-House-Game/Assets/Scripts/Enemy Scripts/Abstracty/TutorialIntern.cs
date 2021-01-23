using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialIntern : EnemyCombatBehaviour
{
    private List<BeatMapStruct> internBeats = new List<BeatMapStruct>();

    private void Start()
    {
        anim = gameObject.transform.GetChild(0).GetComponent<Animator>();

        var beats = AudioManager.instance.enemyBeatMap;
        _currentHealth = _maxHealth;

        //Set the enemy types to have the correct beats
        internBeats = beats.GetRange(4, 1);

        _attackAnim = GameObject.FindGameObjectWithTag(effectName).GetComponent<ParticleSystem>();
        _attackAnim.gameObject.transform.position = gameObject.transform.position;
    }

    public override IEnumerator AttackPlayer()
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

}
