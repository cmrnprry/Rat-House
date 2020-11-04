using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static EnemyController;
using System;

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
    Calmy_Tea = 0, // heals small heath, fixes burn
    Plastic_Utensils = 1,// does damage, cause bleed
    Hot_Coffee = 2, //does damage, causes buring
    Pams_Fruitcake = 3,//hit's a lil, causes poisioning
    Jims_Lunch = 4,// heals a lil, fixes poision
}

//Number tells how much damage per turn effect does
public enum StatusEffect
{
    None = -1,
    Cures_Burn = 0,
    Cures_Poison = 1,
    Cures_Bleed = 2,
    Burn = 10,
    Poison = 12,
    Bleed = 8
}

public struct Items
{
    //i = item type
    //c = number of item in inventory
    //d = amount of health/damage item does, if any
    //e = status effect inflicted, if any
    public Items(ItemType i, int c, int d, StatusEffect e)
    {
        item = i;
        count = c;
        delta = d;
        effect = e;
    }

    public ItemType item { get; set; }
    public int count { get; set; }
    public int delta { get; set; }
    public StatusEffect effect { get; set; }
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
    public ActionType selectedActionType;
    private int _selectedItem = 0;

    //Current index of _actionList
    private int _selectedAction = 0;

    //if the player is using an item, this is it
    private Items itemUsed;

    //Current enemy selected
    private int _selectedEnemy = 0;
    private int _battleStart = 0;
    private int _battleEnd;

    //Checks to see if the player can select from the action list
    public bool enemyTurnOver = false;

    //Keeps trackof player/enemy stats in battle
    [HideInInspector]
    public CombatStats _stats;

    [Header("Menus")]
    public GameObject attackMenuParent;
    public GameObject attackMenu;
    public GameObject itemMenuParent;
    public GameObject itemMenu;
    public GameObject menuSelect;
    private GameObject _enemyParent;
    private GameObject _enemyEffects;

    [Header("UI")]
    public Animator heartAnim;
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI hitDetectionText;
    public Animator SplashAnim;
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


    /****************************   HANDLES SETTING UP BATTLE   **********************************************/


    public void SetUpBattleScene()
    {
        //Find the stats 
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();

        //find the enemy parent
        _enemyParent = GameObject.FindGameObjectWithTag("Enemy Parent");
        _enemyEffects = GameObject.FindGameObjectWithTag("Enemy Effects");

        PlaceEnemies();
        _stats.SetStats();

        _battleEnd = _inBattle.Count - 1;
        _battleStart = 0;

        //Display player health
        GameManager.instance.healthParent.SetActive(true);
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);

