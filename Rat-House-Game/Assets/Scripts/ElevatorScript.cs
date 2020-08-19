using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorScript : MonoBehaviour
{
    public Animator doorsAnim;
    public Animator lightsAnim;

    public bool hasKey = false;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && hasKey == true)
        {
            doorsAnim.SetBool("HasKey", true);
            lightsAnim.SetBool("HasKey", true);
            if (SceneManager.GetActiveScene().name == "Overworld_Level1-FINAL")
            {
                GameManager.instance.StartCoroutine(GameManager.instance.LoadLevelTwo());
            }
            else if (SceneManager.GetActiveScene().name == "Overworld_Level2-FINAL")
            {
                GameManager.instance.StartCoroutine(GameManager.instance.LoadBreakRoom());
            }
        }
    }
}
