using UnityEngine;
// using System.Linq;

public class PIDController : MonoBehaviour {

    public const int N_PROPELLERS = 4;

    private Rigidbody rb;

    [HideInInspector]
    public float[] rotSpeed  =  { 1.0f, -1.0f, 1.0f, -1.0f };
    [HideInInspector]
    public const float[] rotDirection = null;

    private float[] rotSpeedAbs  =  new float[4];

    [HideInInspector]
    public float _rotTorqueCoeff;
    [HideInInspector]
	public float _rotDragCoeff;

    private float[] rotTorque = new float[4];
    private float[] rotDrag = new float[4];

    [HideInInspector]
    public GameObject[] _propellers;

    private Vector3 totalForce;

    public const float gravity = 9.81f;

    public float thrust = 0;

    public float torqueRoll = 0;
    public float torquePitch = 0;
    public float torqueYaw = 0;

    public float KzD = 2.5f;
    public float KzP = 1.5f;
    public float KrollD = 1.75f;
    public float KrollP = 6.0f;
    public float KpitchD = 1.75f;
    public float KpitchP = 6.0f;
    public float KyawD = 1.75f;
    public float KyawP = 6.0f;

    public float mass = 0.0f;

    public float Ixx = 0.0f;
    public float Iyy = 0.0f;
    public float Izz = 0.0f;


    public float desiredY = 7.0f;
    public float desiredYVel = 0.0f;

    public float desiredRoll = 0.0f;
    public float desiredRollVel = 0.0f;

    public float desiredPitch = 0.0f;
    public float desiredPitchVel = 0.0f;

    public float desiredYaw = 0.0f;
    public float desiredYawVel = 0.0f;

    public float propellerDistance = 0.0f;

    public float standardY = 0.0f;
    public float factorY = 1.0f;

    public Quaternion rotation = Quaternion.identity;

    private PlayerController PlayCon;

    // Use this for initialization
    void Start () {
        PlayCon = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        mass = rb.mass;
        Ixx = rb.angularDrag;
        Iyy = rb.angularDrag;
        Izz = rb.angularDrag;
    }

    // FixedUpdate is called right before Update
    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
    {
        desiredRoll = PlayCon.desiredRoll;
        desiredPitch = PlayCon.desiredPitch;
        desiredY = standardY + factorY*PlayCon.desiredY;
        desiredYaw = PlayCon.desiredYaw;

        var measY = transform.position.y;
    	var measYVel = rb.velocity.y;

     //   var measRoll = transform.eulerAngles.z;
     //   var measRollVel = rb.angularVelocity.z;
    	//var measPitch = transform.eulerAngles.x;
    	//var measPitchVel = rb.angularVelocity.x;
    	//var measYaw = transform.eulerAngles.y;
    	//var measYawVel = rb.angularVelocity.y;

        //print(transform.eulerAngles.z);
        //print(transform.eulerAngles.x);
        //print(transform.eulerAngles.y);

        rotation = transform.rotation;

        var measRoll = rotation.eulerAngles.z;
        var measRollVel = rb.angularVelocity.z;
        var measPitch = rotation.eulerAngles.x;
        var measPitchVel = rb.angularVelocity.x;
        var measYaw = rotation.eulerAngles.y;
        var measYawVel = rb.angularVelocity.y;

        // correct rotation errors : -180 to +180

        if (measRoll>180.0)
        {
            measRoll = measRoll - (float)360.0;
        }

        if (measPitch > 180.0)
        {
            measPitch = measPitch - (float)360.0;
        }

        if (measYaw > 180.0)
        {
            measYaw = measYaw - (float)360.0;
        }

        var measRollRad = measRoll * (float)Mathf.PI / (float)180.0;
        var measPitchRad = measPitch * (float)Mathf.PI / (float)180.0;
        var measYawRad = measYaw * (float)Mathf.PI / (float)180.0;

        thrust = (gravity + KzD*(desiredYVel - measYVel) + KzP*(desiredY - measY))*mass/(Mathf.Cos(measRollRad) *Mathf.Cos(measPitchRad));
    	torqueRoll = (KrollD*(desiredRollVel - measRollVel) + KrollP*(desiredRoll - measRoll))*Ixx;
    	torquePitch = (KpitchD*(desiredPitchVel - measPitchVel) + KpitchP*(desiredPitch - measPitch))*Izz;
    	torqueYaw = (KyawD*(desiredYawVel - measYawVel) + KyawP*(desiredYaw - measYaw))*Iyy;

        var thrustCoeff = thrust/(4.0f*PlayCon._rotDragCoeff);
        var torqueRollCoeff = torqueRoll/(2.0f*PlayCon._rotDragCoeff*PlayCon.propellerDistance);
        var torquePitchCoeff = torquePitch/(2.0f*PlayCon._rotDragCoeff*PlayCon.propellerDistance);
        var torqueYawCoeff = torqueYaw/(4.0f*PlayCon._rotTorqueCoeff);

        var rot1 = (thrustCoeff - torqueRollCoeff + torquePitchCoeff + torqueYawCoeff);
        var rot2 = (thrustCoeff - torqueRollCoeff - torquePitchCoeff - torqueYawCoeff);
        var rot3 = (thrustCoeff + torqueRollCoeff - torquePitchCoeff + torqueYawCoeff);
        var rot4 = (thrustCoeff + torqueRollCoeff + torquePitchCoeff - torqueYawCoeff);

        //PlayCon.rotSpeed[0] = Mathf.Sqrt(Mathf.Abs(rot1)) * Mathf.Sign(rot1);
        //PlayCon.rotSpeed[1] = -Mathf.Sqrt(Mathf.Abs(rot2)) * Mathf.Sign(rot2);
        //PlayCon.rotSpeed[2] = Mathf.Sqrt(Mathf.Abs(rot3)) * Mathf.Sign(rot3);
        //PlayCon.rotSpeed[3] = -Mathf.Sqrt(Mathf.Abs(rot4)) * Mathf.Sign(rot4);

        rb.transform.eulerAngles = new Vector3(desiredPitch, desiredYaw, desiredRoll);


        //print(measRoll);
        //print(Mathf.Cos(measRoll));
        //print(thrust);

        //Debug.Log(measRoll);
        //Debug.Log((torqueRoll));

        //Debug.Log(PlayCon._rotDragCoeff);
        //Debug.Log(thrust / (4.0f * PlayCon._rotDragCoeff));
        //Debug.Log(rotSpeed[1]);
        //Debug.Log(rotSpeed[2]);
        //Debug.Log(rotSpeed[3]);

        if (float.IsNaN(PlayCon.rotSpeed[0]))
        {
            Debug.Log(PlayCon.rotSpeed[0]);
        }


    }
}