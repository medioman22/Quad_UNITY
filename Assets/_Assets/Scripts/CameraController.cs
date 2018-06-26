using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public bool DebugMode = false;

    public GameObject player;

    private Vector3 offset_init;

    private Vector3 rot_init;
    private Vector3 rot;

    private Vector3 pos_init;
    private Vector3 pos;

    // Use this for initialization
    void Start()
    {
        offset_init = transform.position - player.transform.position;

        rot_init = player.transform.eulerAngles;
        pos_init = player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {    // runs AFTER all computations have been done

        rot = player.transform.eulerAngles;
        pos = player.transform.position;

        transform.eulerAngles = new Vector3(rot_init.x, rot.y, rot_init.z);

        Vector3 offset = Quaternion.Euler(transform.eulerAngles) * offset_init;

        transform.position = player.transform.position + offset;

        if (DebugMode)
            {
                print(offset);
                print(rot);
            }
    }
}