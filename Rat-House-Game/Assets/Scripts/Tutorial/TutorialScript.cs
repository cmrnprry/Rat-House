using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class TutorialScript : MonoBehaviour
{
    //Where in the dialogue to show
    private int _index = 0;

    //Checks if enemy has been selected
    private bool _enemySelected = false;

    //Checks if the battle action is done
    private bool _isFinished = false;
    private bool _canSelect = false;

    //Variables for any attack
    private int _selected = 0;
    private int _maxBattleOptions = 4;

    private Animator diaAnim;
    private Dialogue dialogue;

    [TextArea(3, 5)]
    public string[] afterBattleDialogue;

    [TextArea(3, 5)]
    public string[] duringBattleDialogue;

    [TextArea(3, 5)]
    public string[] beforeBattleDialogue;

    public GameObject[] overworldLevelOne;

    void Start()
    {
        diaAnim = GameManager.instance.diaAnim;
        dialogue = GameManager.instance.dialogue;
    }

    //Shows the Opening Dialogue for the tutorial
    public IEnumerator ShowOpeningDialogue()
    {
        Debug.Log("wait to stop typing");
        yield return new WaitUntil(() => dialogue.isTyping == false);
        dialogue.enterText.SetActive(true);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));
        dialogue.enterText.SetActive(false);
        yield return new WaitForEndOfFrame();

        Debug.Log("action hit");
        //when you press space...

        Debug.Log("next");
        Debug.Log("index: " + _index);
        //When we're at the end of the intro dialogue
        if (_index == dialogue.sentences.Length - 1)
        {
            overworldLevelOne = SceneManager.GetActiveScene().GetRootGameObjects();

            //Lower the text box
            //anim.SetBool("isOpen", false);

            //reset the index to 0
            _index = 0;

            yield return new WaitForSecondsRealtime(.2f);

            //play some sort of screen wipe
            GameManager.instance.anim.CrossFade("Fade_Out", 1);
            yield return new WaitForSecondsRealtime(2);

            //load correct scene
            TurnOffScene();
            SceneManager.LoadScene("Battle-FINAL", LoadSceneMode.Additive);
            GameManager.instance.anim.CrossFade("Fade_In", 1);

            //Turn off the top overlay and turn on the player health bar
            GameManager.instance.topOverlay.SetActive(false);
            GameManager.instance.healthParent.SetActive(true);

            yield return new WaitForFixedUpdate();

            //Set up the battle scene
            SetUpTutorialBattle();

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            StartTutorialBattle();
            yield break;
        }

        //increase the index
        _index++;

        //load next sentence
        dialogue.NextSentence();

        //Restart the coroutine
        StartCoroutine(ShowOpeningDialogue());
        yield break;


    }

    void StartTutorialBattle()
    {
        //Start Music
        AudioManager.instance.StartCombatMusic();

        Debug.Log("start Tutorial Battle");

        //Open the dialogue box
        diaAnim.SetBool("isOpen", true);

        //set the sentences in the dialogue manager
        dialogue.sentences = duringBattleDialogue;

        //Start dialogue
        dialogue.StartDialogue();
        StartCoroutine(ShowBattleDialogue());
    }

    IEnumerator ReturnControlToPlayer()
    {
        //do action depending on where we are in dialogue
        switch (_index)
        {
            case 2:
                StartCoroutine(Punch());
                break;
            case 4:
                StartCoroutine(StartAttackMusic(0));
                break;
            case 6:
                StartCoroutine(Kick());
                break;
            case 8:
                StartCoroutine(UseAttackItem());
                break;
            case 10:
                StartCoroutine(EnemyAttacks());
                break;
            case 12:
                StartCoroutine(UseHealthItem());
                break;
            case 14:
                StartCoroutine(AnyAttack());
                break;
            default:
                Debug.LogError("Something has gone wrong :(");
                break;
        }

        //Waiting for player to finish an action
        yield return new WaitUntil(() => _isFinished);

        yield return new WaitForSecondsRealtime(.2f);

        //If this is the last bit of dialogue
        if (_index >= duringBattleDialogue.Length)
        {
            Debug.Log("End tutorial battle");

            //Stop the music
            AudioManager.instance.StopCombatMusic();

            //set thes entences in the dialogue manager to be the afer battle dialogue
            dialogue.sentences = afterBattleDialogue;

            //Turn the top overlay back on and the player health off
            GameManager.instance.topOverlay.SetActive(true);
            GameManager.instance.healthParent.SetActive(false);
            CombatController.instance.enemyHealthBars[0].gameObject.SetActive(false);

            //play some sort of screen wipe
            GameManager.instance.anim.CrossFade("Fade_Out", 1);
            yield return new WaitForSecondsRealtime(2);

            //load correct scene
            TurnOnScene();
            SceneManager.UnloadSceneAsync("Battle-FINAL");
            GameManager.instance.anim.CrossFade("Fade_In", 1);

            yield return new WaitForSecondsRealtime(0.5f);

            //Reset index to 0
            _index = 0;

            //Open the dilogue box
            diaAnim.SetBool("isOpen", true);

            //Start dialogue
            dialogue.StartDialogue();
            StartCoroutine(ShowAfterBattleDialogue());

            yield break;
        }

        //reset the bool to fase
        _isFinished = false;

        //open the daiologue box
        diaAnim.SetBool("isOpen", true);

        //Go to the next sentence
        dialogue.NextSentence();
        StartCoroutine(ShowBattleDialogue());

    }

    IEnumerator ShowAfterBattleDialogue()
    {
        Debug.Log("show after dialogue");
        Debug.Log("index: " + _index);

        yield return new WaitUntil(() => dialogue.isTyping == false);
        dialogue.enterText.SetActive(true);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));
        dialogue.enterText.SetActive(false);

        //when you press space...
        Debug.Log("Next Line");

        //incease the index
        _index++;

        //load next sentence
        dialogue.NextSentence();

        //When we're at the end of the intro dialogue
        if (_index == afterBattleDialogue.Length)
        {
            CombatController.instance._inBattle = new List<GameObject>();

            yield return new WaitForEndOfFrame();
            Debug.Log("End tutorial");

            yield return new WaitForSecondsRealtime(.5f);

            //set the game state to be the overworld
            GameManager.instance.SetGameState(GameState.AfterTutorial);
            yield break;
        }

        StartCoroutine(ShowAfterBattleDialogue());
        yield break;


    }

    IEnumerator Punch()
    {
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        yield return new WaitForEndOfFrame();

        //when you hit space...
        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        yield return new WaitUntil(() => _enemySelected);

        yield return new WaitForSecondsRealtime(0.5f);

        _enemySelected = false;
        _isFinished = true;
    }

    IEnumerator StartAttackMusic(int selected)
    {
        //Turn off the action menu (should already be off but just to make sure)
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);

        //Set up the attack
        StartCoroutine(AudioManager.instance.SetAttackMap(selected));
        CombatController.instance._stats.actionSounds = AudioManager.instance.attackSFX.GetRange(0, 3).ToArray();

        yield return new WaitForSecondsRealtime(1f);

        //wait until the attack music has stopped playing
        yield return new WaitUntil(() => AudioManager.instance.startAction);
        yield return new WaitUntil(() => !AudioManager.instance.startAction);

        //Wait half a second
        yield return new WaitForSecondsRealtime(0.5f);

        if (selected == 0 || selected == 1 || selected == 2)
        {
            var e = CombatController.instance._inBattle[0].GetComponent<EnemyCombatBehaviour>();
            StartCoroutine(HitEnemy(e));
            e.healthSlider.value -= .25f;
        }

        if (CombatController.instance.selectedActionType == ActionType.Heal)
        {
            ShowBattleMenu();
            _selected = 0;

            CombatController.instance.playerHealthSlider.value = 1;
            CombatController.instance.playerHealthText.text = 100 + "%";

            CombatController.instance.HighlightMenuItem();
            CombatController.instance.selectedActionType = ActionType.Punch;

            CombatController.instance.ResetSlider();

            if (_index == 14)
                StartCoroutine(AnyAttack());
            yield break;
        }

        _isFinished = true;
        CombatController.instance.ResetSlider();
    }

    IEnumerator Kick()
    {
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);
        CombatController.instance.HighlightMenuItem();

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Down"));

        //Change selected action
        CombatController.instance.selectedActionType = ActionType.Kick;

        //Change the selected action
        var x = CombatController.instance.attackMenu.transform.GetChild(1).GetChild(0);
        CombatController.instance.menuSelect.transform.position = x.position;

        yield return new WaitForEndOfFrame();

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        yield return new WaitForEndOfFrame();

        //when you hit space...
        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        yield return new WaitUntil(() => _enemySelected);

        yield return new WaitForEndOfFrame();

        //Start the attack music
        StartCoroutine(StartAttackMusic(1));

        //set selected enemy to fase
        _enemySelected = false;

        //wait until the attack music has stopped playing
        yield return new WaitUntil(() => AudioManager.instance.startAction);
        yield return new WaitUntil(() => !AudioManager.instance.startAction);
    }

    IEnumerator UseAttackItem()
    {
        Debug.Log("Use attack Item");

        //wait for the player to press hit right
        yield return new WaitUntil(() => Input.GetButtonDown("Right"));

        //Change selected action to Items
        CombatController.instance.selectedActionType = ActionType.Item;

        yield return new WaitForEndOfFrame();

        //change to item menu
        StartCoroutine(ShowItemMenu(2, 2));

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Down"));

        //Change the selected action
        var x = CombatController.instance.itemMenu.transform.GetChild(1).GetChild(0);
        CombatController.instance.menuSelect.transform.position = x.position + new Vector3(7, 0, 0);

        yield return new WaitForEndOfFrame();

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        yield return new WaitForEndOfFrame();

        //when you hit space...
        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        yield return new WaitUntil(() => _enemySelected);

        yield return new WaitForEndOfFrame();


        var e = CombatController.instance._inBattle[0].GetComponent<EnemyCombatBehaviour>();
        StartCoroutine(HitEnemy(e));
        var item = new Items(ItemType.Hot_Coffee, 1, 10, StatusEffect.Burn);
        e.SetStatusEffect(item);
        e.healthSlider.value -= .25f;

        //wait for above to be done
        yield return new WaitForSecondsRealtime(0.5f);

        _isFinished = true;
    }

    IEnumerator EnemyAttacks()
    {
        CombatController.instance._stats.gameObject.transform.position = new Vector3(12.5f, 6.19f, 0f);
        CombatController.instance._stats.gameObject.GetComponent<Note>().Flip();
        //turn off the battle anim
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);

        Debug.Log("Enemy Attacks!");

        //Turn off the battle menu
        CombatController.instance.TurnOffHighlight();

        //For now this is the placeholder attacks
        var e = CombatController.instance._inBattle[0].GetComponent<EnemyCombatBehaviour>();

        StartCoroutine(e.AttackPlayer());

        yield return new WaitUntil(() => e.IsTurnOver());

        yield return new WaitForSecondsRealtime(0.5f);

        CombatController.instance.playerHealthSlider.value -= .25f;
        CombatController.instance.playerHealthText.text = 75 + "%";

        _isFinished = true;
        CombatController.instance.ResetSlider();
        ShowBattleMenu();
        CombatController.instance.HighlightMenuItem();
    }

    IEnumerator UseHealthItem()
    {
        Debug.Log("Use health Item");

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Down"));

        //Change selected action
        CombatController.instance.selectedActionType = ActionType.Kick;

        //Change the selected action
        var x = CombatController.instance.attackMenu.transform.GetChild(1).GetChild(0);
        CombatController.instance.menuSelect.transform.position = x.position;

        yield return new WaitForEndOfFrame();

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Down"));

        //Change selected action
        CombatController.instance.selectedActionType = ActionType.Throw;

        //Change the selected action
        var y = CombatController.instance.attackMenu.transform.GetChild(2).GetChild(0);
        CombatController.instance.menuSelect.transform.position = y.position;

        yield return new WaitForEndOfFrame();

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Down"));

        //Change selected action
        CombatController.instance.selectedActionType = ActionType.Heal;

        //Change the selected action
        var z = CombatController.instance.attackMenu.transform.GetChild(3).GetChild(0);
        CombatController.instance.menuSelect.transform.position = z.position;

        yield return new WaitForEndOfFrame();

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        yield return new WaitForEndOfFrame();

        //turn off battle anim
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);

        //when you hit space...
        StartCoroutine(SelectPlayer());

        yield return new WaitUntil(() => _enemySelected);
        yield return new WaitForEndOfFrame();

        //Start the attack music
        StartCoroutine(StartAttackMusic(3));

        //set selected enemy to fase
        _enemySelected = false;

        //wait until the attack music has stopped playing
        yield return new WaitUntil(() => AudioManager.instance.startAction);
        yield return new WaitUntil(() => !AudioManager.instance.startAction);

        yield return new WaitForSecondsRealtime(0.5f);

        CombatController.instance.playerHealthSlider.value += .1f;
        CombatController.instance.playerHealthText.text = 85 + "%";

        _isFinished = true;
        ShowBattleMenu();
        CombatController.instance.HighlightMenuItem();
        //Change selected action
        CombatController.instance.selectedActionType = ActionType.Punch;
    }

    IEnumerator AnyAttack()
    {
        Debug.Log("Choose Action");
        yield return new WaitUntil(() => Input.GetButtonDown("Up") || Input.GetButtonDown("Down") || Input.GetButtonDown("Right") || Input.GetButtonDown("SelectAction"));

        if (Input.GetButtonDown("Up"))
        {
            if (_selected == 0)
            {
                _selected = _maxBattleOptions - 1;
            }
            else
            {
                _selected--;
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            if (_selected == _maxBattleOptions - 1)
            {
                _selected = 0;
            }
            else
            {
                _selected++;
            }
        }
        else if (Input.GetButtonDown("SelectAction"))
        {
            switch (_selected)
            {
                case 0:
                    Debug.Log("Punch");
                    CombatController.instance.selectedActionType = ActionType.Punch;
                    StartCoroutine(ChooseAttack());
                    break;
                case 1:
                    Debug.Log("Kick");
                    CombatController.instance.selectedActionType = ActionType.Kick;
                    StartCoroutine(ChooseAttack());
                    break;
                case 2:
                    Debug.Log("Throw");
                    CombatController.instance.selectedActionType = ActionType.Throw;
                    StartCoroutine(ChooseAttack());
                    break;
                case 3:
                    Debug.Log("Suck Blood");
                    CombatController.instance.selectedActionType = ActionType.Heal;
                    StartCoroutine(ChooseAttack());
                    break;
                default:
                    Debug.LogError("Something has gone wrong in Combat Controller");
                    break;
            }

            _selected = 0;
            yield break;
        }
        else if (Input.GetButtonDown("Right"))
        {
            Debug.Log("Open Item Menu");

            //Wait a frame before showing anything
            yield return new WaitForEndOfFrame();

            //Switch to the menu selection
            StartCoroutine(ShowItemMenu(2, 1));

            //reset the selected action to 0
            _selected = 0;
            StartCoroutine(ChooseItem());
            yield break;
        }

        //Fix Highlight
        var x = CombatController.instance.attackMenu.transform.GetChild(_selected).GetChild(0);
        CombatController.instance.menuSelect.transform.position = x.position;

        //Change selected action
        if (_selected == 0)
        {
            CombatController.instance.selectedActionType = ActionType.Punch;
        }
        else if (_selected == 1)
        {
            CombatController.instance.selectedActionType = ActionType.Kick;
        }
        else if (_selected == 2)
        {
            CombatController.instance.selectedActionType = ActionType.Throw;
        }
        else if (_selected == 3)
        {
            CombatController.instance.selectedActionType = ActionType.Heal;
        }


        yield return new WaitForEndOfFrame();
        StartCoroutine(AnyAttack());


    }

    //Choose Attack to be final attack
    IEnumerator ChooseAttack()
    {
        yield return new WaitForEndOfFrame();

        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        yield return new WaitUntil(() => _enemySelected);

        yield return new WaitForEndOfFrame();

        StartCoroutine(StartAttackMusic(_selected));
    }

    IEnumerator ChooseItem()
    {
        Debug.Log("Choose Item");
        yield return new WaitUntil(() => Input.GetButtonDown("Up") || Input.GetButtonDown("Down") || Input.GetButtonDown("Left") || Input.GetButtonDown("SelectAction"));

        if (Input.GetButtonDown("Up"))
        {
            if (_selected == 0)
            {
                _selected = 1;
            }
            else
            {
                _selected--;
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            if (_selected == 2)
            {
                _selected = 0;
            }
            else
            {
                _selected++;
            }
        }
        else if (Input.GetButtonDown("SelectAction"))
        {
            var e = CombatController.instance._inBattle[0].GetComponent<EnemyCombatBehaviour>();
            switch (_selected)
            {
                case 1:
                    Debug.Log("Action Item");
                    yield return new WaitForEndOfFrame();
                    StartCoroutine(SelectEnemy());

                    //wait for the player to slesct enemy
                    yield return new WaitUntil(() => _enemySelected);

                    yield return new WaitForEndOfFrame();

                    //Play some animation

                    StartCoroutine(HitEnemy(e));

                    yield return new WaitForSecondsRealtime(0.5f);

                    _isFinished = true;
                    _enemySelected = false;
                    break;
                case 2:
                    Debug.Log("Action Item");
                    yield return new WaitForEndOfFrame();
                    StartCoroutine(SelectEnemy());

                    //wait for the player to slesct enemy
                    yield return new WaitUntil(() => _enemySelected);

                    yield return new WaitForEndOfFrame();

                    //Play some animation
                    StartCoroutine(HitEnemy(e));

                    yield return new WaitForSecondsRealtime(0.5f);

                    _isFinished = true;
                    _enemySelected = false;
                    break;
                case 0:
                    Debug.Log("Heal");
                    CombatController.instance.playerHealthSlider.value += .1f;
                    CombatController.instance.playerHealthText.text = 95 + "%";

                    ShowBattleMenu();
                    _selected = 0;
                    CombatController.instance.HighlightMenuItem();
                    CombatController.instance.selectedActionType = ActionType.Punch;

                    StartCoroutine(AnyAttack());
                    break;
                default:
                    Debug.LogError("Something has gone wrong in Combat Controller");
                    break;
            }

            _selected = 0;
            yield break;
        }
        else if (Input.GetButtonDown("Left"))
        {
            ShowBattleMenu();
            _selected = 0;
            CombatController.instance.HighlightMenuItem();
            CombatController.instance.selectedActionType = ActionType.Punch;

            StartCoroutine(AnyAttack());

            yield break;
        }

        //Fix Highlight
        var x = CombatController.instance.attackMenu.transform.GetChild(_selected).GetChild(0);
        CombatController.instance.menuSelect.transform.position = x.position;

        yield return new WaitForEndOfFrame();

        StartCoroutine(ChooseItem());

    }

    IEnumerator SelectEnemy()
    {
        Debug.Log("Select Enemy");

        //Make battle menu disappear and highlight the correct enemy
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);
        CombatController.instance.HighlightSingleEnemy();

        //Set enemy selected to be false
        _enemySelected = false;

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        yield return new WaitForEndOfFrame();

        //Turn off the enemy highlight
        CombatController.instance.TurnOffHighlight();

        //Set enemy selected to be true
        _enemySelected = true;
    }

    IEnumerator SelectPlayer()
    {
        Debug.Log("Select Player");

        //Make battle menu disappear and highlight the correct enemy
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);
        CombatController.instance.HighlightPlayer();

        //Set enemy selected to be false
        _enemySelected = false;

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        yield return new WaitForEndOfFrame();

        //Turn off the enemy highlight
        CombatController.instance.TurnOffHighlight();

        //Set enemy selected to be true
        _enemySelected = true;
    }

    IEnumerator ShowItemMenu(int health, int damage)
    {
        //sets the text pos
        var text = CombatController.instance.attackMenu.transform.GetChild(0).gameObject;

        //turns on the correct menus
        CombatController.instance.attackMenuParent.SetActive(false);
        CombatController.instance.itemMenuParent.SetActive(true);

        foreach (var i in CombatController.instance.itemList)
        {
            var item = i.item.ToString().Replace('_', ' ');
            var obj = Instantiate(text, CombatController.instance.itemMenu.transform);
            if (i.item == ItemType.Plastic_Utensils || i.item == ItemType.Hot_Coffee)
            {
                obj.GetComponent<TextMeshProUGUI>().text = item + " (" + damage + ")";
            }
            else if (i.item == ItemType.Jims_Lunch)
            {
                obj.GetComponent<TextMeshProUGUI>().text = item + " (" + health + ")";
            }

        }

        yield return new WaitForEndOfFrame();

        //resets highlight to 0
        var x = CombatController.instance.itemMenu.transform.GetChild(0).GetChild(0);
        CombatController.instance.menuSelect.transform.position = x.position + new Vector3(7, 0, 0);
    }

    void ShowBattleMenu()
    {
        CombatController.instance.attackMenuParent.SetActive(true);

        //Clear Item Menu
        foreach (Transform child in CombatController.instance.itemMenu.transform)
        {
            Destroy(child.gameObject);
        }

        CombatController.instance.itemMenuParent.SetActive(false);
    }

    public IEnumerator HitEnemy(EnemyCombatBehaviour e)
    {
        e.EnemyHit();
        StartCoroutine(AudioManager.instance.WaitUntilNextBeat(Math.Round(AudioManager.instance.songPositionInBeats, MidpointRounding.AwayFromZero)));

        yield return new WaitUntil(() => AudioManager.instance.nextBeat);
        AudioManager.instance.nextBeat = false;
        e.Idle();
    }

    //Shows the Battle Dialogue for the tutorial
    public IEnumerator ShowBattleDialogue()
    {
        Debug.Log("Show Battle Dialogue");
        Debug.Log("index: " + _index);

        //Waits for the text to stop typing
        yield return new WaitUntil(() => !dialogue.isTyping);
        dialogue.enterText.SetActive(true);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));
        dialogue.enterText.SetActive(false);

        //when you press space...
        if (Input.GetButton("SelectAction"))
        {
            //When we're at the end of the intro dialogue
            if (_index % 2 == 1 && _index != 0)
            {
                //Lower the text box
                diaAnim.SetBool("isOpen", false);

                yield return new WaitForSecondsRealtime(.2f);

                _index++;

                //if we're not on the second action
                if (!(_index >= 1 && _index <= 4) && !(_index >= 9 && _index <= 11))
                {
                    GameManager.instance.battleAnimator.SetBool("IsOpen", true);
                    CombatController.instance.HighlightMenuItem();
                }


                yield return new WaitForSecondsRealtime(.2f);

                StartCoroutine(ReturnControlToPlayer());
                yield break;
            }

            Debug.Log("Next Line");

            _index++;

            //load next sentence
            dialogue.NextSentence();
            StartCoroutine(ShowBattleDialogue());
            yield break;
        }
    }

    //Sets up the battle scene
    void SetUpTutorialBattle()
    {
        Debug.Log("Add Enemy");

        //Adds enemy and sets up placement
        CombatController.instance.enemyList.Add(EnemyType.Tutorial_Intern);
        CombatController.instance.TutorialSetUp();
    }

    public void TurnOffScene()
    {
        foreach (GameObject obj in overworldLevelOne)
        {
            obj.SetActive(false);
        }
    }

    public void TurnOnScene()
    {
        foreach (GameObject obj in overworldLevelOne)
        {
            obj.SetActive(true);
        }
    }
}
