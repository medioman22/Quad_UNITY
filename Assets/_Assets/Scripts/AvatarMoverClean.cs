using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;

using System.Collections.Generic;
using System;


public class AvatarMoverClean : MonoBehaviour
{

    public GameObject[] limbs = new GameObject[13];
    private Rigidbody[] rb = new Rigidbody[13];
    public List<int> limb_order = new List<int>(new int[] { 3, 6, 7, 8, 9, 10, 11, 12, 13 });
    public Quaternion[] rot_init = new Quaternion[13];

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
    private CSVPlotterFirst csvF;
    //private 

    private float time_start = 0.0f;
    public float time_diff = 0.0f;
    private float time_new = 0.0f;
    private float time_new_start = 0.0f;

    public string SubjectFolder = "";

    private int count = 0;

    void Start()
    {
        
        time_start = Time.realtimeSinceStartup;
        time_new_start = Time.realtimeSinceStartup;

        time_diff = 0.0f;

        csv = gameObject.GetComponent<CSVPlotter>();
        csvF = gameObject.GetComponent<CSVPlotterFirst>();

        for (int i = 0; i < limbs.Length; i++)
        {
            rb[i] = limbs[i].GetComponent<Rigidbody>();
        }

        var init_pose = csvF.pose;
        SetInitPose(init_pose);
    }

    // FixedUpdate is called right before Update
    void Update()
    {

        time_new = Time.realtimeSinceStartup;
        time_diff = time_new - time_new_start;
        time_new_start = Time.realtimeSinceStartup;

        var avatar = csv.euler;


        if (avatar.Count>0)
        {
            for (int i = 0; i < 9; i++)
            {
                // UnityEngine.Debug.Break();

                int gap = 3*i;

                count += 1;
                
                // UnityEngine.Debug.Break();
                // right to left handed quaternion
                Quaternion rot = Quaternion.Euler(avatar[gap + 1], -avatar[gap + 2], -avatar[gap]);
                // Quaternion rot = new Quaternion(-avatar[torso + 4],avatar[torso + 5], avatar[torso + 6], -avatar[torso + 7]);
                var fin_rot = rot_init[i];
                fin_rot = rot * rot_init[limb_order[i] - 1];

                try
                {
                    // rb[0].rotation = fin_rot.normalized;
                    rb[limb_order[i]-1].transform.localRotation = fin_rot.normalized;
                }
                catch (Exception e)
                {
                    print("waiting for stable skeleton data");
                }

                // if (HandsStraight)
                // {
                //     if (i==8)
                //     {
                //         rb[i].rotation = rb[7].rotation;
                //         rb[i].position = rb[7].position - rb[7].rotation * handDist;
                //     }
                //     if (i==12)
                //     {
                //         rb[i].rotation = rb[11].rotation;
                //         rb[i].position = rb[11].position + rb[11].rotation * handDist;
                //     }

                // }
            }
        }

    }

    void SetInitPose(float[] pose)
    {
        var avatar = pose;

        Quaternion[] rot_all = new Quaternion[rb.Length];

        if (avatar.Length>0)
        {
            for (int i = 0; i<rb.Length; i++)
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
                        if (count == 1)
                        {
                            transform.Rotate(Quaternion.Inverse(rb[0].rotation).eulerAngles);
                        }
                    }


                    // right to left handed quaternion
                    rot_all[i] = new Quaternion(-avatar[torso + 4], avatar[torso + 5], avatar[torso + 6], -avatar[torso + 7]);

                    rot_init[2] = Quaternion.Euler(0,0,0);
                    rot_init[5] = Quaternion.Inverse(rot_all[2]) * rot_all[5];
                    rot_init[6] = Quaternion.Inverse(rot_all[5]) * rot_all[6];
                    rot_init[7] = Quaternion.Inverse(rot_all[6]) * rot_all[7];
                    rot_init[8] = Quaternion.Inverse(rot_all[7]) * rot_all[8];
                    rot_init[9] = Quaternion.Inverse(rot_all[2]) * rot_all[9];
                    rot_init[10] = Quaternion.Inverse(rot_all[9]) * rot_all[10];
                    rot_init[11] = Quaternion.Inverse(rot_all[10]) * rot_all[11];
                    rot_init[12] = Quaternion.Inverse(rot_all[11]) * rot_all[12];

                    var fin_rot = Quaternion.identity;
                    fin_rot = rot_all[i];

                    try
                    {
                        rb[i].rotation = fin_rot.normalized;
                    }
                    catch (Exception e)
                    {
                        print("waiting for stable skeleton data");
                    }
                }
        }
    }
}