using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVPlotterFirst : MonoBehaviour {

    public string filename;

    public float[] pose = new float[169];
    GameObject sub;
    private string data_String;
    private StreamReader strReader;
    
    private void Start() 
        {    
        sub = new GameObject();

        strReader = new StreamReader(filename);

        // header
        data_String = strReader.ReadLine();
        data_String = strReader.ReadLine();
        var data_values = data_String.Split(',');

        pose = Array.ConvertAll(data_values, s => float.Parse(s));
        
    }
}