using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class BeatMapReader : MonoBehaviour
{
    public struct BeatMapStruct
    {
        public BeatMapStruct(string at, List<float> beats)
        {
            attack_type = at;
            beatsToHit = beats;
        }
        public string attack_type { get; }
        public List<float> beatsToHit { get; }

    }

    // Start is called before the first frame update
    void Start()
    {
        ReadTSVFile();
    }

    public void ReadTSVFile()
    {
        List<BeatMapStruct> maps = new List<BeatMapStruct>();
        List<string[]> tempList = new List<string[]>();
        StreamReader reader = new StreamReader("./Assets/Beatmaps/BeatMaps.tsv");

        string line;

        // Read and display lines from the file until the end of 
        // the file is reached.
        while ((line = reader.ReadLine()) != null)
        {
            string[] data = line.Split('\t');

            if (data[0] != "Attack_Type")
                maps.Add(ToStruct(data));
        }

        AudioManager.instance.SetBeatMaps(maps);
    }

    BeatMapStruct ToStruct(string[] data)
    {
        string name = data[0];
        List<float> list = new List<float>();

        //Debug.Log("Name: " + name);

        for (int i = 1; i < data.Length; i++)
        {
            var x = Int32.Parse(data[i]);
           // Debug.Log("Beat: " + x);

            list.Add(x);
        }


        return new BeatMapStruct(name, list);
    }
}
