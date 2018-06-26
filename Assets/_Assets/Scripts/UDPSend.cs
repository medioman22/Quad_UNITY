using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{

    private static int localPort;

    // prefs
    private string IP;  // define in init
    public int port;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    string strMessage = "";


    // call it from shell (as program)
    private static void Main()
    {
        UDPSend sendObj = new UDPSend();
        sendObj.init();

        // testing via console
        // sendObj.inputFromConsole();

        // as server sending endless
        sendObj.sendEndless(" endless infos \n");

    }
    // start from unity3d
    public void Start()
    {
        init();
    }

    // OnGUI
    void OnGUI()
    {
        Rect rectObj = new Rect(40, 380, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        GUI.Box(rectObj, "# UDPSend-Data\n127.0.0.1 " + port + " #\n"
                + "shell> nc -lu 127.0.0.1  " + port + " \n"
                , style);

        // ------------------------
        // send it
        // ------------------------
        strMessage = GUI.TextField(new Rect(40, 420, 140, 20), strMessage);
        if (GUI.Button(new Rect(190, 420, 40, 20), "send"))
        {
            sendString(strMessage + "\n");
        }
    }

    // init
    public void init()
    {
        // Define end point , from which the messages are sent.
        print("UDPSend.init()");

        // define
        IP = "127.0.0.1";
        port = 30000;

        // ----------------------------
        // Send
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(26000);

        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);


        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }

    // inputFromConsole
    private void inputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();

                // Send the text to the remote client .
                if (text != "")
                {

                    // encode data with the UTF8 encoding to binary .
                    byte[] data = Encoding.UTF8.GetBytes(text);

                    // Send the text to the remote client .
                    client.Send(data, data.Length, remoteEndPoint);
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    public void sendString(string message)
    {
        try
        {
            //if (message != "")
            //{

            // encode data with the UTF8 encoding to binary .
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Send the message to the remote client .
            client.Send(data, data.Length, remoteEndPoint);
            //}
            print(data.Length);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    // endless test
    private void sendEndless(string testStr)
    {
        do
        {
            sendString(testStr);


        }
        while (true);

    }

    public float[] receiveString()
    {
        var data = client.Receive(ref remoteEndPoint); // listen on port XXXXX

        //var mess = System.BitConverter.ToSingle(data, 0);

        print(data.Length);

        var mess = new float[data.Length / 4];
        Buffer.BlockCopy(data, 0, mess, 0, data.Length);

        //return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        return mess;
    }

    public float[] receiveAvatar()
    {
        var data = client.Receive(ref remoteEndPoint); // listen on port XXXXX

        //var mess = System.BitConverter.ToSingle(data, 0);

        //print(data.Length);

        var mess = new float[data.Length / 4];
        Buffer.BlockCopy(data, 0, mess, 0, data.Length);

        //return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        return mess;
    }

}