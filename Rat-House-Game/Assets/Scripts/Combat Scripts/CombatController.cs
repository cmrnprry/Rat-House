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

public class CombatController : MonoBehaviour
{
    public static CombatController instance;

    //Current selected player action
    public ActionType selectedAction;

    //Hide the player menu
    public GameObject battleMenu;

    //List of enemies currently on the board
    public List<EnemyType> enemyList; //MAX OF 5

    //List of enemy placements
    public List<Vector3> enemyPlacement;

    //base damage that attacks can do
    public List<float> attackDamage;


    //List of all potential player actions
    [SerializeField]
    private List<ActionType> _actionList;

    //Current indext of _actionList
    private int _selected = 0;

    //Checks to see if the player can select from the action list
    private bool _canSelect;

    //Keeps trackof player/enemy stats in battle
    private CombatStats _stats;

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


    private void Start()
    {
        //Add Enemy prefabs to the list
        //_enemyPrefabs.Add(GameObject.FindGameObjectWithTag("Coffee"));
        //_enemyPrefabs.Add(GameObject.FindGameObjectWithTag("Intern"));
        //_enemyPrefabs.Add(GameObject.FindGameObjectWithTag("Water_Cooler"));
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
        _stats = GameObject.FindGameObjectWithTag("CombatStats").GetComponent<CombatStats>();


        int index = 0;
        var parent = GameObject.FindGameObjectWithTag("Enemy Parent");
        foreach (var e in enemyList)
        {


            //GameObject obj = Resources.Load("/Enemies/" + e.ToString(), typeof(GameObject)) as GameObject;
            Debug.Log("/Enemies/" + e.ToString());
            GameObject enemy = Instantiate(Resources.Load("Enemies/" + e.ToString(), typeof(GameObject)) as GameObject, enemyPlacement[index], Quaternion.identity);
            enemy.transform.parent = parent.transform;

            //switch (e)
            //{
            //    case EnemyType.Coffee:
            //        MakeCoffeeEnemy(index, parent);
            //        break;
            //    case EnemyType.Water_Cooler:
            //        break;
            //    case EnemyType.Intern:
            //        break;
            //    default:
            //        Debug.LogError("Something has gone wrong with setting up the enemies");
            //        break;
            //}

            index++;
        }
    }

    void MakeCoffeeEnemy(int index, GameObject parent)
    {
        var obj = GameObject.FindGameObjectWithTag("Coffee");

        var enemy = Instantiate(obj, enemyPlacement[index], Quaternion.identity);
        enemy.transform.parent = parent.transform;
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
            switch (_actionList[_selected])
            {
                case ActionType.Basic_Attack:
                    Debug.Log("Basic Attack");
                    battleMenu.SetActive(false);
                    StartCoroutine(AudioManager.instance.SetMap(0));
                    break;
                //case ActionType.Item:
                //    Debug.Log("Open Item Menu");
                //    break;
                default:
                    Debug.LogError("Something has gone wrong in Combat Controller");
                    break;
            }

            _canSelect = false;
            yield break;
        }

        ShowSelectedAction();
        _canSelect = true;
        yield return new WaitForSecondsRealtime(0.20f);
        StartCoroutine(ChooseAction());
    }


    //Tells the Combat Stats to deal with damage
    public void DealDamage()
    {
        //Want to send over what enemy was targeted
        //what attack was done
        //is defaulted to 0 untile multiple enemes are implemented
        _stats.DealDamageToEnemy();
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
        StartCoroutine(ChooseAction());
    }

    void ShowSelectedAction()
    {
        selectedAction = _actionList[_selected];
        Debug.Log(selectedAction);
    }

    //Setter to tell the Combat Controller what enemies are on the board
    public void SetEnemies(List<EnemyType> e)
    {
        enemyList = e;
    }
}
