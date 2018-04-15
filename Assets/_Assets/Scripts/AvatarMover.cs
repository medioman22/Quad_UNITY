using System;
using UnityEngine;
using System.Collections;
using System.Linq;



public class AvatarMover : MonoBehaviour
{
    public bool DebugMode = false;

    private UDPCommunication udp;

    private Rigidbody rb;
    //private 

    public static bool DataFromUDP = true;

    private float time_start = 0.0f;
    public float time_diff = 0.0f;
    private float time_new = 0.0f;
    private float time_new_start = 0.0f;

    // Use this for initialization
    void Start()
    {
        time_start = Time.realtimeSinceStartup;
        time_new_start = Time.realtimeSinceStartup;

        time_diff = 0.0f;

        rb = GetComponent<Rigidbody>();

        // Connect UDP

        if (DataFromUDP)
        {
            if ((udp == null) && (GameObject.Find("GameController").gameObject.GetComponent<UDPCommunication>() != null))
            {
                udp = GameObject.Find("GameController").gameObject.GetComponent<UDPCommunication>();
            }
            else
            {
                Debug.LogWarning("Missing UDPSend component. Please add one");
            }
        }
    }

    // FixedUpdate is called right before Update
    void FixedUpdate()
    {
        time_new = Time.realtimeSinceStartup;

        time_diff = time_new - time_new_start;

        time_new_start = Time.realtimeSinceStartup;

        if (DataFromUDP)
        {
            Debug.LogWarning("UDP?");
            var avatar = udp.avatar;

            Rigidbody[] rb = gameObject.GetComponentsInChildren<Rigidbody>();

            Vector3[] qr = new Vector3[rb.Length];
            Quaternion corr;
            corr = Quaternion.Euler(0, 90, -180);
            //Quaternion(0, 1, 0, Mathf.Cos(Mathf.PI/4));

            for (int i = 1; i < rb.Length; i++)
            {

                int torso = 8 * i;

                if (udp.avatar[3] == 0 && udp.avatar[4] == 0 && udp.avatar[5] == 0 && udp.avatar[6] == 0)
                    print("waiting for stable skeleton data");
                else
                {
                    rb[i].position = new Vector3(udp.avatar[torso + 1], udp.avatar[torso + 2], udp.avatar[torso + 3]);

                    qr[i] = Vector3.Normalize(new Vector3(udp.avatar[torso + 4], udp.avatar[torso + 5], udp.avatar[torso + 6]));


                    try
                    {
                        Quaternion rot = new Quaternion(qr[i].x, qr[i].y, qr[i].z, udp.avatar[7]);
                        rb[i].rotation = corr * rot;
                    }
                    catch (Exception e)
                    {
                        //Debug.LogException(e, this);
                        print("waiting for stable skeleton data");
                    }

                    if (DebugMode && i == 2)
                    {
                        var pos = transform.position;

                        Debug.Log(" ");
                        Debug.Log("----- START PlayerController DEBUG -----");
                        Debug.Log(" ");

                        Debug.Log("pos = " + udp.avatar[torso + 1] +
                              " " + udp.avatar[torso + 2] +
                              " " + udp.avatar[torso + 3]);

                        Debug.Log("rot quat = " + rb[i].rotation.x +
                              " " + rb[i].rotation.y +
                              " " + rb[i].rotation.z +
                              " " + rb[i].rotation.w);

                        //print(" ");
                        Debug.Log("n = " + udp.avatar[udp.avatar.Length - 1]);
                    }

                }
            }
        }


        Debug.Log(time_diff);
    }

    // Update is called once per frame
    void Update()
    {
    }

}