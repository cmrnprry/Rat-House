using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // Different Types of Potential Enemies
public enum EnemyType
{
    NULL = -2,
    Tutorial_Intern = -1,
    Coffee = 0,
    Intern = 1,
    Water_Cooler = 2,
    Computer_Man = 3,
}

public class EnemyController : MonoBehaviour
{
    [Header("Enemies to Appear in an Encounter")]
    public List<EnemyType> enemiesInBattle;

    [TextArea(3, 5)]
    public string[] preBattleDialogue;

    [TextArea(3, 5)]
    public string[] postBattleDialogue;

    [TextArea(3, 5)]
    public string[] beatenBattleDialogue;

    public bool isBeaten = false;


    /*
     PLANNED CODE:
        - This script will be attached to every fightable enemy
        - THis script will hold all the battle data for a fight which includes:
            - Any dialogue before a fight
            - The amount of enemies in the given enconuter
            - The type of enemies in the given encounter
            - Should Probably rename
         
         
         */

}