        ShowActionMenu();
        HighlightMenuItem();
    }

    public void PlaceEnemies()
    {
        int index = 0;

        foreach (var e in enemyList)
        {
            //Instasiate the enmy of Type
            GameObject enemy = Instantiate(Resources.Load("Enemies/" + e.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);

            //Add it to the list of enemy game objects
            _inBattle.Add(enemy);

            enemyHealthBars[index].gameObject.SetActive(true);
            enemyHealthBars[index].value = 1;

            //Set enemy health

            _ = (e == EnemyType.Susan) ? GameManager.instance.susan.healthSlider = enemyHealthBars[index] : enemy.GetComponent<Enemy>().healthSlider = enemyHealthBars[index];

            //Parent enemy
            enemy.transform.parent = _enemyParent.transform;

            //increase the index
            index++;
        }


    }

    //For adding an enemy to a battle
    public void AddEnemy(EnemyType enemy)
    {
        //set index to be the length
        int index = _inBattle.Count;

        //Check if any enemies were killed, if so rest the index
        for (int i = 1; i < _inBattle.Count; i++)
        {
            if (_inBattle[i] == null)
            {
                index = i;
                break;
            }
        }

        if (index >= 5)
        {
            index = ReplaceWeakerEnemies();
        }

        Debug.Log("Index: " + index);
        Debug.Log("add enemy of type: " + enemy.ToString());

        //Instasiate the enmy of Type
        GameObject newE = Instantiate(Resources.Load("Enemies/" + enemy.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);

        if (index < _inBattle.Count)
        {
            if (_inBattle[index] != null)
                Destroy(_inBattle[index].gameObject);

            _inBattle[index] = newE;
            enemyList[index] = enemy;
        }
        else
        {
            _inBattle.Add(newE);
            enemyList.Add(enemy);
            _stats.enemyHealth.Add(0);
        }



        //Set enemy health
        _stats.enemyHealth[index] = newE.GetComponent<Enemy>().GetStartingHealth();
        enemyHealthBars[index].gameObject.SetActive(true);
        enemyHealthBars[index].value = 1;
        newE.GetComponent<Enemy>().healthSlider = enemyHealthBars[index];

        //Parent enemy
        newE.transform.parent = _enemyParent.transform;
        _battleEnd = _inBattle.Count - 1;
        _battleStart = 0;
    }

    int ReplaceWeakerEnemies()
    {
        var index = 0;

        //look to see if the lst still contains them, if yes, replace them
        if (enemyList.Contains(EnemyType.Intern))
        {
            index = enemyList.IndexOf(EnemyType.Intern);
        }
        else if (enemyList.Contains(EnemyType.Coffee))
        {
            index = enemyList.IndexOf(EnemyType.Coffee);

        }
        else if (enemyList.Contains(EnemyType.Water_Cooler))
        {
            index = enemyList.IndexOf(EnemyType.Water_Cooler);

        }

        return index;
    }

    public void SetEnemies(List<EnemyType> e)
    {
        enemyList = e;
    }

    public void TutorialSetUp()
    {
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();
        _enemyParent = GameObject.FindGameObjectWithTag("Enemy Parent");
        _enemyEffects = GameObject.FindGameObjectWithTag("Enemy Effects");

        PlaceEnemies();
    }

    public void SetUpSusanBattle()
    {
        Debug.Log("set up susan");
        //Find the stats 
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();

        //find the enemy parent
        _enemyParent = GameObject.FindGameObjectWithTag("Enemy Parent");
        _enemyEffects = GameObject.FindGameObjectWithTag("Enemy Effects");

        //Place the enemies
        PlaceEnemies();
        GameManager.instance.susan.anim = _enemyParent.transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();
        _battleEnd = _inBattle.Count - 1;
        _battleStart = 0;

        //Display player health
        GameManager.instance.healthParent.SetActive(true);
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);

        //Set the Stats
        _stats.SetStats();
    }

    public void SetBasePlayerDamage(List<BeatMapStruct> player)
    {
        foreach (var a in player)
        {
            attackDamage.Add(a.base_damage);
        }
    }


    /****************************   HANDLES RESTARTING BATTLE   **********************************************/


    //Holds all the values to reset the battle
    public void ResetBattle()
    {
        PlaceEnemies();

        //Reset Stats
        _stats.SetStats();
        _stats.gameObject.GetComponent<Note>().ShowAttackBeats();
        _stats.RemoveEffect();
        ResetSlider();

        AudioManager.instance.StartCombatMusic();
        GameManager.instance.deathScreenParent.SetActive(false);

        StopAllCoroutines();

        ShowActionMenu();
        HighlightMenuItem();

        GameManager.instance.battleAnimator.SetBool("IsOpen", true);
    }

    public void ResetSlider()
    {
        //TODO: MAKE THIS NOT HARD CODED IN I DONT FORSEE THE NUMBERS CHSNGING BUT ITS BAD FIX IT
        _stats.gameObject.transform.position = new Vector3(3f, 6.19f, 0f);
        _stats.gameObject.GetComponent<Note>().Flip(1);
    }

    public void ClearBattle()
    {
        StopAllCoroutines();
        Debug.Log("clear");
        //turn on/off corect panels
        itemMenuParent.SetActive(false);
        attackMenuParent.SetActive(true);

        //Reset selected
        _selectedAction = 0;
        _selectedItem = 0;
        _selectedEnemy = 0;
        selectedActionType = ActionType.Punch;

        int index = 0;

        foreach (Transform child in _enemyParent.transform)
        {
            enemyHealthBars[index].gameObject.SetActive(false);
            Destroy(child.gameObject);
            index++;
        }

        ClearItemMenu();

        //Reset Enemy lists
        _inBattle = new List<GameObject>();
    }

    /****************************   HANDLES BATTLE OPTIONS   **********************************************/


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
            TurnOffHighlight();
            GameManager.instance.battleAnimator.SetBool("IsOpen", false);

            //Select the correct action
            if (_actionList[_selectedAction] != ActionType.Heal)
            {
                StartCoroutine(ChooseEnemy());
            }
            else
            { StartCoroutine(ChoosePlayer()); }


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

            selectedActionType = ActionType.Item;

            //Wait a frame before showing anything
            yield return new WaitForEndOfFrame();

            //Switch to the menu selection
            ShowItemsMenu();
            yield return null;
            HighlightMenuItem();

            //reset the selected action to 0
            _selectedAction = 0;
            yield break;
        }

        HighlightMenuItem();

        selectedActionType = _actionList[_selectedAction];

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

                if (itemList[_selectedItem].item == ItemType.Calmy_Tea || itemList[_selectedItem].item == ItemType.Jims_Lunch)
                {
                    StartCoroutine(ChoosePlayer(true));
                }
                else
                { UseDamageItem(itemList[_selectedItem]); }

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

            yield return new WaitForEndOfFrame();

            //Switch to the action selection
            ShowActionMenu();
            HighlightMenuItem();
            yield break;
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(ChooseItem());
    }

    IEnumerator ChooseEnemy(bool isItem = false)
    {
        //Wait until a correct key is pressed
        yield return new WaitUntil(() => Input.GetButtonDown("Up") || Input.GetButtonDown("Down") || Input.GetButtonDown("Left") || Input.GetButtonDown("Right") ||
        Input.GetButtonDown("SelectAction") || Input.GetButtonDown("Back"));

        if (Input.GetButtonDown("Up") || Input.GetButtonDown("Left"))
        {
            if (_selectedEnemy == _battleStart)
            {
                _selectedEnemy = _battleEnd;
            }
            else
            {
                if (_inBattle[_selectedEnemy - 1] == null)
                {
                    _selectedEnemy--;
                }
                _selectedEnemy--;
            }

        }
        else if (Input.GetButtonDown("Down") || Input.GetButtonDown("Right"))
        {
            if (_selectedEnemy == _battleEnd)
            {
                _selectedEnemy = _battleStart;
            }
            else
            {
                if (_inBattle[_selectedEnemy + 1] == null)
                {
                    _selectedEnemy++;
                }
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
                Debug.Log("item: " + isItem + " " + itemUsed.effect.ToString() + " " + itemUsed.delta);
                DealDamage(true);
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

        HighlightEnemy();

        yield return new WaitForEndOfFrame();
        StartCoroutine(ChooseEnemy(isItem));
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


    /****************************   HANDLES USING ITEMS AND CAUSING DAMAGE   **********************************************/


    void UseDamageItem(Items item)
    {
        Debug.Log("item: " + item.item.ToString() + " " + item.delta);

        //Choose the item to use
        itemUsed = item;
        StartCoroutine(ChooseEnemy(true));

        //decrease the amount of the used item
        itemList.Add(new Items(item.item, -1, item.delta, item.effect));
        GameManager.instance.CollapseItemList(itemList);
    }

    IEnumerator UseHealthItem(Items item)
    {
        //decrease the amount of the used item
        itemList.Add(new Items(item.item, -1, item.delta, item.effect));
        GameManager.instance.CollapseItemList(itemList);

        //update the player's health
        itemUsed = item;
        _stats.itemUsed = itemUsed;
        Debug.Log(itemUsed.ToString());
        _stats.UpdatePlayerHealth(item.delta);

        yield return new WaitForSecondsRealtime(0.75f);

        //TODO: MAKE THIS NOT HARD CODED IN I DONT FORSEE THE NUMBERS CHSNGING BUT ITS BAD FIX IT
        _stats.gameObject.transform.position = new Vector3(12.5f, 6.19f, 0f);

        //Start Enemy Phase & play sfx
        AudioManager.instance.SFX.clip = AudioManager.instance.UISFX[2];
        AudioManager.instance.SFX.Play();
        StartCoroutine(EnemyPhase());
    }

    public void DealDamage(bool isItem = false)
    {
        Debug.Log("Selected Enemy: " + _selectedEnemy);

        //TODO: MAKE THIS NOT HARD CODED IN I DONT FORSEE THE NUMBERS CHSNGING BUT ITS BAD FIX IT
        _stats.SliderEnemy();

        _stats.itemUsed = itemUsed;
        StartCoroutine(_stats.DealDamageToEnemy(_selectedEnemy, isItem));
    }



    /****************************   HANDLES HIGHLIGHTING   **********************************************/


    public void HighlightPlayer()
    {
        var spotlight = _enemyEffects.transform.GetChild(0);

        spotlight.gameObject.SetActive(true);
        spotlight.transform.localPosition = new Vector3(-4.14f, 0f, -0.14f);
    }

    public void HighlightEnemy()
    {
        var spotlight = _enemyEffects.transform.GetChild(0);

        spotlight.gameObject.SetActive(true);
        spotlight.transform.position = enemyPlacement[_selectedEnemy];
    }

    public void HighlightMenuItem()
    {
        menuSelect.SetActive(true);

        if (attackMenuParent.activeSelf)
        {
            var x = attackMenu.transform.GetChild(_selectedAction).GetChild(0);
            menuSelect.transform.position = x.position;
        }
        else if (itemMenuParent.activeSelf && itemList.Count > 0)
        {
            var x = itemMenu.transform.GetChild(_selectedItem).GetChild(0);
            menuSelect.transform.position = x.position;
        }
    }

    public void TurnOffHighlight()
    {
        menuSelect.SetActive(false);

        //turn off spotlight
        _enemyEffects.transform.GetChild(0).gameObject.SetActive(false);
    }



    /****************************   HANDLES ENEMY TURN  **********************************************/


    public IEnumerator EnemyPhase(int enemy = 0)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Debug.Log("Enemy Phase Start");
        Image[] splashScreen = null;

        //for each non-defeated enemy, do their attacks
        if (enemy < _inBattle.Count)
        {
            if (_inBattle[enemy] != null)
            {
                float damage = 0;
                GameObject enemyObj = _inBattle[enemy];
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();

                if (enemyObj.name == "Susan(Clone)")
                {
                    Susan susan = GameManager.instance.susan;
                    StartCoroutine(susan.SusanAttack());

                    yield return new WaitUntil(() => susan.IsTurnOver());

                    StartCoroutine(AudioManager.instance.WaitUntilNextBeat(Math.Round(AudioManager.instance.songPositionInBeats, MidpointRounding.AwayFromZero)));
                    yield return new WaitUntil(() => AudioManager.instance.nextBeat);

                    susan.Idle();
                    AudioManager.instance.nextBeat = false;

                    splashScreen = (CombatStats.amountHit >= (CombatStats.totalHits / 2) && CombatStats.amountHit != 0) ? CombatController.instance.splashScreensGood : CombatController.instance.splashScreensBad;
                    //_stats.UpdatePlayerHealth(-1 * susan.GetBaseAttack());
                    damage = susan.GetBaseAttack();

                    susan.SetIsTurnOver(false);

                    yield return new WaitForEndOfFrame();

                    if (susan._currentHealth <= 0 || susan.nextPhase)
                    {
                        yield break;
                    }
                }
                else
                {
                    enemyScript.AttackPlayer(enemyList[enemy]);

                    yield return new WaitUntil(() => enemyScript.IsTurnOver());
                    enemyScript.SetIsTurnOver(false);

                    splashScreen = (CombatStats.amountHit >= (CombatStats.totalHits / 2) && CombatStats.amountHit != 0) ? CombatController.instance.splashScreensGood : CombatController.instance.splashScreensBad;
                    // _stats.UpdatePlayerHealth(-1 * enemyScript.GetBaseAttack());
                    damage = enemyScript.GetBaseAttack();

                    //Update the enemy effect if any
                    enemyScript.UpdateEffect();

                    _stats.enemyHealth[enemy] = enemyScript._currentHealth;
                    if (enemyScript._currentHealth <= 0)
                    {
                        StartCoroutine(_stats.EnemyDeath(enemy, enemyScript));
                    }
                }

                Debug.Log("Turn Over");

                //turn on splashscreens and play animation
                splashScreen[splashScreen.Length - 1].gameObject.SetActive(true);

                string animation = "Base Layer." + splashScreen[splashScreen.Length - 1].gameObject.name;
                CombatController.instance.SplashAnim.Play(animation, 0, 0f);

                //Update Player health
                _stats.UpdatePlayerHealth(-1 * damage);
                yield return new WaitForSecondsRealtime(2f);

                splashScreen[splashScreen.Length - 1].gameObject.SetActive(false);

                if (!_stats.hasEffect)
                    PlayerStatusEffect(enemyList[enemy]);

                //if the player is dead, break
                if (_stats.playerHealth <= 0)
                { yield break; }

                yield return new WaitForSecondsRealtime(0.75f);
            }

            yield return new WaitForEndOfFrame();

            StartCoroutine(EnemyPhase(enemy + 1));
            yield break;
        }

        enemyTurnOver = true;

        if (_stats._enemiesLeft <= 0)
        {
            StartCoroutine(GameManager.instance.BattleWon());
            yield break;
        }

        yield return new WaitForEndOfFrame();

        //Find the first non-defeated enemy to have selected
        for (int i = 0; i < _inBattle.Count; i++)
        {
            if (_inBattle[i] != null)
            {
                _selectedEnemy = i;
                _battleStart = i;
                break;
            }
        }

        FindEnd(_inBattle.Count - 1);

        //Play player turn SFX
        AudioManager.instance.SFX.clip = AudioManager.instance.UISFX[3];
        AudioManager.instance.SFX.Play();

        if (_stats.hasEffect)
            _stats.UpdateEffect();

        ShowActionMenu();
        HighlightMenuItem();
        GameManager.instance.battleAnimator.SetBool("IsOpen", true);

        yield return new WaitForEndOfFrame();

        enemyTurnOver = false;
        yield break;
    }

    void PlayerStatusEffect(EnemyType e)
    {
        Debug.Log("Effect?");
        int effectChance = UnityEngine.Random.Range(0, 101);
        var effect = StatusEffect.None;

        if (e == EnemyType.Water_Cooler)
        {
            effect = (effectChance <= 20) ? StatusEffect.Bleed : StatusEffect.None;

        }
        else if (e == EnemyType.Coffee)
        {
            effect = (effectChance <= 20) ? StatusEffect.Burn : StatusEffect.None;
        }
        else if (e == EnemyType.Computer_Man)
        {
            if (effectChance <= 15)
            {
                effect = StatusEffect.Bleed;
            }
            else if (effectChance <= 45)
            {
                effect = StatusEffect.Burn;
            }
        }
        else if (e == EnemyType.Susan)
        {
            if (effectChance <= 10)
            {
                effect = StatusEffect.Bleed;
            }
            else if (effectChance <= 30)
            {
                effect = StatusEffect.Burn;
            }
            else if (effectChance <= 60)
            {
                effect = StatusEffect.Poison;
            }
        }

        if (effect != StatusEffect.None)
            _stats.SetStatusEffect(effect);
    }

    public string GetColor(StatusEffect effect)
    {
        string color = "";
        if (effect == StatusEffect.Bleed || effect == StatusEffect.Cures_Bleed)
        {
            color = "#C0466C";
        }
        else if (effect == StatusEffect.Burn || effect == StatusEffect.Cures_Burn)
        {
            color = "#EE7042";

        }
        else if (effect == StatusEffect.Poison || effect == StatusEffect.Cures_Poison)
        {
            color = "#319638";

        }

        return color;
    }

    void FindEnd(int index)
    {
        if (_inBattle[index] != null)
        {
            _battleEnd = index;
        }
        else
        {
            FindEnd(index - 1);
        }

    }


    /****************************   HANDLES CHANGING OR CLEARING MENUS   **********************************************/


    public void ShowActionMenu()
    {
        Debug.Log("start Action");
        //Clear Item Menu
        ClearItemMenu();

        //turn off the item and turn on the action
        itemMenuParent.SetActive(false);
        attackMenuParent.SetActive(true);

        selectedActionType = ActionType.Punch;
        _selectedAction = 0;

        //restart the abiliy to chose an action
        StartCoroutine(ChooseAction());
    }

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

    void ClearItemMenu()
    {
        foreach (Transform child in itemMenu.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
