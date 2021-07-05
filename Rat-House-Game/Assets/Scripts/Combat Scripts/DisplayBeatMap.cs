using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBeatMap : MonoBehaviour
{
    private List<float> map = new List<float>();
    private bool _display = false;

    void DisplayBasicAttack()
    {
        var rowOne = map.GetRange(0, 8);
        var rowTwo = map.GetRange(8, 8);


    }
}
