using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class UDPCommunication : MonoBehaviour
{
    public string message = "0";

    private UDPSend udp;

    Int32 count = 0;
    public float[] controls = new float[4];

    float time_start = 0.0f;
    float time_diff = 0.0f;
    float time_new = 0.0f;

    // Use this for initialization
    void Start()
    {
        time_start = Time.realtimeSinceStartup;

        time_diff = 0.0f;

        if ((udp == null) && (GetComponent<UDPSend>() != null))
        {
            udp = GetComponent<UDPSend>();
        }
        else
        {
            Debug.LogWarning("Missing UDPSend component. Please add one");
        }


        //run_cmd();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.DataFromUDP)
        {
            time_new = Time.realtimeSinceStartup;

            time_diff = time_new - time_start;

            print(time_diff);

            time_start = time_new;

            // Send data

            udp.sendString(message);
            //print("sent " + "\"" + message + "\"");// + " to " + IP + " : " + port);

            // Receive data

            controls[0] = 0.0f;
            controls[1] = 0.0f;
            controls[2] = 0.0f;
            controls[3] = 0.0f;
            controls = udp.receiveString();

            //print("received \"" + controls + "\" from ");// + remoteEndPoint.ToString());

            //foreach (var i in str)
            //{
            //    Debug.Log(i);
            //}
            //print("");

            count++;
            //print(count);
        }
    }

    private void run_cmd()
    {

        string fileName = @"C:/Users/matteoPC/Documents/GitHub/Python_acquisition/Acq_test.py";

        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\Python27\python.exe", fileName)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        p.Start();

        print("just started");

        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        Console.WriteLine(output);

        Console.ReadLine();

    }
}