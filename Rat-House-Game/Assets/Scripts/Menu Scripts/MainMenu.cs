using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject credits;
    public GameObject mainmenu;
    public GameObject StartWithTutorial;
    public Animator transition;

    public void StartGame()
    {
        credits.SetActive(false);
        mainmenu.SetActive(false);
        StartWithTutorial.SetActive(true);
    }

    public void ReturnToMenu()
    {
        credits.SetActive(false);
        mainmenu.SetActive(true);
        StartWithTutorial.SetActive(false);
    }

    public void StartTutorial()
    {
        GameManager.instance.SetGameState(GameState.Tutorial);
        StartCoroutine(Transition(true));
    }

    public void SkipTutorial()
    {
        StartCoroutine(Transition(false));
    }

    //true for no skip
    IEnumerator Transition(bool skip)
    {
        GameManager.instance.anim.CrossFade("Fade_Out", 1);
        transition.CrossFade("Fade_Out", 1);
        yield return new WaitForSecondsRealtime(2);
        yield return new WaitForFixedUpdate();
        transition.CrossFade("Fade_In", 1);
        GameManager.instance.anim.CrossFade("Fade_In", 1);

        if (skip)
        {
            SceneManager.LoadScene("Tutorial-FINAL");
        }
        else
        {
            GameManager.instance.SetGameState(GameState.SkipTutorial);
            SceneManager.LoadScene("Overworld_Level1-FINAL");
        }

        yield return new WaitForFixedUpdate();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Credits()
    {
        credits.SetActive(true);
        mainmenu.SetActive(false);
        StartWithTutorial.SetActive(false);
    }
}
