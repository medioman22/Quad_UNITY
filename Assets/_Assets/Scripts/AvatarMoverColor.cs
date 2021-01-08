using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;


public class AvatarMoverColor : MonoBehaviour
{
    public Material Neutral;
    public Material Torso;
    public Material SteeringWheel;
    public Material RightHand;
    public Material LeftHand;

    public GameObject TorsoGO;
    public GameObject LeftFAGO;
    public GameObject RightFAGO;

    public int SubjectNumber = 1;
    public bool ResetPosition = false;
    public Vector3 ZeroPosition = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3 pos_diff = new Vector3(0.0f, 1.0f, 0.0f);

    public bool HeadStraight = false;

    public Vector3 neckDist = new Vector3(0.0f,0.23f,0.0f);
    public Vector3 headDist = new Vector3(0.0f,0.13f,0.0f);
    
    public bool HandsStraight = false;

    public Vector3 handDist = new Vector3(0.25f,0.0f,0.0f);

    private int gap = 0;

    public bool DebugMode = false;

    private CSVPlotter csv;
    private CSVClass csvcl;

    private Rigidbody rb;
    //private 

    private float time_start = 0.0f;
    public float time_diff = 0.0f;
    private float time_new = 0.0f;
    private float time_new_start = 0.0f;
    
    public float corr_roll;
    public float corr_pitch;
    public float corr_yaw;

    public string SubjectFolder = "";

    private int count = 0;


    Process process = null;
    StreamWriter messageStream;

    // [MenuItem("MyGame/Downscale Reference Textures")]
    public static void DownscaleRefTextures()
    {
        try
        {
            // using System.Diagnostics;
            Process p = new Process();
            p.StartInfo.FileName = "python";
            p.StartInfo.Arguments = "avatar_communication.py";
            // Pipe the output to itself - we will catch this later
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            // Where the script lives
            p.StartInfo.WorkingDirectory = "/Users/lis/Documents/github/HRI_mapping/src/avatar/";
            p.StartInfo.UseShellExecute = false;

            p.Start();
            // Read the output - this will show is a single entry in the console - you could get  fancy and make it log for each line - but thats not why we're here
            UnityEngine.Debug.Log(p.StandardOutput.ReadToEnd());
            // p.WaitForExit();
            // p.Close();
        }
        catch (Exception e)
        {
            //UnityEngine.Debug.LogException(e, this);
            UnityEngine.Debug.Log(e);
        }
    }

    void Start()
    {
        
        time_start = Time.realtimeSinceStartup;
        time_new_start = Time.realtimeSinceStartup;

        time_diff = 0.0f;

        rb = GetComponent<Rigidbody>();

        // System.Diagnostics.Process otherProcess = new System.Diagnostics.Process();
        DownscaleRefTextures();

        csv = gameObject.GetComponent<CSVPlotter>();
        csvcl = gameObject.GetComponent<CSVClass>();

    }

    // FixedUpdate is called right before Update
    void Update()
    {

        gap = (SubjectNumber-1)*13*8;
        
        time_new = Time.realtimeSinceStartup;

        time_diff = time_new - time_new_start;

        time_new_start = Time.realtimeSinceStartup;

        var avatar = csv.pose;
        var color = csvcl.pose;

        Rigidbody[] rb = gameObject.GetComponentsInChildren<Rigidbody>();

        Vector3[] qr = new Vector3[rb.Length];
        Quaternion corr;
        corr = Quaternion.Euler(corr_roll, corr_pitch, corr_yaw);
        //Quaternion(0, 1, 0, Mathf.Cos(Mathf.PI/4));

        Quaternion torso_rot = Quaternion.identity;
        Quaternion arm_rot = Quaternion.identity;

        if (avatar.Length>0)
        {
            for (int i = 0; i < rb.Length; i++)
                {

                int torso = gap + 8 * i;

                count += 1;
                
                // right to left handed pos
                rb[i].position = new Vector3(-avatar[torso + 1], avatar[torso + 2], avatar[torso + 3]);
                // CORRECTION FOR Z-UP IN MOCAP
                // rb[i].position = new Vector3(-udp.avatar[torso + 1], udp.avatar[torso + 3], -udp.avatar[torso + 2]);

                if (i==0)
                {
                    pos_diff = ZeroPosition - rb[0].position;
                }

                if (ResetPosition)
                {
                    rb[i].position = rb[i].position + pos_diff;
                    if(count==1)
                    {
                        transform.Rotate(Quaternion.Inverse(rb[0].rotation).eulerAngles);
                    }
                }


                // right to left handed quaternion
                Quaternion rot = new Quaternion(-avatar[torso + 4],avatar[torso + 5], avatar[torso + 6], -avatar[torso + 7]);
                var fin_rot = Quaternion.identity;
                fin_rot = rot * corr;

                try
                {
                    rb[i].rotation = fin_rot.normalized;
                }
                    catch (Exception e)
                {
                    print("waiting for stable skeleton data");
                }


                if (HeadStraight)
                {
                    if (i==3)
                    {
                        rb[i].rotation = rb[2].rotation;
                        rb[i].position = rb[2].position +  rb[2].rotation * neckDist;
                    }
                    if (i==4)
                    {
                        rb[i].rotation = rb[2].rotation;
                        rb[i].position = rb[2].position +  rb[2].rotation * (neckDist + headDist);
                    }

                }


                if (HandsStraight)
                {
                    if (i==8)
                    {
                        rb[i].rotation = rb[7].rotation;
                        rb[i].position = rb[7].position - rb[7].rotation * handDist;
                    }
                    if (i==12)
                    {
                        rb[i].rotation = rb[11].rotation;
                        rb[i].position = rb[11].position + rb[11].rotation * handDist;
                    }

                }

                MakeEverythingNeutral();

                if (color[1] == 4)
                {
                    TorsoGO.GetComponent<MeshRenderer>().material = Torso;
                }
                if (color[1] == 1)
                {
                    LeftFAGO.GetComponent<MeshRenderer>().material = SteeringWheel;
                    RightFAGO.GetComponent<MeshRenderer>().material = SteeringWheel;
                }
                
            }
            // UnityEngine.Debug.Log("TORSO ROTATION (Q)= " + torso_rot);
            // UnityEngine.Debug.Log("TORSO ROTATION = " + torso_rot.ToEulerAngles() * 180 / Mathf.PI);
        }

    }

    void MakeEverythingNeutral()
    {
        TorsoGO.GetComponent<MeshRenderer>().material = Neutral;
        LeftFAGO.GetComponent<MeshRenderer>().material = Neutral;
        RightFAGO.GetComponent<MeshRenderer>().material = Neutral;
    }

    // Update is called once per frame
    // void Update()
    // {
    // }

}