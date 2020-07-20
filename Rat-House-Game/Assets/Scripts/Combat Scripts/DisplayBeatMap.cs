using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBeatMap : MonoBehaviour
{
    private List<float> map = new List<float>();
    private bool _display = false;


    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        //if (CombatController.instance.inBattle)
        //{
        //    switch (CombatController.instance.selectedAction)
        //    {
        //        case ActionType.Basic_Attack:
        //            map = AudioManager.instance.beatMap[0].beatsToHit;
        //            DisplayBasicAttack();
        //            break;
        //        case ActionType.Item:
        //            Debug.Log("Open Item Menu");
        //            break;
        //        default:
        //            Debug.LogError("Something has gone wrong in Combat Controller");
        //            break;
        //    }
        //}
    }


    void DisplayBasicAttack()
    {
        var rowOne = map.GetRange(0, 8);
        var rowTwo = map.GetRange(8, 8);


    }
}
