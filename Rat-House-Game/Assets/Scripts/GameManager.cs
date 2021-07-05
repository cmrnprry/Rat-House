using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public enum GameState
{
    Overworld = 0,
    Battle = 1,
    Susan = 2,
    Tutorial = 3,
    AfterTutorial = 4,
    SkipTutorial = 5
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

    public bool tempWait = true;
    public bool isSusanBattle = false;

    [Header("UI Items")]
    //reference to the canvas
    public GameObject canvas;
    public GameObject pauseMenu;
    public GameObject susanVideo;
    public VideoPlayer susanVideoPlayer;

    [Header("Overworld Inventoy")]
    public InventoryMenu inventory;
    public GameObject invParent;

    [Header("Battle Menu Parent")]
    public GameObject battleParent;
    public Animator battleAnimator;

    [Header("Dialogue Menu Parent")]
    public GameObject dialogueParent;

    [Header("Death Screen Parent")]
    public GameObject deathScreenParent;

    [Header("Health Bar Parent")]
    public GameObject healthParent;
    public GameObject topOverlay;

    [Header("Dialogue")]
    public Animator diaAnim;
    public Dialogue dialogue;
    public bool dialogueOver = false;
    public bool dialogueInProgress = false;
    public bool transition = false;
    private int _index = 0;

    [Header("Tutorial Script")]
    //tutorial
    public TutorialScript tutorial;
    public GameObject tutorialPage;

    [Header("Scene Objects")]
    public Animator anim;
    public GameObject[] overworldObjects;
    public PlayerController player;
    public GameObject finalImage;

    [Header("Key Items")]
    public Animator itemGetAnim;
    //public GameObject itemGetOBJ;
    public TextMeshProUGUI itemText;
    public bool hasKey = false;

    [HideInInspector]
    public Susan susan;

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
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu") && SceneManager.GetActiveScene() != SceneManager.GetSceneByName("LastScene") && tutorialPage.activeSelf == false)
        {
            if (Input.GetButtonDown("Pause"))
            {
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                pauseMenu.GetComponent<PauseMenu>().MainPause();
                Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
            }
            else if (Input.GetButton("OpenInventory"))
            {
                Time.timeScale = invParent.activeSelf ? 0 : 1;
            }

        }
    }



    /****************************   HANDLES RETURING TO OVERWORLD   **********************************************/


    public void TurnOffScene()
    {
        foreach (GameObject obj in overworldObjects)
        {
            obj.SetActive(false);
        }
    }

    public void TurnOnScene()
    {
        foreach (GameObject obj in overworldObjects)
        {
            obj.SetActive(true);
        }
    }

    private IEnumerator ReturnToOverworld()
    {
        transition = true;
        Debug.Log("return");
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2);

        //Turn off the battle UI
        TurnOffBattleMenus();
        Debug.Log("off");

        yield return new WaitForEndOfFrame();

        TurnOnScene();

        //UnLoad the Battle Scene
        SceneManager.UnloadSceneAsync("Battle-FINAL");

        anim.CrossFade("Fade_In", 1);

        yield return new WaitForEndOfFrame();

        Debug.Log("show");
        //Show any Dialogue
        if (currEnemy.GetComponent<EnemyController>().isBeaten)
        {
            StartCoroutine(SetEnemyDialogue(currEnemy.GetComponent<EnemyController>().postBattleDialogue));

            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => dialogueOver && !dialogueInProgress);
        }

        //Give player movement 
        StartCoroutine(player.PlayerMovement());
        transition = false;
    }



    /****************************   HANDLES INVENTORY AND PAUSE MENUS   **********************************************/


    IEnumerator PauseMenu()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Pause"));
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        StartCoroutine(PauseMenu());
    }

    public void OpenInventory()
    {
        if (invParent.activeSelf)
        {
            invParent.SetActive(false);
            inventory.CloseInventory();
        }
        else
        {
            invParent.SetActive(true);
            inventory.OpenInventory();
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



    /****************************   HANDLES DIALOGUE   **********************************************/


    public IEnumerator SetEnemyDialogue(string[] dia)
    {
        Debug.Log("here");
        //if there's no dialogue to be set
        if (dia.Length <= 0)
        {
            dialogueOver = true;
            dialogueInProgress = false;
            yield break;
        }

        //Set stuff
        dialogueOver = false;
        dialogueInProgress = true;
        diaAnim.SetBool("isOpen", true);
        dialogue.sentences = dia;

        yield return new WaitForEndOfFrame();

        dialogue.StartDialogue();

        yield return new WaitForEndOfFrame();

        StartCoroutine(ShowEnemyDialogue());

    }

    IEnumerator ShowEnemyDialogue()
    {
        yield return new WaitUntil(() => dialogue.isTyping == false);
        dialogue.enterText.SetActive(true);


        yield return new WaitUntil(() => Input.GetButton("SelectAction"));
        dialogue.enterText.SetActive(false);
        yield return new WaitForSecondsRealtime(0.15f);

        //when you press space...
        //When we're at the end of the intro dialogue
        if (_index == dialogue.sentences.Length - 1)
        {
            Debug.Log("end of dialogue");
            //Lower the text box
            diaAnim.SetBool("isOpen", false);

            yield return null;

            _index = 0;
            dialogueOver = true;
            dialogueInProgress = false;
            Debug.Log("end of dialogue");
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



    /****************************   HANDLES GENERAL BATTLE & WIN/LOSS   **********************************************/


    public IEnumerator StartBattle()
    {
        //play some sort of screen wipe
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2);
        TurnOffScene();

        yield return new WaitForFixedUpdate();

        topOverlay.SetActive(false);
        SceneManager.LoadScene("Battle-FINAL", LoadSceneMode.Additive);

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        AudioManager.instance.StartCombatMusic();

        yield return new WaitForSecondsRealtime(2);

        anim.CrossFade("Fade_In", 1);
        yield return new WaitForFixedUpdate();

        //Spawn the correct enemies 
        CombatController.instance.SetUpBattleScene();
    }

    public IEnumerator BattleWon()
    {
        Debug.Log("battle Won");
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

    public IEnumerator BattleLost()
    {
        anim.CrossFade("Fade_Out", 1);

        yield return new WaitForSecondsRealtime(2);
        yield return new WaitForEndOfFrame();

        anim.CrossFade("Fade_In", 1);

        yield return null;

        TurnOffBattleMenus();
        deathScreenParent.SetActive(true);
    }

    public void RetryBattle()
    {
        topOverlay.SetActive(false);
        healthParent.SetActive(true);
        CombatController.instance.ResetBattle();
    }

    public void QuitBattle()
    {
        CombatController.instance.ClearBattle();
        tempWait = true;
        SetGameState(GameState.Overworld);
    }

    void TurnOffBattleMenus()
    {
        Debug.Log("turn off");
        //Clear Battle Stuffs
        CombatController.instance.ClearBattle();

        //Turn off the battle music
        AudioManager.instance.StopCombatMusic();

        //Turn off the battle UI
        deathScreenParent.SetActive(false);
        battleAnimator.SetBool("IsOpen", false);
        healthParent.SetActive(false);
        topOverlay.SetActive(true);
    }



    /****************************   SETTING AND UPDATING GAME STATE   **********************************************/


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
                StartCoroutine(StartBattle());
                break;
            case GameState.AfterTutorial:
                StartCoroutine(AfterTutorial());
                break;
            case GameState.Tutorial:
                StartCoroutine(StartTutorial());
                break;
            case GameState.SkipTutorial:
                StartCoroutine(SkipTutorial());
                break;
            default:
                Debug.LogError("Something has gone wrong in GameState Update loop");
                break;
        }
    }



    /****************************   HANDLES LOADING AND TUTORIAL   **********************************************/


    IEnumerator StartTutorial()
    {
        anim.gameObject.SetActive(true);
        anim.CrossFade("Fade_In", 1);

        //Not sure why we wait here but I will not temp God
        yield return new WaitForSecondsRealtime(1f);

        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2);
        tutorialPage.SetActive(false);
        anim.CrossFade("Fade_In", 1);

        //Not sure why we wait here but I will not temp God
        yield return new WaitForSecondsRealtime(1f);

        //open the text box and start dialogue
        diaAnim.SetBool("isOpen", true);

        //set thesentences in the dialogue manager
        dialogue.sentences = tutorial.beforeBattleDialogue;

        dialogue.StartDialogue();

        //start the dialogue in the tutorial script
        StartCoroutine(tutorial.ShowOpeningDialogue());
    }

    IEnumerator SkipTutorial()
    {
        anim.gameObject.SetActive(true);
        anim.CrossFade("Fade_In", 1);

        //Not sure why we wait here but I will not temp God
        yield return new WaitForSecondsRealtime(1f);

        yield return new WaitUntil(() => tutorialPage.GetComponent<TutorialMenu>().turnoff);

        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2);
        tutorialPage.SetActive(false);
        anim.CrossFade("Fade_In", 1);

        //Not sure why we wait here but I will not temp God
        yield return new WaitForSecondsRealtime(1f);

        overworldObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        StartCoroutine(SetEnemyDialogue(levelOneDialogue));

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !dialogueInProgress && dialogueOver);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(player.PlayerMovement());
    }

    private IEnumerator AfterTutorial()
    {
        //play some sort of screen wipe
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2);

        //Turn off the battle music
        AudioManager.instance.StopCombatMusic();

        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene("Overworld_Level1-FINAL");

        yield return new WaitForEndOfFrame();

        anim.CrossFade("Fade_In", 1);

        yield return new WaitForSecondsRealtime(1);

        StartCoroutine(SetEnemyDialogue(levelOneDialogue));

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !dialogueInProgress && dialogueOver);

        overworldObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(player.PlayerMovement());
    }

    //TODO: AT SOME POINT MAKE LOADING ONE FUNCTION INSTEAD OF A BUNCHA SEPERATE ONES

    public IEnumerator LoadLevelTwo()
    {
        hasKey = false;
        level = 2;

        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene("Overworld_Level2-FINAL");

        anim.CrossFade("Fade_In", 1);

        yield return new WaitForSecondsRealtime(1);

        AudioManager.instance.bgMusic.clip = AudioManager.instance.bgClips[level];
        AudioManager.instance.bgMusic.Play();

        StartCoroutine(SetEnemyDialogue(levelTwoDialogue));

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !dialogueInProgress && dialogueOver);

        overworldObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(player.PlayerMovement());
    }

    public IEnumerator LoadBreakRoom()
    {
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene("Overworld_BreakRoom_FINAL");

        anim.CrossFade("Fade_In", 1);

        yield return new WaitForSecondsRealtime(1);


        overworldObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        StartCoroutine(player.PlayerMovement());
    }

    public IEnumerator PlaySusanVideo()
    {
        anim.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2f);
        susanVideo.SetActive(true);
        anim.CrossFade("Fade_In", 1);
        yield return new WaitForSecondsRealtime(6f);
        yield return new WaitForEndOfFrame();
    }

}
