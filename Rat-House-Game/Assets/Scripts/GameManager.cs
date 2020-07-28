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
    Dead = 4,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Items the Player currently has
    //public List<Items> itemList = new List<Items>();

    //Keeps track of the current game state
    private GameState _currState = GameState.Overworld;

    public GameObject _deathScreen;

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
        Debug.Log("Load Battle Scene");
        SceneManager.LoadScene(1);

        yield return new WaitForFixedUpdate();

        //Spawn the correct enemies 
        CombatController.instance.SetUpBattleScene();

        Debug.Log("Start Battle");

        AudioManager.instance.StartCombatMusic();
        StartCoroutine(CombatController.instance.ChooseAction());
    }

    private IEnumerator ReturnToOverworld()
    {
        AudioManager.instance.StopCombatMusic();
        CombatController.instance.ClearBattle();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene(0);

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

        //  SceneManager.LoadScene("Joe Is Dead-FINAL");
        _deathScreen.SetActive(true);


        //Set Buttons
        var buttons = GameObject.FindGameObjectsWithTag("Button");
        var retry = buttons[0].GetComponent<Button>();
        var quit = buttons[1].GetComponent<Button>();

        //Set buttons on Click
        retry.onClick.AddListener(RetryBattle);
        quit.onClick.AddListener(QuitBattle);
    }

    //Retry the current battle
    public void RetryBattle()
    {
        CombatController.instance.ResetBattle();
        //StartCoroutine(StartBattle());
    }

    //Retrun to the overworld
    public void QuitBattle()
    {
        _currState = GameState.Overworld;
        UpdateGameState();
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
        GameManager.instance.SetGameState(GameState.Overworld);
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
}
