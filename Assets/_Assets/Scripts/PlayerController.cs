using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
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
    public float desiredYaw = 0.0f;
    
    private float time_start = 0.0f;
    private float time_diff = 0.0f;
    private float time_new = 0.0f;

    // Use this for initialization
    void Start ()
    {
        time_start = Time.realtimeSinceStartup;

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

        time_diff = time_new - time_start;

        print(time_diff);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveLateral = Input.GetAxis("Vertical");

        // fictional movement control

        //var movement = new Vector3(moveHorizontal, 0.0f, moveLateral);

        //rb.AddForce(movement * _speed);

        // here we add the torques

        // scaling vector
        rotTorque = rotSpeed.Select(n => n * _rotTorqueCoeff).ToArray();

        // on y
        var torqueY = new Vector3(0.0f, rotTorque.Sum(), 0.0f);
        var torqueYAligned = transform.rotation * torqueY;
        rb.AddRelativeTorque(torqueYAligned);


        // here we add the forces
        for (int i = 0; i < rotSpeedAbs.Length; i++)
        {
            rotSpeedAbs[i] = rotSpeed[i] * rotDirection[i];
        }

        rotDrag = rotSpeedAbs.Select(n => n * _rotDragCoeff).ToArray();


        var lever = new Vector3();
        var forceXZ = new Vector3();
        var forceXZAligned = new Vector3();
        var torqueXZ = new Vector3[4];

        // on x and z
        for (int i = 0; i < 4; i++)
        {
            // Debug.Log(_propellers[i].transform.position);
            // Debug.Log(transform.position);

            forceXZ.y = rotDrag[i];

            forceXZAligned = transform.rotation * forceXZ;
            // Debug.Log(transform.rotation);

            lever = -(_propellers[i].transform.position - transform.position);

            // Debug.Log(forceXZ);

            torqueXZ[i] = Vector3.Cross(forceXZAligned, lever);

            // if(i==0)
            // {
            //     Debug.Log(i);
            //     Debug.Log(rotSpeed[i]);
            //     Debug.Log(rotSpeedAbs[i]);
            //     Debug.Log(rotDrag[i]);
            //     Debug.Log(transform.rotation);
            //     Debug.Log(forceXZAligned);
            //     Debug.Log(lever);
            //     Debug.Log(torqueXZ[i]);
            // }        

            rb.AddRelativeTorque(torqueXZ[i]);
        }

        var direction = new Vector3[4];

        totalForce.y = rotDrag.Sum();

        //Debug.Log(totalForce);
        //Debug.Log(_propellers);

        var totalForceAligned = transform.rotation * totalForce;
        rb.AddForce(totalForceAligned);


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
            desiredPitch = Input.GetAxis("Vertical");
            desiredRoll = Input.GetAxis("Horizontal");
            desiredY = Input.GetAxis("Up");
            desiredYaw = Input.GetAxis("Yaw");
        }

        print("Pitch = " + desiredPitch);
        print("Roll = " + desiredRoll);
        print("Yaw = " + desiredYaw);
        print("Y = " + desiredY);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}