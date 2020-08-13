﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static EnemyController;

public enum ActionType
{
    Item = -1,
    Punch = 0,
    Kick = 1,
    Throw = 2,
    Heal = 3,
}

public enum ItemType
{
    Basic_Heath = 0,
    Basic_Damage = 1,
}

public struct Items
{
    //i = item type
    //c = number of item in inventory
    //d = amount of health/damage item does, if any
    //e = status effect inflicted, if any
    public Items(ItemType i, int c, int d = 0)
    {
        item = i;
        count = c;
        delta = d;
    }

    public ItemType item { get; set; }
    public int count { get; set; }
    public int delta { get; set; }
}

public class CombatController : MonoBehaviour
{
    public static CombatController instance;

    [Header("Lists")]
    //List of enemy Types currently on the board
    public List<EnemyType> enemyList = new List<EnemyType>(); //MAX OF 5

    //List of all potential player actions
    [SerializeField]
    private List<ActionType> _actionList;

    //List of all potential player item
    public List<Items> itemList = new List<Items>();

    //base damage that attacks can do
    public List<float> attackDamage = new List<float>();

    //List of enemy placements
    public List<Vector3> enemyPlacement;
    public List<Slider> enemyHealthBars;

    //list of enemies in battle
    public List<GameObject> _inBattle = new List<GameObject>();


    [Header("Currently Selected")]
    //Current selected player action
    public ActionType selectedAction;

    //Current indext of _actionList
    private int _selectedAction = 0;

    //Current enemy selected
    private int _selectedEnemy = 0;

    //Current enemy selected
    private int _selectedItem = 0;

    //Checks to see if the player can select from the action list
    public bool enemyTurnOver = false;

    //Keeps trackof player/enemy stats in battle
    private CombatStats _stats;

    [Header("Menus")]
    public GameObject attackMenuParent;
    public GameObject attackMenu;
    public GameObject itemMenuParent;
    public GameObject itemMenu;
    public GameObject menuSelect;
    private GameObject _enemyParent;
    private GameObject _enemyHealthParent;

