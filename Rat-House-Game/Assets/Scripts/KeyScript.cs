using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GameManager.instance.hasKey = true;

            GameManager.instance.itemText.text = "Elevator Keycard has been added to your record";
            GameManager.instance.itemGetAnim.SetTrigger("textPopup");

            this.gameObject.SetActive(false);
        }        
    }
}
