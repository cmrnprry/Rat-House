using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorScript : MonoBehaviour
{
    public Animator doorsAnim;
    public Animator lightsAnim;

    //public bool hasKey = false;

    //void OnEnable()
    //{
    //    hasKey = false;
    //}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && GameManager.instance.hasKey == true)
        {
            doorsAnim.SetBool("HasKey", true);
            lightsAnim.SetBool("HasKey", true);
            if (SceneManager.GetActiveScene().name == "Overworld_Level1-FINAL")
            {
                //hasKey = false;
                GameManager.instance.StartCoroutine(GameManager.instance.LoadLevelTwo());
                
            }
            else if (SceneManager.GetActiveScene().name == "Overworld_Level2-FINAL")
            {
                //hasKey = false;
                GameManager.instance.StartCoroutine(GameManager.instance.LoadBreakRoom());
                
            }
        }
    }
}
