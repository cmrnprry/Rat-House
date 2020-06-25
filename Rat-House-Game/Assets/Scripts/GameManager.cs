using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{



    //FOR PROTOTYPE
    public SpriteRenderer bg;
    public GameObject speech1;
    public GameObject speech2;
    public GameObject speech3;
    public GameObject bar;
    public GameObject b1;
    public GameObject button;
    public GameObject b2;
    public GameObject spnoge;
    public Sprite bg2;
    public void Next()
    {
        Debug.Log("here");
        bg.sprite = bg2;

        speech1.SetActive(false);
        b1.SetActive(false);
        bar.SetActive(true);
        b2.SetActive(true);
        speech2.SetActive(true);
        speech3.SetActive(true);
    }

    public void StartNew()
    {
        Debug.Log("here");

        b2.SetActive(false);
        spnoge.SetActive(false);
        speech2.SetActive(false);
        button.SetActive(true);
        speech3.SetActive(false);
        AudioManager.instance.text.gameObject.SetActive(true);
        StartCoroutine(AudioManager.instance.WaitToStart());
    }
}
