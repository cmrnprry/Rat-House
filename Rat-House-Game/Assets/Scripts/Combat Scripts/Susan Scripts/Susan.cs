using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Susan : MonoBehaviour
{
    //holds all the enemies that appear in addition to susan for each phase
    private Dictionary<int, List<EnemyType>> enemiesInBattle = new Dictionary<int, List<EnemyType>>();

    //List of the enemies that will appear
    public List<EnemyType> enemies;

    [Header("Dialogue")]
    [TextArea(3, 5)]
    public string[] preBattleDialogue;

    [TextArea(3, 5)]
    public string[] phaseOneDialogue;

    [TextArea(3, 5)]
    public string[] phaseTwoDialogue;

    [TextArea(3, 5)]
    public string[] postBattleDialogue;
    private int _index;

    [Header("Stats")]
    [SerializeReference]
    private float _maxHealth;
    private float _currentHealth;

    [SerializeReference]
    private float _baseAttack;

    [HideInInspector]
    public Slider healthSlider;

    [Header("Attacks")]
    //Number of attacks and chances of said attacks hitting. They follow the order in the spread sheet
    public int numberOfAttacks;
    public int[] chancesOfHitting;

    private bool _turnOver = false;

    public string effectName;
    private ParticleSystem _attackAnim;
    private List<BeatMapStruct> beats = new List<BeatMapStruct>();


    // Start is called before the first frame update
    void Start()
    {
        beats = AudioManager.instance.enemyBeatMap.GetRange(6, 4);

        _attackAnim = GameObject.FindGameObjectWithTag(effectName).GetComponent<ParticleSystem>();
        _attackAnim.gameObject.transform.position = transform.position;
    }

    int CalculateChance()
    {
        return Random.Range(0, 100);
    }

    IEnumerator SusanAttack()
    {
        //pick which attack is made via the chance
        int chance = CalculateChance();
        int music = 0;

        if (chance >= 40) // sneak attack
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = beats[3].beatsToHit;
            CombatStats.totalHits = beats[3].beatsToHit.Count;

            //Set the base attack
            _baseAttack = beats[3].base_damage;
        }
        else if (chance >= 25) //mug throw
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = beats[0].beatsToHit;
            CombatStats.totalHits = beats[0].beatsToHit.Count;

            //Set the base attack
            _baseAttack = beats[0].base_damage;
        }
        else if (chance >= 20) //baby pics
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = beats[1].beatsToHit;
            CombatStats.totalHits = beats[1].beatsToHit.Count;

            //Set the base attack
            _baseAttack = beats[1].base_damage;
        }
        else //Lecture
        {
            //Set the beats to hit and the total hits
            AudioManager.instance.chosenEnemyAttack = beats[2].beatsToHit;
            CombatStats.totalHits = beats[2].beatsToHit.Count;

            //Set the base attack
            _baseAttack = beats[2].base_damage;
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

    public void SetDialogue()
    {
        GameManager.instance.diaAnim.SetBool("isOpen", true);
        GameManager.instance.dialogue.sentences = preBattleDialogue;
        GameManager.instance.dialogue.StartDialogue();
        StartCoroutine(OpeningSusanDialogue());
    }

    public IEnumerator OpeningSusanDialogue()
    {
        Debug.Log("here");
        //Waits for the text to stop typing
        yield return new WaitUntil(() => GameManager.instance.dialogue.isTyping == false);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == GameManager.instance.dialogue.sentences.Length)
        {
            //Lower the text box
            GameManager.instance.diaAnim.SetBool("isOpen", false);

            //reset the index to 0
            _index = 0;

            StartCoroutine(GoToBattle());

            yield break;
        }

        //increase the index
        _index++;

        //load next sentence
        GameManager.instance.dialogue.NextSentence();

        //Restart the coroutine
        StartCoroutine(OpeningSusanDialogue());
        yield break;

    }

    IEnumerator GoToBattle()
    {
        //play some sort of screen wipe
        GameManager.instance.anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSeconds(2);


        GameManager.instance.topOverlay.SetActive(false);
        SceneManager.LoadScene("Susan_Battle-FINAL", LoadSceneMode.Additive);
        GameManager.instance.TurnOffScene();

        yield return new WaitForSeconds(2);

        GameManager.instance.topOverlay.SetActive(false);

        GameManager.instance.anim.CrossFade("Fade_In", 1);

        yield return new WaitForFixedUpdate();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
