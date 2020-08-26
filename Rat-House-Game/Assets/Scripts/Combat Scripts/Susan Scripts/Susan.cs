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

    public Animator anim;
    public GameObject finalImage;

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
    public float _currentHealth;
    private int phase = 0;

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
    public bool nextPhase = false;

    public string effectName;
    private ParticleSystem _attackAnim;
    private List<BeatMapStruct> beats = new List<BeatMapStruct>();


    // Start is called before the first frame update
    void Start()
    {
        // beats = AudioManager.instance.enemyBeatMap.GetRange(6, 4);
        _currentHealth = _maxHealth;
    }

    public void UpdateHealth(float dmg, bool effectUpdate = false)
    {
        _currentHealth -= dmg;
        healthSlider.value = (_currentHealth / _maxHealth);


        if (_currentHealth <= 0)
        {
            StartCoroutine(SusanDeath());
        }
        else if (_currentHealth <= 50 && phase == 2)
        {
            //turn off all battle stuffs
            GameManager.instance.battleAnimator.SetBool("IsOpen", false);
            CombatController.instance.TurnOffHighlight();

            nextPhase = true;
            SetDialogue(phaseTwoDialogue);
        }
        else if (_currentHealth <= 100 && phase == 1)
        {
            Debug.Log("turn off and set");
            //turn off all battle stuffs
            GameManager.instance.battleAnimator.SetBool("IsOpen", false);
            CombatController.instance.TurnOffHighlight();

            nextPhase = true;
            SetDialogue(phaseOneDialogue);
        }
        else if (!effectUpdate)
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
        anim.SetTrigger("Attack");

        yield return new WaitUntil(() => AudioManager.instance.startDodge);

        //while the animation is playing wait
        yield return new WaitUntil(() => !AudioManager.instance.startDodge);

        yield return new WaitForSecondsRealtime(0.5f);

        _turnOver = true;
    }

    public void SetDialogue(string[] dia)
    {
        GameManager.instance.diaAnim.SetBool("isOpen", true);
        GameManager.instance.dialogue.sentences = dia;
        GameManager.instance.dialogue.StartDialogue();
        StartCoroutine(SusanDialogue());
    }

    public IEnumerator SusanDialogue()
    {
        //Waits for the text to stop typing
        yield return new WaitUntil(() => GameManager.instance.dialogue.isTyping == false);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == GameManager.instance.dialogue.sentences.Length - 1)
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
                yield return new WaitForSecondsRealtime(5f);

                GameManager.instance.anim.CrossFade("Fade_Out", 1);
                yield return new WaitForSecondsRealtime(1);

                finalImage.SetActive(false);

                yield return new WaitForFixedUpdate();

                SceneManager.LoadScene("Main Menu");
                yield return new WaitForFixedUpdate();
                GameManager.instance.anim.CrossFade("Fade_In", 1);

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
        yield return new WaitForSecondsRealtime(2);

        GameManager.instance.topOverlay.SetActive(false);
        SceneManager.LoadScene("Battle-FINAL", LoadSceneMode.Additive);
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
        nextPhase = false;
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
        nextPhase = false;
        yield return null;
    }

    public IEnumerator SusanDeath()
    {
        Debug.Log("Dead");

        anim.SetTrigger("Dead");
        yield return new WaitForSecondsRealtime(2f);

        healthSlider.gameObject.SetActive(false);
        anim = null;

        //turn off all battle stuffs
        GameManager.instance.anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(1);

        GameManager.instance.battleAnimator.SetBool("IsOpen", false);
        GameManager.instance.healthParent.SetActive(false);
        CombatController.instance.ClearBattle();
        CombatController.instance.TurnOffHighlight();
        phase = 3;

        finalImage.SetActive(true);

        yield return new WaitForFixedUpdate();

        SceneManager.LoadScene("LastScene");
        yield return new WaitForFixedUpdate();
        GameManager.instance.anim.CrossFade("Fade_In", 1);

        yield return new WaitForSecondsRealtime(1);


        SetDialogue(postBattleDialogue);
    }

    public void SetStatusEffect(Items item)
    {
        Debug.Log("Here");
        effect = item.effect;
        hasEffect = true;

        Color color = new Color();
        ColorUtility.TryParseHtmlString(CombatController.instance.GetColor(effect), out color);

        anim.gameObject.GetComponent<SpriteRenderer>().color = color;

        //For now, they will all last 3 turns
        turnsUntilEffectOver = 3;
    }

    public void UpdateEffect()
    {
        if (hasEffect)
        {
            turnsUntilEffectOver -= 1;
            UpdateHealth((int)effect, true);

            if (turnsUntilEffectOver <= 0)
                RemoveEffect();
        }
    }

    public void RemoveEffect()
    {
        hasEffect = false;
        effect = StatusEffect.None;
        anim.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void EnemyHit()
    {
        anim.SetTrigger("Hit");
    }

    public void Idle()
    {

        anim.SetTrigger("Idle");
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
        UpdateEffect();

        _turnOver = over;
    }

    public float GetBaseAttack()
    {
        return _baseAttack;
    }
}
