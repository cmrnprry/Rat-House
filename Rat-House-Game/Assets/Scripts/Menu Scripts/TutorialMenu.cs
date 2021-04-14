using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenu : MonoBehaviour
{

    public TextMeshProUGUI next;
    public GameObject back;

    public GameObject[] pages;
    public bool turnoff = false;
    private int index = 0;

    public void Next()
    {
        back.SetActive(true);

        if (index + 1 < pages.Length - 1)
        {
            next.text = "Next";
            pages[index].SetActive(false);
            pages[index + 1].SetActive(true);
            
        }
        else if (index + 1 == pages.Length - 1)
        {
            next.text = "Continue";
            pages[index].SetActive(false);
            pages[index + 1].SetActive(true);
        }
        else
        {
            turnoff = true;
        }

        index = (index + 1 < pages.Length) ? index + 1 : index;
        
    }

    public void Back()
    {
        next.text = "Next";

        if (index - 1 > 0)
        {
            back.SetActive(true);
            pages[index].SetActive(false);
            pages[index - 1].SetActive(true);

        }
        else if (index - 1 == 0)
        {
            back.SetActive(false);
            pages[index].SetActive(false);
            pages[index - 1].SetActive(true);
        }

        index = (index - 1 >= 0) ? index - 1 : index;
    }
}
