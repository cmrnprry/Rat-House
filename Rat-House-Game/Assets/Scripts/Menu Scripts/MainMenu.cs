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
        SceneManager.LoadScene("Tutorial-FINAL");
    }

    public void SkipTutorial()
    {
        SceneManager.LoadScene("Overworld_Level1-FINAL");
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
