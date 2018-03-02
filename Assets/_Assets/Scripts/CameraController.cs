using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public bool DebugMode = false;

    public GameObject player;

    private Vector3 offset_init;

    // Use this for initialization
    void Start()
    {
        offset_init = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {    // runs AFTER all computations have been done

        float rot = player.transform.eulerAngles.y;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, rot, transform.eulerAngles.z);

        Vector3 offset = Quaternion.Euler(0, rot, 0) * offset_init;

        if (DebugMode)
            {
                print(offset);
                print(rot);
            }

        transform.position = player.transform.position + offset;
    }
}