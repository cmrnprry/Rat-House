using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public enum GameState
{
    Overworld = 0,
    Battle = 1,
    Boss = 2,
    CutScene = 3,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Keeps track of the current game state
    private GameState _currState = GameState.Overworld;

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

    public IEnumerator BattleWon()
    {
        //Play win music if any

        //Play win anim if any

        //Show new attack gained if any

        //Set Game State
        GameManager.instance.SetGameState(GameState.Overworld);
        AudioManager.instance.StopCombatMusic();

        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene(0);

        yield return new WaitForEndOfFrame();
        
        //Show any Dialogue
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
