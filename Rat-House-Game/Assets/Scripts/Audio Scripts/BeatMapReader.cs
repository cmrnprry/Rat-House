using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public struct BeatMapStruct
{
    public BeatMapStruct(string at, int d, List<float> beats)
    {
        action_type = at;
        base_damage = d;
        beatsToHit = beats;
    }
    public string action_type { get; }
    public int base_damage { get; }
    public List<float> beatsToHit { get; }

}
public class BeatMapReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ReadTSVFile();
    }

    public void ReadTSVFile()
    {
        List<BeatMapStruct> p_maps = new List<BeatMapStruct>();
        List<BeatMapStruct> e_maps = new List<BeatMapStruct>();

        //var path = Application.dataPath + "/Resources/BeatMaps/BeatMaps.tsv";
        TextAsset playerActions = Resources.Load("BeatMaps/PlayerBeatMaps") as TextAsset;
        TextAsset enemyActions = Resources.Load("BeatMaps/EnemyBeatMaps") as TextAsset;

        //StreamReader reader = new StreamReader(path.text);

        string[] p_data = playerActions.text.Split(',');
        foreach (string str in p_data)
        {
            string[] action = str.Split('\t');

            p_maps.Add(ToStruct(action));
        }


        string[] e_data = enemyActions.text.Split(',');
        foreach (string str in e_data)
        {
            string[] action = str.Split('\t');

            e_maps.Add(ToStruct(action));
        }


        AudioManager.instance.SetBeatMaps(p_maps, e_maps);
        CombatController.instance.SetBasePlayerDamage(p_maps);
    }

    BeatMapStruct ToStruct(string[] data)
    {
        string name = data[0];
        int dmg = Int32.Parse(data[1]);
        List<float> list = new List<float>();

        for (int i = 2; i < data.Length; i++)
        {
            if (data[i] != "X")
            {
                float x = float.Parse(data[i]);
                list.Add(x);
            }
        }

        return new BeatMapStruct(name, dmg, list);
    }
}