    [Header("UI")]
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI hitDetectionText;
    //public Animator SplashAnim;
    public Image[] splashScreensGood;
    public Image[] splashScreensBad;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(this.gameObject);
    }

    //Holds all the values to reset the battle
    public void ResetBattle()
    {
        //Clear old enemies obj and reset sleected
        ClearBattle();

        PlaceEnemies();

        //Reset Stats
        _stats.SetStats();

        GameManager.instance.deathScreenParent.SetActive(false);
        attackMenuParent.SetActive(true);
        StartCoroutine(ChooseAction());
    }

    public void ClearBattle()
    {
        //turn on/off corect panels
        itemMenuParent.SetActive(false);
        attackMenuParent.SetActive(true);

        //Reset selected
        _selectedAction = 0;
        _selectedItem = 0;
        _selectedEnemy = 0;
        selectedAction = ActionType.Punch;

        foreach (Transform child in _enemyParent.transform)
        {
            Destroy(child.gameObject);
        }

        ClearItemMenu();

        //Reset Enemy lists
        _inBattle = new List<GameObject>();
    }

    //Method to set up battles
    //Will basically place the enemies in the correct spots
    public void SetUpBattleScene()
    {
        //Find the stats 
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();

        //find the enemy parent
        _enemyParent = GameObject.FindGameObjectWithTag("Enemy Parent");

        //Place the enemies
        PlaceEnemies();

        //Display player health
        GameManager.instance.healthParent.SetActive(true);
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);

        //Set the Stats
        _stats.SetStats();
    }

    public void PlaceEnemies()
    {
        var index = 0;
        Debug.Log("place enemies");

        foreach (var e in enemyList)
        {
            //Instasiate the enmy of Type
            GameObject enemy = Instantiate(Resources.Load("Enemies/" + e.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);

            //Add it to the list of enemy game objects
            _inBattle.Add(enemy);

            enemyHealthBars[index].gameObject.SetActive(true);

            //Set enemy health

            _ = (e == EnemyType.Susan) ? GameManager.instance.susan.healthSlider = enemyHealthBars[index] : enemy.GetComponent<Enemy>().healthSlider = enemyHealthBars[index];

            //Parent enemy
            enemy.transform.parent = _enemyParent.transform;

            //increase the index
            index++;
        }
    }

    public void AddEnemy(EnemyType enemy)
    {
        var index = _inBattle.Count;
        Debug.Log("add enemy of type: " + enemy.ToString());

        //if the board is full
        if (index >= 5)
        {
            //Would want to find the weakest enemy
            //delete him
            //set index to be his pos

            //for now do nothing
            return;
        }


        //Instasiate the enmy of Type
        GameObject newE = Instantiate(Resources.Load("Enemies/" + enemy.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);

        //Add it to the list of enemy game objects
        _inBattle.Add(newE);

        //Set enemy health
        enemyHealthBars[index].gameObject.SetActive(true);
        newE.GetComponent<Enemy>().healthSlider = enemyHealthBars[index];

        //Parent enemy
        newE.transform.parent = _enemyParent.transform;
    }

    //Handles the player choosing which action to take
    public IEnumerator ChooseAction()
    {
        //Wait until a correct key is pressed
        yield return new WaitUntil(() => Input.GetButtonDown("Up") || Input.GetButtonDown("Down") || Input.GetButtonDown("SelectAction") || Input.GetButtonDown("Right"));

        if (Input.GetButtonDown("Up"))
        {
            if (_selectedAction == 0)
            {
                _selectedAction = _actionList.Count - 1;
            }
            else
            {
                _selectedAction--;
            }
        }
        else if (Input.GetButtonDown("Down"))
        {
            if (_selectedAction == _actionList.Count - 1)
            {
                _selectedAction = 0;
            }
            else
            {
                _selectedAction++;
            }
        }
        else if (Input.GetButtonDown("SelectAction"))
        {
            //Turn off the highlight
            TurnOffHighlight();

            //Turn off the attack 
            GameManager.instance.battleAnimator.SetBool("IsOpen", false);

            //Select the correct action
            switch (_actionList[_selectedAction])
            {
                case ActionType.Punch:
                    _stats.actionSounds = AudioManager.instance.attackSFX.GetRange(0, 3).ToArray();
                    StartCoroutine(ChooseEnemy());
                    break;
                case ActionType.Kick:
                    _stats.actionSounds = AudioManager.instance.attackSFX.GetRange(0, 3).ToArray(); //TODO: put in kick attacks
                    StartCoroutine(ChooseEnemy());
                    break;
                case ActionType.Throw:
                    _stats.actionSounds = AudioManager.instance.attackSFX.GetRange(0, 3).ToArray(); //TODO: put in throw attacks
                    StartCoroutine(ChooseEnemy());
                    break;
                case ActionType.Heal:
                    _stats.actionSounds = AudioManager.instance.attackSFX.GetRange(0, 3).ToArray(); //TODO: put in heal attacks
                    StartCoroutine(ChoosePlayer());
                    break;
                default:
                    Debug.LogError("Something has gone wrong in Combat Controller");
                    break;
            }


            yield return new WaitForEndOfFrame();

            _stats.action = _selectedAction;

            //break out of the coroutine
            yield break;
        }
        else if (Input.GetButtonDown("Right"))
        {
            //Folder flip
            AudioManager.instance.SFX.clip = AudioManager.instance.UISFX[4];
            AudioManager.instance.SFX.Play();

            selectedAction = ActionType.Item;
            Debug.Log("Open Item Menu");

            //Wait a frame before showing anything
            yield return new WaitForEndOfFrame();

            //Switch to the menu selection
            ShowItemsMenu();
            HighlightMenuItem();

            //reset the selected action to 0
            _selectedAction = 0;
            yield break;
        }

        //Highlight the correct item
        HighlightMenuItem();

        //Set the correct selected action
        selectedAction = _actionList[_selectedAction];

        //Wait a frame to  rerun the coroutine
        yield return new WaitForEndOfFrame();
        StartCoroutine(ChooseAction());
    }
    public IEnumerator ChooseItem()
    {
        //Wait until a correct key is pressed
        yield return new WaitUntil(() => Input.GetButtonDown("Up") || Input.GetButtonDown("Down") || Input.GetButtonDown("SelectAction") || Input.GetButtonDown("Left"));

        //If you hvae items other wise turn  off the highlight
        if (itemList.Count > 0)
        {
            if (Input.GetButtonDown("Up"))
            {
                if (_selectedItem == 0)
                {
                    _selectedItem = itemList.Count - 1;
                }
                else
                {
                    _selectedItem--;
                }
            }
            else if (Input.GetButtonDown("Down"))
            {
                if (_selectedItem == itemList.Count - 1)
                {
                    _selectedItem = 0;
                }
                else
                {
                    _selectedItem++;
                }
            }
            else if (Input.GetButtonDown("SelectAction"))
            {
                TurnOffHighlight();
                GameManager.instance.battleAnimator.SetBool("IsOpen", false);

                switch (itemList[_selectedItem].item)
                {
                    case ItemType.Basic_Heath:
                        Debug.Log("Basic Heath Item");
                        StartCoroutine(ChoosePlayer(true));
                        break;
                    case ItemType.Basic_Damage:
                        Debug.Log("Basic Damage Item");
                        UseDamageItem(itemList[_selectedItem]);
                        break;
                    default:
                        Debug.LogError("Something has gone wrong in Combat Controller");
                        break;
                }

                yield break;
            }

            HighlightMenuItem();
        }
        else
        {
            menuSelect.SetActive(false);
        }

        if (Input.GetButton("Left"))
        {
            //Folder flip
            AudioManager.instance.SFX.clip = AudioManager.instance.UISFX[4];
            AudioManager.instance.SFX.Play();

            Debug.Log("Open Action Menu");

            //Wait a frame before showing anything
            yield return new WaitForEndOfFrame();

            //Switch to the action selection
            ShowActionMenu();
            HighlightMenuItem();
            yield break;
        }

        yield return new WaitForSecondsRealtime(0.15f);
        StartCoroutine(ChooseItem());
    }

    //Allows the player to choose which enemy they will attack
    IEnumerator ChooseEnemy(bool isItem = false, float itemDmg = 0)
    {
        //Wait until a correct key is pressed
        yield return new WaitUntil(() => Input.GetButtonDown("Up") || Input.GetButtonDown("Down") || Input.GetButtonDown("Left") || Input.GetButtonDown("Right") ||
        Input.GetButtonDown("SelectAction") || Input.GetButtonDown("Back"));

        if (Input.GetButtonDown("Up") || Input.GetButtonDown("Left"))
        {
            if (_selectedEnemy == 0)
            {
                _selectedEnemy = _inBattle.Count - 1;
            }
            else
            {
                _selectedEnemy--;
            }

            if (_inBattle[_selectedEnemy] == null)
            {
                _selectedEnemy--;
            }
        }
        else if (Input.GetButtonDown("Down") || Input.GetButtonDown("Right"))
        {
            if (_selectedEnemy == _inBattle.Count - 1)
            {
                _selectedEnemy = 0;
            }
            else
            {
                _selectedEnemy++;
            }

            if (_inBattle[_selectedEnemy] == null)
            {
                _selectedEnemy++;
            }
        }
        else if (Input.GetButtonDown("SelectAction"))
        {
            TurnOffHighlight();

            if (isItem)
            {
                Debug.Log("deal item damage");

                //TODO: Display animation of item usage
                Debug.Log("item: " + isItem + " " + itemDmg);
                DealDamage(true, itemDmg);
            }
            else
            {
                Debug.Log("Set Map");
                StartCoroutine(AudioManager.instance.SetAttackMap(_selectedAction));
            }

            yield break;
        }
        else if (Input.GetButtonDown("Back"))
        {
            TurnOffHighlight();

            GameManager.instance.battleAnimator.SetBool("IsOpen", true);

            ShowActionMenu();
            HighlightMenuItem();
            yield break;
        }
        //TODO:Add some sort of visual display to show the selected enemy
        HighlightEnemy();

        yield return new WaitForEndOfFrame();
        StartCoroutine(ChooseEnemy(isItem, itemDmg));
    }

    IEnumerator ChoosePlayer(bool isItem = false)
    {

        //Wait until a correct key is pressed
        yield return new WaitUntil(() => Input.GetButtonDown("SelectAction") || Input.GetButtonDown("Back"));


        if (Input.GetButtonDown("SelectAction"))
        {
            TurnOffHighlight();

            if (isItem)
            {
                StartCoroutine(UseHealthItem(itemList[_selectedItem]));
            }
            else
            {
                Debug.Log("Set Map");
                StartCoroutine(AudioManager.instance.SetAttackMap(_selectedAction));
            }

            yield break;
        }
        else if (Input.GetButtonDown("Back"))
        {
            TurnOffHighlight();

            GameManager.instance.battleAnimator.SetBool("IsOpen", true);

            ShowActionMenu();
            HighlightMenuItem();
            yield break;
        }

        HighlightPlayer();

        yield return new WaitForEndOfFrame();
        StartCoroutine(ChoosePlayer(isItem));
    }

    void UseDamageItem(Items item)
    {
        Debug.Log("item: " + item.item.ToString() + " " + item.delta);
        //Choose the item to use
        StartCoroutine(ChooseEnemy(true, item.delta));

        //decrease the amount of the used item
        itemList.Add(new Items(item.item, -1, item.delta));
        GameManager.instance.CollapseItemList(itemList);
    }

    //Will be called every time the player uses a health item
    IEnumerator UseHealthItem(Items item)
    {
        //decrease the amount of the used item
        itemList.Add(new Items(item.item, -1, item.delta));
        GameManager.instance.CollapseItemList(itemList);

        //update the player's health
        _stats.UpdatePlayerHealth(item.delta);

        yield return new WaitForSecondsRealtime(0.75f);

        //TODO: MAKE THIS NOT HARD CODED IN I DONT FORSEE THE NUMBERS CHSNGING BUT ITS BAD FIX IT
        _stats.gameObject.transform.position = new Vector3(12.5f, 6.19f, 0f);

        //Start Enemy Phase & play sfx
        AudioManager.instance.SFX.clip = AudioManager.instance.UISFX[2];
        AudioManager.instance.SFX.Play();
        StartCoroutine(EnemyPhase());
    }

    public void HighlightPlayer()
    {
        var spotlight = _enemyParent.transform.GetChild(0);

        spotlight.gameObject.SetActive(true);
        spotlight.transform.localPosition = new Vector3(-4.14f, 0f, -0.14f);
    }

    public void HighlightEnemy()
    {
        var spotlight = _enemyParent.transform.GetChild(0);

        spotlight.gameObject.SetActive(true);
        spotlight.transform.position = enemyPlacement[_selectedEnemy];
    }

    //Tells the Combat Stats to deal with damage
    public void DealDamage(bool isItem = false, float itemDmg = 0)
    {
        Debug.Log("item dmg: " + itemDmg);
        //Want to send over what enemy was targeted
        //what attack was done
        //is defaulted to 0 untile multiple enemes are implemented
        Debug.Log("Selected Enemy: " + _selectedEnemy);

        //TODO: MAKE THIS NOT HARD CODED IN I DONT FORSEE THE NUMBERS CHSNGING BUT ITS BAD FIX IT
        _stats.gameObject.transform.position = new Vector3(12.5f, 6.19f, 0f);

        StartCoroutine(_stats.DealDamageToEnemy(_selectedEnemy, isItem, itemDmg));
    }

    //Will play through the enemy turn
    // paramater here is used to track which enemy will be taking their turn
    // Default is 0
    public IEnumerator EnemyPhase(int enemy = 0)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log("Enemy Phase Start");

        //for each non-defeated enemy, do their attacks
        if (enemy < _inBattle.Count)
        {
            if (_inBattle[enemy] != null)
            {
                var e = _inBattle[enemy];

                if (e.name == "Susan(Clone)")
                {
                    StartCoroutine(GameManager.instance.susan.SusanAttack());

                    //Waits untik this returns true
                    yield return new WaitUntil(() => GameManager.instance.susan.IsTurnOver());


                    //Reset the IsTurnOver to be false
                    GameManager.instance.susan.SetIsTurnOver(false);

                    //Deal Damage to Player
                    _stats.UpdatePlayerHealth(-1 * GameManager.instance.susan.GetBaseAttack());
                }
                else
                {
                    var en = e.GetComponent<Enemy>();
                    en.AttackPlayer(enemyList[enemy]);

                    //Waits untik this returns true
                    yield return new WaitUntil(() => en.IsTurnOver());

                    //Reset the IsTurnOver to be false
                    en.SetIsTurnOver(false);

                    //Deal Damage to Player
                    _stats.UpdatePlayerHealth(-1 * en.GetBaseAttack());
                }

                //check if it we're using "good" or "bad" splash screens
                var splashScreen = CombatStats.amountHit >= (CombatStats.totalHits / 2) ? CombatController.instance.splashScreensGood : CombatController.instance.splashScreensBad;

                Debug.Log("Turn Over");

                splashScreen[splashScreen.Length - 1].gameObject.SetActive(true);
                //SplashAnim.SetBool(splashScreen[splashScreen.Length - 1].gameObject.name, true);

                yield return new WaitForSecondsRealtime(2f);

                
                splashScreen[splashScreen.Length - 1].gameObject.SetActive(false);

                yield return new WaitForSecondsRealtime(0.75f);

                if (_stats.playerHealth <= 0)
                {
                    yield break;
                }
            }

            yield return new WaitForEndOfFrame();

            StartCoroutine(EnemyPhase(enemy + 1));
            yield break;
        }

        enemyTurnOver = true;

        yield return new WaitForEndOfFrame();

        //Find the first non-defeated enemy to have selected
        for (int i = 0; i < _inBattle.Count; i++)
        {
            if (_inBattle[i] != null)
            {
                _selectedEnemy = i;
                break;
            }
        }

        //Play player turn SFX
        AudioManager.instance.SFX.clip = AudioManager.instance.UISFX[3];
        AudioManager.instance.SFX.Play();

        //Turn on the highlight
        ShowActionMenu();
        HighlightMenuItem();

        //Turn on the attack 
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);

        yield return new WaitForEndOfFrame();

        enemyTurnOver = false;
        Debug.Log("Enemy Phase Over");
        yield break;
    }

    //Turns off all the highlights and menus
    public void TurnOffHighlight()
    {
        menuSelect.SetActive(false);

        //turn off spotlight
        _enemyParent.transform.GetChild(0).gameObject.SetActive(false);
    }

    //Adds highlight to the battle menu
    public void HighlightMenuItem()
    {
        menuSelect.SetActive(true);

        if (attackMenuParent.activeSelf)
        {
            var x = attackMenu.transform.GetChild(_selectedAction);
            menuSelect.transform.position = x.position;
        }
        else if (itemMenuParent.activeSelf && itemList.Count > 0)
        {
            var x = itemMenu.transform.GetChild(_selectedItem);
            menuSelect.transform.position = x.position;
        }
    }

    //Returns to the Battle Menu
    public void ShowActionMenu()
    {
        //Clear Item Menu
        ClearItemMenu();

        //turn off the item and turn on the action
        itemMenuParent.SetActive(false);
        attackMenuParent.SetActive(true);

        selectedAction = ActionType.Punch;
        _selectedAction = 0;

        //restart the abiliy to chose an action
        StartCoroutine(ChooseAction());
    }

    //Show the Item Menu
    void ShowItemsMenu()
    {
        _selectedItem = 0;
        //get the objects for the items
        var text = attackMenu.transform.GetChild(0).gameObject;

        //Turn off/on the correct parents
        attackMenuParent.SetActive(false);
        itemMenuParent.SetActive(true);

        //Spawn each item
        foreach (var i in itemList)
        {
            var item = i.item.ToString().Replace('_', ' ');
            var obj = Instantiate(text, itemMenu.transform);

            obj.GetComponent<TextMeshProUGUI>().text = item + " (" + i.count + ")";
        }

        //Start the item choice coroutine
        StartCoroutine(ChooseItem());
    }

    //Clears all Items in the menu
    void ClearItemMenu()
    {
        foreach (Transform child in itemMenu.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetBasePlayerDamage(List<BeatMapStruct> player)
    {
        foreach (var a in player)
        {
            attackDamage.Add(a.base_damage);
        }
    }

    //Setter to tell the Combat Controller what enemies are on the board
    public void SetEnemies(List<EnemyType> e)
    {
        enemyList = e;
    }

    //Sets up the tutorial battle scene
    public void TutorialSetUp()
    {
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();
        _enemyParent = GameObject.FindGameObjectWithTag("Enemy Parent");

        PlaceEnemies();
    }

    public void SetUpSusanBattle()
    {
        Debug.Log("set up susan");
        //Find the stats 
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();

        //find the enemy parent
        _enemyParent = GameObject.FindGameObjectWithTag("Enemy Parent");

        //Place the enemies
        PlaceEnemies();

        //Display player health
        GameManager.instance.healthParent.SetActive(true);
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);

        //Set the Stats
        _stats.SetStats();
    }
}
