using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
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
