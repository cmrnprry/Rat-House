using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static EnemyController;

public enum ActionType
{
    Item = -1,
    Basic_Attack = 0,
}

public enum ItemType
{
    Basic_Heath = 0,
    Basic_Damage = 1,
}

public struct Items
{
    public Items(ItemType i, int c)
    {
        item = i;
        count = c;
    }

    public ItemType item { get; set; }
    public int count { get; set; }
}

public class CombatController : MonoBehaviour
{
    public static CombatController instance;

    [Header("Lists")]
    //List of enemy Types currently on the board
    public List<EnemyType> enemyList; //MAX OF 5

    //List of all potential player actions
    [SerializeField]
    private List<ActionType> _actionList;

    //List of all potential player item
    [SerializeField]
    private List<Items> _itemList = new List<Items>();

    //base damage that attacks can do
    public List<float> attackDamage;

    //List of enemy placements
    public List<Vector3> enemyPlacement;

    //list of enemies in battle
    [HideInInspector]
    public List<GameObject> _inBattle = new List<GameObject>();


    [Header("Currently Selected")]
    //Current selected player action
    public ActionType selectedAction;

    //Current indext of _actionList
    private int _selected = 0;

    //Current enemy selected
    private int _selectedEnemy = 0;

    //Current enemy selected
    private int _selectedItem = 0;

    //Checks to see if the player can select from the action list
    private bool _canSelect;

    //Keeps trackof player/enemy stats in battle
    private CombatStats _stats;

    [Header("Menus")]
    //Hide the player menu
    [HideInInspector]
    public GameObject battleMenu;
    public GameObject itemMenu;
    public GameObject menuSelect;

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

    void ResetBattle()
    {
        _selected = 0;
    }

