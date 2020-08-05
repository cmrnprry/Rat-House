using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    private int _maxBattleOptions = 3;

    public Animator anim;

    public Dialogue dialogue;

    private bool _skip = false;

    [TextArea(3, 5)]
    public string[] afterBattleDialogue;

    [TextArea(3, 5)]
    public string[] duringBattleDialogue;

    [TextArea(3, 5)]
    public string[] beforeBattleDialogue;


    //Shows the Opening Dialogue for the tutorial
    public IEnumerator ShowOpeningDialogue()
    {
        //Waits for the text to stop typing
        yield return new WaitUntil(() => dialogue.isTyping == false);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        if (Input.GetButton("SelectAction") && !_skip)
        {
            //When we're at the end of the intro dialogue
            if (_index == dialogue.sentences.Length)
            {
                //Lower the text box
                anim.SetBool("isOpen", false);

                //reset the index to 0
                _index = 0;

                yield return new WaitForSecondsRealtime(.2f);

                //play some sort of screen wipe

                //load correct scene
                SceneManager.LoadScene("Tutorial_Battle-FINAL");

                //Turn off the top overlay and turn on the player health bar
                GameManager.instance.topOverlay.SetActive(false);
                GameManager.instance.healthParent.SetActive(true);

                yield return new WaitForFixedUpdate();

                //Set up the battle scene
                SetUpTutorialBattle();

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
    }

    void StartTutorialBattle()
    {
        //Start Music
        AudioManager.instance.StartCombatMusic();

        Debug.Log("start Tutorial Battle");

        //Open the dialogue box
        anim.SetBool("isOpen", true);

        //set the sentences in the dialogue manager
        dialogue.sentences = duringBattleDialogue;

        //Start dialogue
        dialogue.StartDialogue();
        StartCoroutine(ShowBattleDialogue());
    }

    IEnumerator ReturnControlToPlayer()
    {
        //If the player has not skipped the tutorial
        if (!_skip)
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
            if (_index == duringBattleDialogue.Length)
            {
                Debug.Log("End tutorial battle");

                //Stop the music
                AudioManager.instance.StopCombatMusic();

                //set thes entences in the dialogue manager to be the afer battle dialogue
                dialogue.sentences = afterBattleDialogue;

                //Turn the top overlay back on and the player health off
                GameManager.instance.topOverlay.SetActive(true);
                GameManager.instance.healthParent.SetActive(false);

                //Load the Scene
                SceneManager.LoadScene("Tutorial-FINAL");

                yield return new WaitForSecondsRealtime(0.5f);

                //Reset index to 0
                _index = 0;

                //Open the dilogue box
                anim.SetBool("isOpen", true);

                //Start dialogue
                dialogue.StartDialogue();
                StartCoroutine(ShowAfterBattleDialogue());

                yield break;
            }

            //reset the bool to fase
            _isFinished = false;

            //open the daiologue box
            anim.SetBool("isOpen", true);

            //Go to the next sentence
            dialogue.NextSentence();
            StartCoroutine(ShowBattleDialogue());
        }
    }

    IEnumerator ShowAfterBattleDialogue()
    {
        Debug.Log("show after dialogue");
        Debug.Log("index: " + _index);

        //Waits for the text to stop typing
        yield return new WaitUntil(() => !dialogue.isTyping);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        if (Input.GetButtonDown("SelectAction") && !_skip)
        {
            Debug.Log("Next Line");

            //incease the index
            _index++;

            //load next sentence
            dialogue.NextSentence();

            //When we're at the end of the intro dialogue
            if (_index == afterBattleDialogue.Length)
            {
                Debug.Log("End tutorial");

                yield return new WaitForSecondsRealtime(.5f);

                //set the game state to be the overworld
                GameManager.instance.SetGameState(GameState.Overworld);
                yield break;
            }

            StartCoroutine(ShowAfterBattleDialogue());
            yield break;
        }

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

        yield return new WaitForSecondsRealtime(1f);

        //wait until the attack music has stopped playing
        yield return new WaitUntil(() => !AudioManager.instance.attackMusic.isPlaying);

        //Wait half a second
        yield return new WaitForSecondsRealtime(0.5f);

        _isFinished = true;
    }

    IEnumerator Kick()
    {
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);
        CombatController.instance.HighlightMenuItem();

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Down"));

        //Change selected action
        CombatController.instance.selectedAction = ActionType.Kick;

        //Change the selected action
        var x = CombatController.instance.attackMenu.transform.GetChild(1);
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
        StartCoroutine(StartAttackMusic(0));

        //set selected enemy to fase
        _enemySelected = false;

        //Change selected action to default
        CombatController.instance.selectedAction = ActionType.Punch;
    }

    IEnumerator UseAttackItem()
    {
        Debug.Log("Use attack Item");

        //wait for the player to press hit right
        yield return new WaitUntil(() => Input.GetButtonDown("Right"));

        //Change selected action to Items
        CombatController.instance.selectedAction = ActionType.Item;

        yield return new WaitForEndOfFrame();

        //change to item menu
        ShowItemMenu(2, 2);

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Down"));

        //Change the selected action
        var x = CombatController.instance.itemMenu.transform.GetChild(1);
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

        //Play Splash screen

        //wait for above to be done
        yield return new WaitForSecondsRealtime(0.5f);

        _isFinished = true;

        //Change selected action
        CombatController.instance.selectedAction = ActionType.Punch;
    }

    IEnumerator EnemyAttacks()
    {

        //turn off the battle anim
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);

        Debug.Log("Enemy Attacks!");

        //Turn off the battle menu
        CombatController.instance.TurnOffHighlight();

        //WHEN WE HAVE THE DODGING IN PLACE THAT HERE

        //For now this is the placeholder attacks
        var e = CombatController.instance._inBattle[0].GetComponent<Enemy>();

        e.AttackPlayer(EnemyType.Tutorial_Intern);

        yield return new WaitUntil(() => e.IsTurnOver());

        yield return new WaitForSecondsRealtime(0.5f);

        _isFinished = true;
        ShowBattleMenu();
        CombatController.instance.HighlightMenuItem();
    }

    IEnumerator UseHealthItem()
    {
        Debug.Log("Use health Item");

        //wait for the player to press hit down
        yield return new WaitUntil(() => Input.GetButtonDown("Right"));

        //Change selected action
        CombatController.instance.selectedAction = ActionType.Item;

        //change to item menu
        ShowItemMenu(2, 1);

        yield return new WaitForEndOfFrame();

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        yield return new WaitForEndOfFrame();

        //turn off battle anim
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);

        //Play some animation

        yield return new WaitForSecondsRealtime(0.5f);

        _isFinished = true;
        ShowBattleMenu();
        CombatController.instance.HighlightMenuItem();
        //Change selected action
        CombatController.instance.selectedAction = ActionType.Punch;
    }

    IEnumerator AnyAttack()
    {
        Debug.Log("Choose Action");
        if (!_skip)
        {
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
                        StartCoroutine(ChooseAttack());
                        break;
                    case 1:
                        Debug.Log("Kick");
                        StartCoroutine(ChooseAttack());
                        break;
                    case 2:
                        Debug.Log("Items");
                        CombatController.instance.selectedAction = ActionType.Item;
                        _selected = 0;
                        ShowItemMenu(2, 1);
                        StartCoroutine(ChooseItem());
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
                ShowItemMenu(1, 1);

                //reset the selected action to 0
                _selected = 0;
                yield break;
            }

            //Fix Highlight
            var x = CombatController.instance.attackMenu.transform.GetChild(_selected);
            CombatController.instance.menuSelect.transform.position = x.position;

            //Change selected action
            if (_selected == 0)
            {
                CombatController.instance.selectedAction = ActionType.Punch;
            }
            else if (_selected == 1)
            {
                CombatController.instance.selectedAction = ActionType.Kick;
            }

            yield return new WaitForEndOfFrame();
            StartCoroutine(AnyAttack());
        }

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
        if (!_skip)
        {
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
                if (_selected == 1)
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
                    case 1:
                        Debug.Log("Action Item");
                        yield return new WaitForEndOfFrame();
                        StartCoroutine(SelectEnemy());

                        //wait for the player to slesct enemy
                        yield return new WaitUntil(() => _enemySelected);

                        yield return new WaitForEndOfFrame();

                        //Play some animation

                        yield return new WaitForSecondsRealtime(0.5f);

                        _isFinished = true;
                        _enemySelected = false;
                        break;
                    case 0:
                        Debug.Log("Heal");
                        StartCoroutine(ChooseItem());
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
                CombatController.instance.selectedAction = ActionType.Punch;

                StartCoroutine(AnyAttack());

                yield break;
            }

            //Fix Highlight
            var x = CombatController.instance.attackMenu.transform.GetChild(_selected);
            CombatController.instance.menuSelect.transform.position = x.position;

            yield return new WaitForEndOfFrame();

            StartCoroutine(ChooseItem());
        }
    }

    IEnumerator SelectEnemy()
    {
        Debug.Log("Select Enemy");

        //Make battle menu disappear and highlight the correct enemy
        GameManager.instance.battleAnimator.SetBool("IsOpen", false);
        CombatController.instance.HighlightEnemy();

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

    void ShowItemMenu(int health, int damage)
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
            if (i.item == ItemType.Basic_Damage)
            {
                obj.GetComponent<TextMeshProUGUI>().text = item + " (" + damage + ")";
            }
            else if (i.item == ItemType.Basic_Heath)
            {
                obj.GetComponent<TextMeshProUGUI>().text = item + " (" + health + ")";
            }

        }

        //resets highlight to 0
        var x = CombatController.instance.itemMenu.transform.GetChild(0);
        CombatController.instance.menuSelect.transform.position = x.position;
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

    //Shows the Battle Dialogue for the tutorial
    public IEnumerator ShowBattleDialogue()
    {
        Debug.Log("Show Battle Dialogue");
        Debug.Log("index: " + _index);

        //Waits for the text to stop typing
        yield return new WaitUntil(() => !dialogue.isTyping);

        //wait for the player to press enter/space
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction"));

        //when you press space...
        if (Input.GetButton("SelectAction") && !_skip)
        {
            //When we're at the end of the intro dialogue
            if (_index % 2 == 1 && _index != 0)
            {
                //Lower the text box
                anim.SetBool("isOpen", false);

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

    public void SkipTutorial()
    {
        StopAllCoroutines();
        _skip = true;
    }
}
