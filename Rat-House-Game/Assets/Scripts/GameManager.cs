using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public enum GameState
{
    Overworld = 0,
    Battle = 1,
    Boss = 2,
    CutScene = 3,
    Tutorial,
    Dead = 4,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Items the Player currently has
    //public List<Items> itemList = new List<Items>();

    [Header("Game State")]
    //Keeps track of the current game state
    [SerializeField]
    private GameState _currState = GameState.Overworld;

    //Keeps track of the current enemy that is being fought
    public GameObject currEnemy;

    //what level we're currently on
    public int level;

    [Header("Handles Difficulty")]
    //Number of times the player has retried a battle
    public int numberRetries = 0;

    [Header("UI Items")]
    //reference to the canvas
    public GameObject canvas;

    //Battle Menu Parent
    public GameObject battleParent;
    public Animator battleAnimator;

    //Dialogue Menu Parent
    public GameObject dialogueParent;

    //Death Screen Parent
    public GameObject deathScreenParent;

    //Health Bar Parent
    public GameObject healthParent;
    public GameObject topOverlay;


    [Header("Tutorial Script")]
    //tutorial
    public TutorialScript tutorial;

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

        // Do not destroy these objects, when we load a new scene.
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(canvas.gameObject);
    }


    private void Start()
    {
        //Items the player starts off with
        CombatController.instance.itemList.Add(new Items(ItemType.Basic_Heath, 3, 10));
        CombatController.instance.itemList.Add(new Items(ItemType.Basic_Damage, 2, 10));

        UpdateGameState();
    }

    private void Update()
    {
        //FOR TESTING
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetGameState(GameState.Overworld);
            tutorial.SkipTutorial();
            tutorial.anim.SetBool("isOpen", false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /** Method that is called when the game state needs to be updated
     * For now it starts in the Overworld until cutscenes are implemented
     * 
     **/
    private void UpdateGameState()
    {
        switch (_currState)
        {
            case GameState.Overworld:
                StartCoroutine(ReturnToOverworld());
                break;
            case GameState.Battle:
                StartCoroutine(StartBattle());
                break;
            case GameState.Boss:
                break;
            case GameState.CutScene:
                break;
            case GameState.Tutorial:
                StartTutorial();
                break;
            default:
                Debug.LogError("Something has gone wrong in GameState Update loop");
                break;
        }
    }

    //Makes ot so there is only one of each item type in the list
    public void CollapseItemList(List<Items> itemList)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            for (int k = i + 1; k < itemList.Count; k++)
            {
                if (itemList[i].item == itemList[k].item)
                {

                    var total = itemList[i].count + itemList[k].count;
                    var newItem = new Items(itemList[i].item, total);

                    itemList.RemoveAt(k);
                    itemList.RemoveAt(i);


                    if (newItem.count > 0)
                    {
                        itemList.Insert(i, newItem);
                    }


                }
            }
        }
    }

    /** Start Combat by:
    * Switching to the correct scene
    * Starting the background audio in the Audio Manager
    * Giving the player the ability to choose in the battle menu
    **/
    public IEnumerator StartBattle()
    {
        topOverlay.SetActive(false);

        SceneManager.LoadScene("Battle-FINAL");

        yield return new WaitForFixedUpdate();

        //Spawn the correct enemies 
        CombatController.instance.SetUpBattleScene();

        AudioManager.instance.StartCombatMusic();
        StartCoroutine(CombatController.instance.ChooseAction());
    }

    //Returns to the overworld from a differnt scene
    private IEnumerator ReturnToOverworld()
    {
        //reset the number of retries to 0
        numberRetries = 0;

        //Turn off the battle music
        AudioManager.instance.StopCombatMusic();

        //Turn off the battle UI
        battleAnimator.SetBool("IsOpen", false);

        //If we were in the battle scene, make sure to clear it out
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Battle-FINAL"))
            CombatController.instance.ClearBattle();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();


        topOverlay.SetActive(true);

        //Load the Overworld Scene
        SceneManager.LoadScene("Overworld_Level1-FINAL");

        //Change enemy to Beaten
        if (currEnemy != null)
            currEnemy.GetComponent<EnemyController>().isBeaten = true;

        yield return new WaitForEndOfFrame();

        //Show any Dialogue
    }

    //The Battle was won
    //TODO: Set the beaten enemy to Beaten
    public IEnumerator BattleLost()
    {
        //Play some sort of death animation or something

        //while animaiotn is playing, return null

        yield return null;

        deathScreenParent.SetActive(true);
    }

    //Retry the current battle
    public void RetryBattle()
    {
        numberRetries++;
        CombatController.instance.ResetBattle();
    }

    //Retrun to the overworld
    public void QuitBattle()
    {
        SetGameState(GameState.Overworld);
    }

    //The Battle was won
    //TODO: Set the beaten enemy to Beaten
    public IEnumerator BattleWon()
    {
        //Play win music if any

        //Play win anim if any

        //Show new attack gained if any

        yield return new WaitForEndOfFrame();

        //Set Game State
        SetGameState(GameState.Overworld);
    }

    //Set the current game state
    public void SetGameState(GameState state)
    {
        if (_currState != state)
        {
            _currState = state;
            UpdateGameState();
        }
    }

    //Get the current game state
    public GameState GetGameState()
    {
        return _currState;
    }

    //Handles the tutorial stuff
    void StartTutorial()
    {
        //open the text box and start dialogue
        tutorial.anim.SetBool("isOpen", true);

        //set thesentences in the dialogue manager
        tutorial.dialogue.sentences = tutorial.beforeBattleDialogue;

        tutorial.dialogue.StartDialogue();

        //start the dialogue in the tutorial script
        StartCoroutine(tutorial.ShowOpeningDialogue());
    }
}