    //Method to set up battles
    //Will basically place the enemies in the correct spots
    public void SetUpBattleScene()
    {
        //set the stats
        battleMenu = GameObject.FindGameObjectWithTag("BattleMenu");
        itemMenu = GameObject.FindGameObjectWithTag("ItemMenu");
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();
        menuSelect = GameObject.FindGameObjectWithTag("MenuSelect");

        _itemList = GameManager.instance.itemList;


        int index = 0;
        var parent = GameObject.FindGameObjectWithTag("Enemy Parent");
        foreach (var e in enemyList)
        {
            //Instasiate the enmy of Type
            GameObject enemy = Instantiate(Resources.Load("Enemies/" + e.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);

            //Add it to the list of enemy game objects
            _inBattle.Add(enemy);


            //Parent enemy
            enemy.transform.parent = parent.transform;

            //increase the index
            index++;
        }

        Debug.Log("in Battle Count: " + _inBattle.Count);

        _stats.SetStats();
    }

    //Handles the player choosing which action to take
    //TODO: Implement Item Menu
    public IEnumerator ChooseAction()
    {
        //Debug.Log("Choose Action");
        if (Input.GetButton("Up"))
        {
            if (_selected == 0)
            {
                _selected = _actionList.Count - 1;
            }
            else
            {
                _selected--;
            }
        }
        else if (Input.GetButton("Down"))
        {
            if (_selected == _actionList.Count - 1)
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

            switch (_actionList[_selected])
            {
                case ActionType.Basic_Attack:
                    Debug.Log("Basic Attack");
                    TurnOffHighlight();
                    StartCoroutine(ChooseEnemy());
                    break;
                case ActionType.Item:
                    ShowItems();
                    Debug.Log("Open Item Menu");
                    break;
                default:
                    Debug.LogError("Something has gone wrong in Combat Controller");
                    break;
            }

            _canSelect = false;
            yield break;
        }

        HighlightMenuItem();
        ShowSelectedAction();
        _canSelect = true;
        yield return new WaitForSecondsRealtime(0.20f);
        StartCoroutine(ChooseAction());
    }

    void TurnOffHighlight()
    {
        battleMenu.SetActive(false);
        menuSelect.SetActive(false);
        itemMenu.SetActive(false);

        //turn off particles
        var parent = GameObject.FindGameObjectWithTag("Enemy Parent");
        parent.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
    }

    void HighlightMenuItem()
    {
        menuSelect.SetActive(true);
        if (battleMenu.activeSelf)
        {
            var x = battleMenu.transform.GetChild(_selected);
            menuSelect.transform.position = x.position;
        }
        else if (itemMenu.activeSelf)
        {
            var x = itemMenu.transform.GetChild(_selectedItem);
            menuSelect.transform.position = x.position;
        }
    }

    void ShowItems()
    {
        var text = battleMenu.transform.GetChild(0).gameObject;
        battleMenu.SetActive(false);
        itemMenu.SetActive(true);

        foreach (var i in _itemList)
        {
            var item = i.item.ToString().Replace('_', ' ');
            var obj = Instantiate(text, itemMenu.transform);

            obj.GetComponent<TextMeshProUGUI>().text = item + " (" + i.count + ")";
        }

        StartCoroutine(ChooseItem());
    }

    public IEnumerator ChooseItem()
    {
        //Debug.Log("Choose Action");
        if (Input.GetButton("Up"))
        {
            if (_selectedItem == 0)
            {
                _selectedItem = _itemList.Count - 1;
            }
            else
            {
                _selectedItem--;
            }
        }
        else if (Input.GetButton("Down"))
        {
            if (_selectedItem == _itemList.Count - 1)
            {
                _selectedItem = 0;
            }
            else
            {
                _selectedItem++;
            }
        }
        else if (Input.GetButton("SelectAction") && _canSelect)
        {
            _canSelect = false;

            switch (_itemList[_selectedItem].item)
            {
                case ItemType.Basic_Heath:
                    Debug.Log("Basic Heath Item");
                    break;
                case ItemType.Basic_Damage:
                    Debug.Log("Basic Heath Damage");
                    break;
                default:
                    Debug.LogError("Something has gone wrong in Combat Controller");
                    break;
            }

            _canSelect = false;
            // yield break;
        }
        else if (Input.GetButton("Back"))
        {
            _canSelect = false;

            ReturnToBattleMenu();

            yield break;
        }

        HighlightMenuItem();
        _canSelect = true;
        yield return new WaitForSecondsRealtime(0.05f);
        StartCoroutine(ChooseItem());
    }

    void UseHealthItem(ItemType item)
    {

    }

    void ReturnToBattleMenu()
    {
        StartCoroutine(ChooseAction());
        battleMenu.SetActive(true);


        //Clear Item Menu
        foreach (Transform child in itemMenu.transform)
        {
            Destroy(child.gameObject);
        }


        itemMenu.SetActive(false);
    }

    IEnumerator ChooseEnemy()
    {
        if (Input.GetButton("Up"))
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
        else if (Input.GetButton("Down"))
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
        else if (Input.GetButton("SelectAction") && _canSelect)
        {
            TurnOffHighlight();
            StartCoroutine(AudioManager.instance.SetMap(_selected));

            _canSelect = false;
            yield break;
        }

        //TODO:Add some sort of visual display to show the selected enemy
        HighlightEnemy();
        Debug.Log("selected enemy: " + _inBattle[_selectedEnemy].name);

        _canSelect = true;
        yield return new WaitForSecondsRealtime(0.15f);
        StartCoroutine(ChooseEnemy());
    }

    void HighlightEnemy()
    {
        var parent = GameObject.FindGameObjectWithTag("Enemy Parent");
        var particles = parent.transform.GetChild(0);

        particles.transform.position = enemyPlacement[_selectedEnemy];

        particles.GetComponent<ParticleSystem>().Play();
    }

    //Tells the Combat Stats to deal with damage
    public void DealDamage()
    {
        //Want to send over what enemy was targeted
        //what attack was done
        //is defaulted to 0 untile multiple enemes are implemented
        Debug.Log("Selected Enemy: " + _selectedEnemy);
        _stats.DealDamageToEnemy(_selectedEnemy);
    }

    //Will play through the enemy turn
    // paramater here is used to track which enemy will be tal=king their turn
    // Default is 0
    public IEnumerator EnemyPhase(int enemy = 0)
    {
        /*
            For each enemy they should act according to their enemy type
            Basically code would look like :
                if (enemy >= _enemyList.Count)
                    EnemyAttack(_enemyList[enemy]) <- enemy will preform their attack and check if all the enemies have been defeated

                    yield return new WaitForEndOfFrame();

                    StartCoroutine(EnemyPhase(enemy + 1)); 
                else
                    give control back to the player
         */
        Debug.Log("Enemy Attack");

        yield return new WaitForEndOfFrame();

        //Give the player control back
        battleMenu.SetActive(true);

        //Find the first non-defeated enemy
        for (int i = 0; i < _inBattle.Count; i++)
        {
            if (_inBattle[i] != null)
            {
                _selectedEnemy = i;
                break;
            }
        }

        StartCoroutine(ChooseAction());
    }

    void ShowSelectedAction()
    {
        selectedAction = _actionList[_selected];
       // Debug.Log(selectedAction);
    }

    //Setter to tell the Combat Controller what enemies are on the board
    public void SetEnemies(List<EnemyType> e)
    {
        enemyList = e;
    }
}
