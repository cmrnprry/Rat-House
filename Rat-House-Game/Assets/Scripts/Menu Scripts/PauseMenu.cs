﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseParent;
    public GameObject optionsMenu;
    public GameObject checkBox;

    public Slider overall;
    public Slider bg;
    public Slider sfx;

    public void ExitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Back()
    {
        Time.timeScale = 1;
        GameManager.instance.pauseMenu.SetActive(false);
        pauseParent.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void MainPause()
    {
        pauseParent.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        pauseParent.SetActive(false);
    }

    public void Metronome()
    {
        checkBox.SetActive(!checkBox.activeSelf);
        AudioManager.instance.bgClips[0] = checkBox.activeSelf ? AudioManager.instance.metronome[0] : AudioManager.instance.metronome[1];
    }

    public void OverallChange()
    {
        AudioListener.volume = overall.value;
    }

    public void BGChange()
    {
        AudioManager.instance.bgMusic.volume = bg.value;
    }

    public void SFXChange()
    {
        AudioManager.instance.SFX.volume = sfx.value;
    }
}
