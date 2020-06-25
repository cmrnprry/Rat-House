using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{



    //FOR PROTOTYPE
    public SpriteRenderer bg;
    public GameObject speech1;
    public GameObject speech2;
    public GameObject speech3;
    public GameObject bar;
    public GameObject angry;
    public GameObject b1;
    public GameObject i;
    public GameObject r;
    public GameObject button;
    public GameObject b2;
    public GameObject spnoge;
    public SpriteRenderer spnogeCoffee;
    public TextMeshProUGUI spnogeText;
    public Sprite bg2;
    public Sprite good;
    public Sprite bad;
    public static GameManager instance;

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
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
        ShowTutorial();
        StartCoroutine(AudioManager.instance.WaitToStart());
    }

    public void ShowTutorial()
    {
        i.SetActive(true);
        r.SetActive(true);
    }

    public void End()
    {
        spnoge.SetActive(true);
        button.SetActive(false);
        spnogeCoffee.gameObject.SetActive(true);
        speech2.SetActive(true);
        speech3.SetActive(true);

        if (ButtonController.instance.score >= 1500)
        {
            //good
            spnogeText.text = "You made my coffe so good thanks.";
            spnogeCoffee.sprite = good;
        }
        else
        {
            spnogeText.text = "I would like to speak to your manager.";
            spnogeCoffee.sprite = bad;
            angry.SetActive(true);
        }

    }

}
