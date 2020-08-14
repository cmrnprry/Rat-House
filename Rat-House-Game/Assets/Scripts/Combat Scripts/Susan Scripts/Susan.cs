using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Susan : MonoBehaviour
{
    //holds all the enemies that appear in addition to susan for each phase
    //List of the enemies that will appear
    public List<EnemyType> startBattle;
    public List<EnemyType> phaseOneBattle;
    public List<EnemyType> phaseTwoBattle;

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
    [SerializeReference]
    private float _currentHealth;
    private int phase = 0;

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
        // beats = AudioManager.instance.enemyBeatMap.GetRange(6, 4);
        _currentHealth = _maxHealth;
    }

    public void UpdateHealth(float dmg)
    {
        _currentHealth -= dmg;
        healthSlider.value = (_currentHealth / _maxHealth);


        if(_currentHealth <= 0)
        {
            SusanDeath();
        }
        else if (_currentHealth <= 50 && phase == 2)
        {
            //turn off all battle stuffs
            GameManager.instance.battleAnimator.SetBool("IsOpen", false);
            CombatController.instance.TurnOffHighlight();

            SetDialogue(phaseTwoDialogue);
        }
        else if (_currentHealth <= 100 && phase == 1)
        {
            Debug.Log("turn off and set");
            //turn off all battle stuffs
            GameManager.instance.battleAnimator.SetBool("IsOpen", false);
            CombatController.instance.TurnOffHighlight();

            SetDialogue(phaseOneDialogue);
        }
        else
        {
            StartCoroutine(CombatController.instance.EnemyPhase());
        }
    }

    int CalculateChance()
    {
        return Random.Range(0, 100);
    }

    public IEnumerator SusanAttack()
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
        Debug.Log("Play attack animation");

        yield return new WaitUntil(() => AudioManager.instance.startDodge);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.startDodge);

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    public void SetDialogue(string[] dia)
    {
        Debug.Log("set dialogue");
        GameManager.instance.diaAnim.SetBool("isOpen", true);
        GameManager.instance.dialogue.sentences = dia;
        GameManager.instance.dialogue.StartDialogue();
        StartCoroutine(SusanDialogue());
    }

    public IEnumerator SusanDialogue()
    {
        Debug.Log("here");
        //Waits for the text to stop typing
        yield return new WaitUntil(() => GameManager.instance.dialogue.isTyping == false);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == 0)//GameManager.instance.dialogue.sentences.Length)
        {
            //Lower the text box
            GameManager.instance.diaAnim.SetBool("isOpen", false);

            //reset the index to 0
            _index = 0;

            if (phase == 0)
            {
                Debug.Log("Got to battle");
                StartCoroutine(GoToBattle());
            }
            else if (phase == 1)
            {
                Debug.Log("Phase one");
                StartCoroutine(PhaseOne());
            }
            else if (phase == 2)
            {
                Debug.Log("PhaseTwo");
                StartCoroutine(PhaseTwo());
            }
            else if (phase >= 3)
            {
                Debug.Log("end");
                SceneManager.LoadScene("Temp-LastScene");
            }

            phase += 1;
            yield break;
        }

        //increase the index
        _index++;

        //load next sentence
        GameManager.instance.dialogue.NextSentence();

        //Restart the coroutine
        StartCoroutine(SusanDialogue());
        yield break;

    }

    IEnumerator GoToBattle()
    {
        //play some sort of screen wipe
        GameManager.instance.anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSeconds(2);

        Debug.Log("load");

        GameManager.instance.topOverlay.SetActive(false);
        SceneManager.LoadScene("Susan_Battle-FINAL", LoadSceneMode.Additive);
        GameManager.instance.TurnOffScene();

        GameManager.instance.anim.CrossFade("Fade_In", 1);

        yield return new WaitForFixedUpdate();

        //Spawn the correct enemies 
        beats = AudioManager.instance.enemyBeatMap.GetRange(6, 4);
        CombatController.instance.SetEnemies(startBattle);
        CombatController.instance.SetUpSusanBattle();
        healthSlider.value = 1;

        AudioManager.instance.StartCombatMusic();
        StartCoroutine(CombatController.instance.ChooseAction());

        yield return new WaitForFixedUpdate();
    }

    IEnumerator PhaseOne()
    {
        Debug.Log("phase one start");
        foreach (var e in phaseOneBattle)
        {
            CombatController.instance.AddEnemy(e);
        }

        yield return new WaitForEndOfFrame();

        //turn on all battle stuffs
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);
        CombatController.instance.ResetSlider();
        CombatController.instance.ShowActionMenu();
        CombatController.instance.HighlightMenuItem();

        yield return null;
    }
    
    IEnumerator PhaseTwo()
    {
        foreach (var e in phaseTwoBattle)
        {
            CombatController.instance.AddEnemy(e);
        }

        //turn on all battle stuffs
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);
        CombatController.instance.ResetSlider();
        CombatController.instance.ShowActionMenu();
        CombatController.instance.HighlightMenuItem();

        yield return null;
    }

    public void SusanDeath()
    {
        //turn off all battle stuffs
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);
        CombatController.instance.TurnOffHighlight();
        SetDialogue(postBattleDialogue);
    }

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

    public float GetBaseAttack()
    {
        return _baseAttack;
    }
}
