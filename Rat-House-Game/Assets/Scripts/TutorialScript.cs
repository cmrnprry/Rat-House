using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialScript : MonoBehaviour
{
    //Where in the dialogue to show
    private int _index = 0;
    private bool hasHit = false;

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

    [TextArea(3, 5)]
    public string[] afterBattleDialogue;

    [TextArea(3, 5)]
    public string[] duringBattleDialogue;

    [TextArea(3, 5)]
    public string[] beforeBattleDialogue;

    //Shows the Opening Dialogue for the tutorial
    public IEnumerator ShowOpeningDialogue()
    {
        Debug.Log("Show Openinng Dialogue");

        //Waits for the text to stop typing
        while (dialogue.isTyping)
        {
            yield return null;
        }

        //wait for the player to press enter/space
        while (!Input.GetButton("SelectAction"))
        {
            yield return null;
        }

        //when you press space...
        if (Input.GetButton("SelectAction"))
        {
            //When we're at the end of the intro dialogue
            if (_index == 0)//dialogue.sentences.Length)
            {
                //Lower the text box
                anim.SetBool("isOpen", false);
                _index = 0;

                yield return new WaitForSecondsRealtime(.2f);

                //play some sort of screen wipe

                GameManager.instance.topOverlay.SetActive(false);
                GameManager.instance.healthParent.SetActive(true);

                //load correct scene
                SceneManager.LoadScene("Tutorial_Battle-FINAL");

                yield return new WaitForFixedUpdate();

                //Set up the battle scene
                SetUpTutorialBattle();

                yield return new WaitForFixedUpdate();

                StartTutorialBattle();
                yield break;
            }

            Debug.Log("Next Line");

            _index++;

            //load next sentence
            dialogue.NextSentence();

            StartCoroutine(ShowOpeningDialogue());
            yield break;
        }
    }

    void StartTutorialBattle()
    {
        //Start Music
        AudioManager.instance.StartCombatMusic();

        Debug.Log("start Tutorial Battle");

        anim.SetBool("isOpen", true);

        //set the sentences in the dialogue manager
        dialogue.sentences = duringBattleDialogue;

        //Start dialogue
        dialogue.StartDialogue();
        StartCoroutine(ShowBattleDialogue());
    }

    IEnumerator ReturnControlToPlayer()
    {
        Debug.Log("Do action");

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
        while (!_isFinished)
        {
            Debug.Log("waiting for player action to finish");
            yield return null;
        }

        yield return new WaitForSecondsRealtime(.2f);

        //If this is the last bit of dialogue
        if (_index == duringBattleDialogue.Length)
        {
            //Stop the music
            AudioManager.instance.StopCombatMusic();

            //set thesentences in the dialogue manager
            dialogue.sentences = afterBattleDialogue;

            //Load the Scene
            SceneManager.LoadScene("Tutorial-FINAL");

            GameManager.instance.topOverlay.SetActive(true);
            GameManager.instance.healthParent.SetActive(false);

            yield return new WaitForSecondsRealtime(0.5f);

            //Reset index
            _index = 0;

            anim.SetBool("isOpen", true);

            //Start dialogue
            dialogue.StartDialogue();
            StartCoroutine(ShowAfterBattleDialogue());

            yield break;
        }

        //reset the bool to fase
        _isFinished = false;

        anim.SetBool("isOpen", true);
        dialogue.NextSentence();

        StartCoroutine(ShowBattleDialogue());
    }

    IEnumerator ShowAfterBattleDialogue()
    {
        Debug.Log("index: " + _index);

        //Waits for the text to stop typing
        while (dialogue.isTyping)
        {
            Debug.Log("Wait until done typing");
            yield return null;
        }

        //wait for the player to press enter/space
        while (!Input.GetButton("SelectAction"))
        {
            yield return null;
        }

        //when you press space...
        if (Input.GetButtonDown("SelectAction"))
        {
            //When we're at the end of the intro dialogue
            if (_index == dialogue.sentences.Length)
            {
                yield return new WaitForSecondsRealtime(.5f);
                GameManager.instance.SetGameState(GameState.Overworld);
            }

            Debug.Log("Next Line");

            _index++;

            //load next sentence
            dialogue.NextSentence();
            StartCoroutine(ShowAfterBattleDialogue());
            yield break;
        }

    }

    IEnumerator StartAttackMusic(int selected)
    {
        Debug.Log("Start Slider");
        StartCoroutine(AudioManager.instance.SetMap(selected));

        yield return new WaitForSecondsRealtime(1f);

        while (AudioManager.instance.attackMusic.isPlaying)
        {
            Debug.Log("waiting attack to be over");
            yield return null;
        }

        GameManager.instance.battleAnimator.SetBool("isOpen", false);

        yield return new WaitForSecondsRealtime(0.5f);

        CombatController.instance.attackMenuParent.SetActive(true);
        CombatController.instance.HighlightMenuItem();

        yield return new WaitForSecondsRealtime(.5f);

        _isFinished = true;
    }

    IEnumerator Punch()
    {
        Debug.Log("Punch");

        //wait for the player to press enter/space
        while (!Input.GetButtonDown("SelectAction"))
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        //when you hit space...
        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        while (!_enemySelected)
            yield return null;

        yield return new WaitForSecondsRealtime(0.5f);

        _enemySelected = false;
        _isFinished = true;
    }

    IEnumerator Kick()
    {
        Debug.Log("Kick");

        //wait for the player to press hit down
        while (!Input.GetButtonDown("Down"))
        {
            Debug.Log("wait for the player to press hit down");
            yield return null;
        }

        //Change selected action
        CombatController.instance.selectedAction = ActionType.Kick;

        //Change the selected action
        var x = CombatController.instance.attackMenu.transform.GetChild(1);
        CombatController.instance.menuSelect.transform.position = x.position;

        yield return new WaitForEndOfFrame();

        //wait for the player to press enter/space
        while (!Input.GetButtonDown("SelectAction"))
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        //when you hit space...
        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        while (!_enemySelected)
            yield return null;

        yield return new WaitForEndOfFrame();

        StartCoroutine(StartAttackMusic(0));

        _enemySelected = false;

        //Change selected action
        CombatController.instance.selectedAction = ActionType.Punch;
    }

    IEnumerator UseAttackItem()
    {
        Debug.Log("Use attack Item");

        //wait for the player to press hit right
        while (!Input.GetButtonDown("Right"))
        {
            Debug.Log("wait for the player to press hit down");
            yield return null;
        }

        //Change selected action to Items
        CombatController.instance.selectedAction = ActionType.Item;

        yield return new WaitForEndOfFrame();

        //change to item menu
        ShowItemMenu(2, 2);


        
        //wait for the player to press hit down
        while (!Input.GetButtonDown("Down"))
        {
            Debug.Log("wait for the player to press hit down");
            yield return null;
        }

        //Change the selected action
        var x = CombatController.instance.itemMenu.transform.GetChild(1);
        CombatController.instance.menuSelect.transform.position = x.position;

        yield return new WaitForEndOfFrame();



        //wait for the player to press enter/space
        while (!Input.GetButtonDown("SelectAction"))
        {

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        //when you hit space...
        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        while (!_enemySelected)
            yield return null;

        yield return new WaitForEndOfFrame();

        //Play some animation

        yield return new WaitForSecondsRealtime(0.5f);

        _isFinished = true;
        //Change selected action
        CombatController.instance.selectedAction = ActionType.Punch;
    }

    IEnumerator EnemyAttacks()
    {
        Debug.Log("Enemy Attacks!");

        //Turn off the battle menu
        CombatController.instance.TurnOffHighlight();

        //WHEN WE HAVE THE DODGING IN PLACE THAT HERE


        //For now this is the placeholder attacks
        var e = CombatController.instance._inBattle[0].GetComponent<Enemy>();

        e.AttackPlayer(EnemyType.Tutorial_Intern);

        while (!e.IsTurnOver())
        {
            Debug.Log("Turn is not yet over");
            yield return null;
        }

        GameManager.instance.battleAnimator.SetBool("isOpen", false);

        yield return new WaitForSecondsRealtime(0.5f);

        _isFinished = true;
        ShowBattleMenu();
        CombatController.instance.HighlightMenuItem();
    }

    IEnumerator UseHealthItem()
    {
        Debug.Log("Use health Item");

        //wait for the player to press hit down
        while (!Input.GetButtonDown("Right"))
        {
            Debug.Log("wait for the player to press hit down");
            yield return null;
        }

        //Change selected action
        CombatController.instance.selectedAction = ActionType.Item;

        //change to item menu
        ShowItemMenu(2, 1);


        yield return new WaitForEndOfFrame();

        //wait for the player to press enter/space
        while (!Input.GetButtonDown("SelectAction"))
        {

            yield return null;
        }

        yield return new WaitForEndOfFrame();

        GameManager.instance.battleAnimator.SetBool("isOpen", false);

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
        if (Input.GetButton("Up"))
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
        else if (Input.GetButton("Down"))
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
        else if (Input.GetButton("SelectAction") && _canSelect)
        {
            _canSelect = false;
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
        else
        {
            CombatController.instance.selectedAction = ActionType.Item;
        }

        _canSelect = true;
        yield return new WaitForSecondsRealtime(.15f);
        StartCoroutine(AnyAttack());
    }

    //Choose Attack to be final attack
    IEnumerator ChooseAttack()
    {
        StartCoroutine(SelectEnemy());

        //wait for the player to slesct enemy
        while (!_enemySelected)
            yield return null;

        yield return new WaitForEndOfFrame();

        StartCoroutine(StartAttackMusic(_selected));
    }

    IEnumerator ChooseItem()
    {
        Debug.Log("Choose Item");
        if (Input.GetButton("Up"))
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
        else if (Input.GetButton("Down"))
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
        else if (Input.GetButton("SelectAction") && _canSelect)
        {
            _canSelect = false;
            switch (_selected)
            {
                case 1:
                    Debug.Log("Action Item");
                    StartCoroutine(SelectEnemy());

                    //wait for the player to slesct enemy
                    while (!_enemySelected)
                        yield return null;

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
        else if (Input.GetButton("Back"))
        {
            ShowBattleMenu();
            CombatController.instance.HighlightMenuItem();
            CombatController.instance.selectedAction = ActionType.Punch;
            _selected = 0;

            StartCoroutine(AnyAttack());

            yield break;
        }

        //Fix Highlight
        var x = CombatController.instance.attackMenu.transform.GetChild(_selected);
        CombatController.instance.menuSelect.transform.position = x.position;

        _canSelect = true;
        yield return new WaitForSecondsRealtime(.15f);

        StartCoroutine(ChooseItem());
    }

    IEnumerator SelectEnemy()
    {
        Debug.Log("Select Enemy");

        //Make battle menu disappear and highlight the correct enemy
        GameManager.instance.battleAnimator.SetBool("isOpen", false);
        CombatController.instance.HighlightEnemy();

        _enemySelected = false;

        //wait for the player to press enter/space
        while (!Input.GetButtonDown("SelectAction"))
        {

            yield return null;
        }

        CombatController.instance.TurnOffHighlight();
      
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
        while (dialogue.isTyping)
        {
            Debug.Log("Wait until done typing");
            yield return null;
        }

        //wait for the player to press enter/space
        while (!Input.GetButton("SelectAction"))
        {

            yield return null;
        }

        //when you press space...
        if (Input.GetButton("SelectAction"))
        {
            //When we're at the end of the intro dialogue
            if (_index % 2 == 1 && _index != 0)
            {
                //Lower the text box
                anim.SetBool("isOpen", false);

                yield return new WaitForSecondsRealtime(.2f);

                _index++;

                GameManager.instance.battleAnimator.SetBool("isOpen", true);

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
}
