using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVClass : MonoBehaviour {

    public CSVPlotter csv;
    public string foldername;
    public string filename;

    private int first = 0;
    private int last = 10000;
    private int downs = 100;

    public float[] pose = new float[1];
    public List<float> pose_list = new List<float>();
    public List<float> euler = new List<float>();
    public int count = 0;
    private string data_String;
    private StreamReader strReader;
    private bool endOfFile;

    private void Start()
    {
        first = csv.first;
        last = csv.last;
        downs = csv.downs;

        strReader = new StreamReader(System.IO.Path.Combine(foldername, filename));
        endOfFile = false;
        count = 0;

        // header
        data_String = strReader.ReadLine();
        
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
            strReader = new StreamReader(System.IO.Path.Combine(foldername, filename));
            count = 0;
            data_String = strReader.ReadLine();
            data_String = strReader.ReadLine();
        }

        var data_values = data_String.Split(',');


        if (count > first && count < last)
        {
            pose = Array.ConvertAll(data_values, s => float.Parse(s));

            pose_list.AddRange(pose);
        }
    }
}