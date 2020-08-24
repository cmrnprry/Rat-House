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
    Susan = 2,
    CutScene = 3,
    Tutorial,
    Dead = 4,
    AfterOverworld = 5,
    SkipTutorial
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game State")]
    //Keeps track of the current game state
    [SerializeField]
    private GameState _currState;

    //Keeps track of the current enemy that is being fought
    public GameObject currEnemy;

    //what level we're currently on
    public int level = 1;
    [TextArea(3, 5)]
    public string[] levelOneDialogue;
    [TextArea(3, 5)]
    public string[] levelTwoDialogue;

    public bool tempWait;
    public bool isSusanBattle = false;

    [Header("Handles Difficulty")]
    //Number of times the player has retried a battle
    public int numberRetries = 0;

    [Header("UI Items")]
    //reference to the canvas
    public GameObject canvas;

    public GameObject pauseMenu;

    //Overworld Inventoy
    public GameObject inventoryParent;
    public GameObject inventoryItems;
    public GameObject item;
    public Sprite[] itemImages;

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

    [Header("Dialogue")]
    public Animator diaAnim;
    public Dialogue dialogue;
    public bool dialogueOver = false;
    public bool dialogueInProgress = false;
    private int _index = 0;

    [Header("Tutorial Script")]
    //tutorial
    public TutorialScript tutorial;

    [Header("Scene Objects")]
    public Susan susan;
    public Animator anim;
    public GameObject[] overworldLevelOne;
    public PlayerController player;

    [Header("Key Items")]
    public Animator itemGetAnim;
    public TextMeshProUGUI itemText;
    public bool hasKey = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            Destroy(canvas.gameObject);
        }
        else
        {
            instance = this;
            // Do not destroy these objects, when we load a new scene.
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(canvas.gameObject);
        }
    }

    private void Start()
    {
        //Items the player starts off with
        CombatController.instance.itemList.Add(new Items(ItemType.Jims_Lunch, 2, 15, StatusEffect.Cures_Poison));
        CombatController.instance.itemList.Add(new Items(ItemType.Hot_Coffee, 1, 15, StatusEffect.Burn));
        CombatController.instance.itemList.Add(new Items(ItemType.Plastic_Utensils, 2, 10, StatusEffect.Bleed));
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu") && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("LastScene")))
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            pauseMenu.GetComponent<PauseMenu>().MainPause();
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        }
    }

    IEnumerator PauseMenu()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Pause"));
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        StartCoroutine(PauseMenu());
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

    public void OpenInventory()
    {
        if (inventoryParent.activeSelf)
        {
            foreach (Transform child in inventoryItems.transform)
            {
                Destroy(child.gameObject);
            }

            inventoryParent.SetActive(false);
        }
        else
        {
            inventoryParent.SetActive(true);
            foreach (var it in CombatController.instance.itemList)
            {
                var i = it.item.ToString().Replace('_', ' ');
                var obj = Instantiate(item, inventoryItems.transform);
                obj.gameObject.SetActive(true);

                string desctription = "Deals " + it.delta + " damage and causes the " + it.effect.ToString() + " status effect";
                if ((int)it.item == 0 || (int)it.item == 4)
                {
                    desctription = "Heals " + it.delta + " damage and cures the " + it.effect.ToString().Substring(6) + " status effect";
                }

                obj.GetComponent<TextMeshProUGUI>().text = i + " (" + it.count + ") - " + desctription;
                foreach (var s in itemImages)
                {
                    if (s.name == it.item.ToString())
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = s;
                        break;
                    }
                }
            }
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
                Debug.Log("Battle");
                StartCoroutine(StartBattle());
                break;
            case GameState.Susan:
                Debug.Log("Susan");
                isSusanBattle = true;
                susan.SetDialogue(susan.preBattleDialogue);
                break;
            case GameState.AfterOverworld:
                StartCoroutine(AfterTutorial());
                break;
            case GameState.Tutorial:
                StartTutorial();
                break;
            case GameState.SkipTutorial:
                StartCoroutine(SkipTutorial());
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
                    var newItem = new Items(itemList[i].item, total, itemList[i].delta, itemList[i].effect);

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
        //play some sort of screen wipe
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSeconds(2);
        TurnOffScene();

        yield return new WaitForFixedUpdate();

        topOverlay.SetActive(false);
        SceneManager.LoadScene("Battle-FINAL", LoadSceneMode.Additive);
        yield return new WaitForFixedUpdate();



        yield return new WaitForSeconds(2);
        AudioManager.instance.StartCombatMusic();

        topOverlay.SetActive(false);

        anim.CrossFade("Fade_In", 1);

        yield return new WaitForFixedUpdate();

        //Spawn the correct enemies 
        CombatController.instance.SetUpBattleScene();
    }

    public void SetEnemyDialogue(string[] dia)
    {
        //if there's no dialogue to be set
        if (dia.Length <= 0)
        {
            dialogueOver = true;
            dialogueInProgress = false;
            return;
        }

        dialogueInProgress = true;
        diaAnim.SetBool("isOpen", true);
        dialogue.sentences = dia;
        dialogue.StartDialogue();
        StartCoroutine(ShowEnemyDialogue());
    }

    IEnumerator ShowEnemyDialogue()
    {
        //Waits for the text to stop typing
        yield return new WaitUntil(() => dialogue.isTyping == false);

        //wait for the player to press enter/z
        yield return new WaitUntil(() => Input.GetButton("SelectAction"));

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == dialogue.sentences.Length)
        {
            //Lower the text box
            diaAnim.SetBool("isOpen", false);

            //reset the index to 0
            _index = 0;

            yield return new WaitForSecondsRealtime(.25f);

            dialogueOver = true;
            dialogueInProgress = false;

            yield break;
        }

        //increase the index
        _index++;

        //load next sentence
        dialogue.NextSentence();

        //Restart the coroutine
        StartCoroutine(ShowEnemyDialogue());
        yield break;

    }

    //Returns to the overworld from a differnt scene
    private IEnumerator ReturnToOverworld()
    {
        //reset the number of retries to 0
        numberRetries = 0;

        //play some sort of screen wipe
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSeconds(2);

        //Turn off the battle music
        AudioManager.instance.StopCombatMusic();

        //Turn off the battle UI
        battleAnimator.SetBool("IsOpen", false);
        TurnOffBattleMenus();
        TurnOnScene();

        //If we were in the battle scene, make sure to clear it out
        CombatController.instance.ClearBattle();

        //UnLoad the Battle Scene
        SceneManager.UnloadSceneAsync("Battle-FINAL");

        anim.CrossFade("Fade_In", 1);

        yield return new WaitForEndOfFrame();

        //Show any Dialogue
        if (currEnemy.GetComponent<EnemyController>().isBeaten)
        {
            SetEnemyDialogue(currEnemy.GetComponent<EnemyController>().postBattleDialogue);
            yield return new WaitUntil(() => dialogueOver);
        }

        //Give player movement 
        StartCoroutine(player.PlayerMovement());
        dialogueOver = false;
    }

    //The Battle was won
    //TODO: Set the beaten enemy to Beaten
    public IEnumerator BattleWon()
    {
        //Play win music if any

        //Play win anim if any

        //Show new attack gained if any

        //Change enemy to Beaten
        if (currEnemy != null)
            currEnemy.GetComponent<EnemyController>().isBeaten = true;

        yield return new WaitForEndOfFrame();

        //Set Game State
        SetGameState(GameState.Overworld);
    }

    //The Battle was won
    //TODO: Set the beaten enemy to Beaten
    public IEnumerator BattleLost()
    {
        //Play some sort of death animation or something

        //while animaiotn is playing, return null

        yield return null;

        TurnOffBattleMenus();
        deathScreenParent.SetActive(true);
    }

    //Retry the current battle
    public void RetryBattle()
    {
        numberRetries++;
        topOverlay.SetActive(false);
        healthParent.SetActive(true);
        CombatController.instance.ResetBattle();
    }

    //Retrun to the overworld
    public void QuitBattle()
    {
        CombatController.instance.ClearBattle();
        tempWait = true;
        SetGameState(GameState.Overworld);
    }

    void TurnOffBattleMenus()
    {
        //Turn off the battle music
        AudioManager.instance.StopCombatMusic();

        //Turn off the battle UI
        deathScreenParent.SetActive(false);
        battleAnimator.SetBool("IsOpen", false);
        healthParent.SetActive(false);
        topOverlay.SetActive(true);
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

    IEnumerator SkipTutorial()
    {
        yield return new WaitForSecondsRealtime(1f);
        overworldLevelOne = SceneManager.GetActiveScene().GetRootGameObjects();

        SetEnemyDialogue(levelOneDialogue);

        yield return new WaitUntil(() => dialogueInProgress);
        yield return new WaitUntil(() => !dialogueInProgress);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(player.PlayerMovement());
    }

    private IEnumerator AfterTutorial()
    {
        //play some sort of screen wipe
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSeconds(2);

        //Turn off the battle music
        AudioManager.instance.StopCombatMusic();

        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene("Overworld_Level1-FINAL");

        yield return new WaitForEndOfFrame();

        anim.CrossFade("Fade_In", 1);

        yield return new WaitForEndOfFrame();

        SetEnemyDialogue(levelOneDialogue);

        yield return new WaitUntil(() => dialogueInProgress);
        yield return new WaitUntil(() => !dialogueInProgress);

        Debug.Log("here");

        overworldLevelOne = SceneManager.GetActiveScene().GetRootGameObjects();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(player.PlayerMovement());
        dialogueOver = false;
    }

    public IEnumerator LoadLevelTwo()
    {
        hasKey = false;

        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Overworld_Level2-FINAL");
        level = 2;
        anim.CrossFade("Fade_In", 1);
        yield return new WaitForSeconds(1);
        AudioManager.instance.bgMusic.clip = AudioManager.instance.bgClips[level];
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        SetEnemyDialogue(levelTwoDialogue);

        yield return new WaitUntil(() => dialogueInProgress);
        yield return new WaitUntil(() => !dialogueInProgress);

        StartCoroutine(player.PlayerMovement());
        overworldLevelOne = SceneManager.GetActiveScene().GetRootGameObjects();
    }

    public IEnumerator LoadBreakRoom()
    {
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Overworld_BreakRoom_FINAL");
        anim.CrossFade("Fade_In", 1);
        yield return new WaitForSeconds(1);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(player.PlayerMovement());
        overworldLevelOne = SceneManager.GetActiveScene().GetRootGameObjects();
    }
}
