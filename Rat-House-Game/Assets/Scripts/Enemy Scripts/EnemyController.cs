﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // Different Types of Potential Enemies
public enum EnemyType
{
    Coffee = 0,
    Intern = 1,
    Water_Cooler = 2,
}

public class EnemyController : MonoBehaviour
{
    public List<EnemyType> enemiesInBattle;


    /*
     PLANNED CODE:
        - This script will be attached to every fightable enemy
        - THis script will hold all the battle data for a fight which includes:
            - Any dialogue before a fight
            - The amount of enemies in the given enconuter
            - The type of enemies in the given encounter
            - Should Probably rename
         
         
         */

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
