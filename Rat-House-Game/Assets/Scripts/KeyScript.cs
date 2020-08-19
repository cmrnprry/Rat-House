using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyScript : MonoBehaviour
{
    //public Animator itemGetAnim;

    //public TextMeshProUGUI itemText;

    //public ElevatorScript elevatorScript;

    // Start is called before the first frame update
    void Start()
    {
        //elevatorScript = GetComponent<ElevatorScript>();
    }

    void OnTriggerEnter(Collider other)
    {
        GameManager.instance.hasKey = true;

        GameManager.instance.itemText.text = "Elevator Keycard has been added to your record";
        GameManager.instance.itemGetAnim.SetTrigger("textPopup");

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
