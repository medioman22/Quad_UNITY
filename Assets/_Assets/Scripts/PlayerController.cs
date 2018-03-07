using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public bool DebugMode = false;

    private UDPCommunication udp;

    private Rigidbody rb;
    //private 

    public const int N_PROPELLERS = 4;

    public static bool DataFromUDP = false;

    public float _speed;

    private int _count;

    public float[] table = new float[5];

    public float[] rotSpeed  =  { 1.0f, -1.0f, 1.0f, -1.0f };
    private float[] rotDirection  =  { 1.0f, -1.0f, 1.0f, -1.0f };

    private float[] rotSpeedAbs  =  new float[4];

    public float _rotTorqueCoeff = 1.0f;
	public float _rotDragCoeff = 1.0f;

    private float[] rotTorque = new float[4];
    private float[] rotDrag = new float[4];

    public GameObject[] _propellers;

    private Vector3 totalForce;

    public float propellerDistance = 0.0f;

    public float desiredRoll = 0.0f;
    public float desiredPitch = 0.0f;
    public float desiredY = 0.0f;
    public float desiredYaw = 90.0f;

    public float desiredRollVel = 0.0f;
    public float desiredPitchVel = 0.0f;
    public float desiredYVel = 0.0f;
    public float desiredYawVel = 0.0f;
    
    private float time_start = 0.0f;
    public float time_diff = 0.0f;
    private float time_new = 0.0f;
    private float time_new_start = 0.0f;

    private int milestoneCounter = 1;

    // Use this for initialization
    void Start ()
    {
        time_start = Time.realtimeSinceStartup;
        time_new_start = Time.realtimeSinceStartup;

        time_diff = 0.0f;

        rb = GetComponent<Rigidbody>();

        propellerDistance = Vector3.Magnitude(_propellers[1].transform.position - transform.position);

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

        /////

        // float moveHorizontal = Input.GetAxis("Horizontal");
        // float moveLateral = Input.GetAxis("Vertical");

        // fictional movement control

        //var movement = new Vector3(moveHorizontal, 0.0f, moveLateral);

        //rb.AddForce(movement * _speed);

        /////

        ////////// here we add the torques

        // scaling vector
        rotTorque = rotSpeed.Select(n => Mathf.Sign(n) * n * n * _rotTorqueCoeff).ToArray();
        // rotTorque = rotSpeed.Select(n =>  n * _rotTorqueCoeff).ToArray();

        // on y
        var torqueY = new Vector3(0.0f, rotTorque.Sum(), 0.0f);
        var torqueYAligned = transform.rotation * torqueY;
        rb.AddRelativeTorque(torqueY);


        // here we add the forces
        for (int i = 0; i < rotSpeedAbs.Length; i++)
        {
            rotSpeedAbs[i] = rotSpeed[i] * rotDirection[i];
        }

        rotDrag = rotSpeedAbs.Select(n => Mathf.Sign(n) * n * n * _rotDragCoeff).ToArray();




        if (DataFromUDP)
        {
            Debug.LogWarning("UDP?");
            var controls = udp.controls;

            desiredPitch = controls[0];
            desiredRoll = controls[1];
            desiredYaw = controls[2];
            desiredY = controls[3];
        }
        else
        {
            float max_m_per_s = 10;
            float max_yaw_per_s = 100;
            float mul_factor = 20.0f;
            desiredPitch = Input.GetAxis("Vertical") * mul_factor;
            desiredRoll = -Input.GetAxis("Horizontal") * mul_factor;
            desiredY = desiredY + Input.GetAxis("Up") * max_m_per_s * time_diff;
            desiredYaw = desiredYaw + Input.GetAxis("Yaw") * max_yaw_per_s * time_diff;
        }

        if (DebugMode)
        {
            var pos = transform.position;

            print(" ");
            print("----- START PlayerController DEBUG -----");
            print(" ");

            print("time interval = " + time_diff);
            print("position = " + pos);
            // print("total force = " + totalForceAligned);
            print("torqueY = " + torqueY);
            print("torqueYAligned = " + torqueYAligned);
    }
        // on x and z
        var lever = new Vector3();
        var forceXZ = new Vector3();
        var forceXZAligned = new Vector3();
        var torqueXZ = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            forceXZ.y = rotDrag[i];
            forceXZAligned = transform.rotation * forceXZ;

            var prop_pos = _propellers[i].transform.position;

            rb.AddForceAtPosition(forceXZAligned, prop_pos);
                
            if (DebugMode)
            {
                            print("----- START propeller DEBUG -----");
                            
                            print("propereller number = " + i);
                            print("forceXZAligned = " + forceXZAligned);
                            print("propeller position = " + prop_pos.ToString("F2"));
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {

    if (other.gameObject.CompareTag("Milestone"))
        {
            other.gameObject.SetActive (false);
            print("Milestone # " + milestoneCounter++);
            print("Crossing time = " + (Time.realtimeSinceStartup - time_start));
            print("Crossing point = " + gameObject.transform.position);
        }

    if (other.gameObject.CompareTag("ObstacleWall"))
        {
            var pos = GetComponent<Renderer>().bounds.size;
            print(pos);
        }
    }

}