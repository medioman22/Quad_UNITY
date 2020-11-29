using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVPlotter : MonoBehaviour {
  
    public string filename;
    public int first = 0;
    public int last = 10000;
    public int downs = 100;
    public string name = "test";

    public float[] pose = new float[169];
    public List<float> pose_list = new List<float>();
    public List<float> euler = new List<float>();
    GameObject sub;

    public int count = 0;
    public int count_downs = 0;
    private string data_String;
    private StreamReader strReader;
    private bool endOfFile;
    private void Start() 
        {    
        sub = new GameObject();

        strReader = new StreamReader(filename);
        endOfFile = false;
        count = 0;

        // header
        data_String = strReader.ReadLine();
        
    }

    void spawn()
    {
        // Instantiate(sub, Vector3.zero, Quaternion.identity);
        sub.name = name;
    }

    private void Update() {
        ReadCSVFile();
    }

    void ReadCSVFile()
    {
        count += 1;

        for (int i = 0; i < downs; i++)
        {
            data_String = strReader.ReadLine();
        }
        if (data_String == null)
        {
            endOfFile = true;
            strReader = new StreamReader(filename);
            count = 0;
            data_String = strReader.ReadLine();
            data_String = strReader.ReadLine();
        }

        var data_values = data_String.Split(',');


        if (count > first && count < last)
        {
            pose = Array.ConvertAll(data_values, s => float.Parse(s));

            pose_list.AddRange(pose);

            euler = pose_list.GetRange(pose_list.Count - 30, 27);
        }
    }
}