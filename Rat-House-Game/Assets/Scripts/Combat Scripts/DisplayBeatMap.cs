using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBeatMap : MonoBehaviour
{
    public List<List<int>> map;
    private bool _display = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CombatController.instance.inBattle)
        {

        }
    }
}
