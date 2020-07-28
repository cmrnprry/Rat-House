using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    //public string playerName;
    public string enemyName;
    [TextArea(3, 7)]
    public string[] sentences;
}