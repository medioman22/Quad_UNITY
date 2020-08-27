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
    
    public float corr_roll;
    public float corr_pitch;
    public float corr_yaw;

    public string SubjectFolder = "";

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
        // string command = "python /Users/lis/Documents/github/HRI_mapping/src/avatar/avatar_communication.py " + SubjectFolder;

        // System.Diagnostics.Process otherProcess = new System.Diagnostics.Process();
        string command = "python /Users/lis/Documents/github/HRI_mapping/src/avatar/avatar_communication.py";// + SubjectFolder;
        try
        {
            UnityEngine.Debug.Log("============== Start Executing [" + command + "] ===============");
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = command,
                UseShellExecute = true,
                CreateNoWindow = false,
                // Arguments = SubjectFolder
            };
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process
            {
                StartInfo = startInfo
            };
            myProcess.Start();
            // myProcess.WaitForExit();
            UnityEngine.Debug.Log("============== End ===============");;
        }
        catch (Exception e)
        {
            print(e);
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
            corr = Quaternion.Euler(corr_roll, corr_pitch, corr_yaw);
            //Quaternion(0, 1, 0, Mathf.Cos(Mathf.PI/4));

            Quaternion torso_rot = Quaternion.identity;
            Quaternion arm_rot = Quaternion.identity;

            for (int i = 0; i < rb.Length; i++)
            {

                int torso = 8 * i;

                if (udp.avatar[3] == 0 && udp.avatar[4] == 0 && udp.avatar[5] == 0 && udp.avatar[6] == 0)
                //print("waiting for stable skeleton data");
                {
                    var a = 1;
                }
                else
                {
                    // right to left handed pos
                    rb[i].position = new Vector3(-udp.avatar[torso + 1], udp.avatar[torso + 2], udp.avatar[torso + 3]);
                    // CORRECTION FOR Z-UP IN MOCAP
                    // rb[i].position = new Vector3(-udp.avatar[torso + 1], udp.avatar[torso + 3], -udp.avatar[torso + 2]);


                        // right to left handed quaternion
                        Quaternion rot = new Quaternion(-udp.avatar[torso + 4], udp.avatar[torso + 5], udp.avatar[torso + 6], -udp.avatar[torso + 7]);
                        var fin_rot = Quaternion.identity;
                        fin_rot = rot * corr;

                    if (DebugMode)
                    {

                        Debug.Log(" ");
                        Debug.Log("----- START PlayerController DEBUG -----");
                        Debug.Log(" ");

                        Debug.Log(i);
                        Debug.Log(" ");
                        Debug.Log(qr[i].magnitude);
                        Debug.Log("pos = " + udp.avatar[torso + 1] +
                              " " + udp.avatar[torso + 2] +
                              " " + udp.avatar[torso + 3]);

                        Debug.Log("rot quat = " + rb[i].rotation.x +
                              " " + rb[i].rotation.y +
                              " " + rb[i].rotation.z +
                              " " + rb[i].rotation.w);

                        print(" ");
                        Debug.Log("n = " + udp.avatar[udp.avatar.Length - 1]);
                    }

                    try
                    {
                        rb[i].rotation = fin_rot.normalized;
                    }
                        catch (Exception e)
                    {
                        //Debug.LogException(e, this);
                       print("waiting for stable skeleton data");
                    }

                    if (i == 2)
                    {
                        torso_rot = fin_rot.normalized;
                    }
                    if (i == 6)
                    {
                        arm_rot = fin_rot.normalized;
                    }
                }
            }
            // Debug.Log(arm_rot.ToEulerAngles() * 60 );
            Debug.Log("TORSO ROTATION (Q)= " + torso_rot);
            Debug.Log("TORSO ROTATION = " + torso_rot.ToEulerAngles() * 180 / Mathf.PI);
            // Debug.Log(arm_rot.ToEulerAngles() * 60 - torso_rot.ToEulerAngles() * 60);
        }

        // Debug.Log(time_diff);
    }

    // Update is called once per frame
    void Update()
    {
    }

}