using UnityEngine;
// using System.Linq;

public class PIDController : MonoBehaviour {

    public bool DebugMode = false;
    public bool UseQuaternions = false;
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

    public float desiredYaw = 90.0f;
    public float desiredYawVel = 0.0f;

    public float propellerDistance = 0.0f;

    public float factorY = 1.0f;

    private float measYOld;
    private float measRollOld;
    private float measPitchOld;
    private float measYawOld;

    private float measAccumRoll;
    private float measAccumPitch;
    private float measAccumYaw;

    private float desiredYOld;
    private float desiredRollOld;
    private float desiredPitchOld;
    private float desiredYawOld;

    private float desiredAccumRoll;
    private float desiredAccumPitch;
    private float desiredAccumYaw;



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
        desiredY = PlayCon.desiredY + PlayCon.standardY;
        desiredYaw = PlayCon.desiredYaw;

        desiredRollVel = PlayCon.desiredRollVel;
        desiredPitchVel = PlayCon.desiredPitchVel;
        desiredYVel = PlayCon.desiredYVel;
        desiredYawVel = PlayCon.desiredYawVel;

        var measY = transform.position.y;
    	var measYVel = rb.velocity.y;

        rotation = transform.rotation;

        if (UseQuaternions)
        {

        }
        else
        {
            var time_diff = PlayCon.time_diff;

            if (time_diff<0.001)
            {
                time_diff = 1;
            }

            var measRoll = rotation.eulerAngles.z;
            var measRollVel = rb.angularVelocity.z;
            var measPitch = rotation.eulerAngles.x;
            var measPitchVel = rb.angularVelocity.x;
            var measYaw = rotation.eulerAngles.y;
            var measYawVel = rb.angularVelocity.y;

            var measRollRad = measRoll * (float)Mathf.PI / (float)180.0;
            var measPitchRad = measPitch * (float)Mathf.PI / (float)180.0;
            var measYawRad = measYaw * (float)Mathf.PI / (float)180.0;

            FixAngles(ref desiredYaw, ref desiredYawOld, ref desiredAccumYaw);
            FixAngles(ref desiredPitch, ref desiredPitchOld, ref desiredAccumPitch);
            FixAngles(ref desiredRoll, ref desiredRollOld, ref desiredAccumRoll);


            FixAngles(ref measYaw, ref measYawOld, ref measAccumYaw);
            FixAngles(ref measPitch, ref measPitchOld, ref measAccumPitch);
            FixAngles(ref measRoll, ref measRollOld, ref measAccumRoll);

            var YErr = desiredY - measY;
            var YErrOld = desiredYOld - measYOld;

            var YVelErr = (YErr - YErrOld)/time_diff;

            var RollErr = desiredRoll - measRoll;
            var RollErrOld = desiredRollOld - measRollOld;

            var RollVelErr = (RollErr - RollErrOld)/time_diff;

            var YawErr = desiredYaw - measYaw;
            var YawErrOld = desiredYawOld - measYawOld;

            var YawVelErr = (YawErr - YawErrOld)/time_diff;

            var PitchErr = desiredPitch - measPitch;
            var PitchErrOld = desiredPitchOld - measPitchOld;

            var PitchVelErr = (PitchErr - PitchErrOld)/time_diff;
    
            desiredYOld = desiredY;
            measYOld = measY;
            desiredRollOld = desiredRoll;
            measRollOld = measRoll;
            desiredPitchOld = desiredPitch;
            measPitchOld = measPitch;
            desiredYawOld = desiredYaw;
            measYawOld = measYaw;

            thrust = (KzD*(YVelErr) + KzP*(YErr))*mass/(Mathf.Cos(measRollRad) *Mathf.Cos(measPitchRad));
        	torqueRoll = (KrollD*(RollVelErr) + KrollP*(RollErr))*Ixx;
        	torquePitch = (KpitchD*(PitchVelErr) + KpitchP*(PitchErr))*Izz;
        	torqueYaw = (KyawD*(YawVelErr) + KyawP*(YawErr))*Iyy;

            var thrustCoeff = thrust / (4.0f * PlayCon._rotDragCoeff);
            var torqueRollCoeff = torqueRoll / (2.0f * PlayCon._rotDragCoeff * PlayCon.propellerDistance);
            var torquePitchCoeff = torquePitch / (2.0f * PlayCon._rotDragCoeff * PlayCon.propellerDistance);
            var torqueYawCoeff = torqueYaw / (4.0f * PlayCon._rotTorqueCoeff);

            var rot1 = (thrustCoeff - torqueRollCoeff + torquePitchCoeff + torqueYawCoeff);
            var rot2 = (thrustCoeff - torqueRollCoeff - torquePitchCoeff - torqueYawCoeff);
            var rot3 = (thrustCoeff + torqueRollCoeff - torquePitchCoeff + torqueYawCoeff);
            var rot4 = (thrustCoeff + torqueRollCoeff + torquePitchCoeff - torqueYawCoeff);

            PlayCon.rotSpeed[0] = Mathf.Sqrt(Mathf.Abs(rot1)) * Mathf.Sign(rot1);
            PlayCon.rotSpeed[1] = -Mathf.Sqrt(Mathf.Abs(rot2)) * Mathf.Sign(rot2);
            PlayCon.rotSpeed[2] = Mathf.Sqrt(Mathf.Abs(rot3)) * Mathf.Sign(rot3);
            PlayCon.rotSpeed[3] = -Mathf.Sqrt(Mathf.Abs(rot4)) * Mathf.Sign(rot4);

            if(DebugMode)
            {
                print(" ");
                print("----- START PID DEBUG -----");
                print(" ");

                print("err = " + YErr.ToString("F2") +
                      " " + RollErr.ToString("F2") + 
                      " " + PitchErr.ToString("F2") + 
                      " " + YawErr.ToString("F2"));

                print("err velocity = " + YVelErr.ToString("F2") + 
                      " " + RollVelErr.ToString("F2") + 
                      " " + PitchVelErr.ToString("F2") + 
                      " " + YawVelErr.ToString("F2"));

                print("thrustCoeff = " + thrustCoeff.ToString("F2") + 
                      " torqueRollCoeff = " + torqueRollCoeff.ToString("F2") + 
                      " torquePitchCoeff = " + torquePitchCoeff.ToString("F2") + 
                      " torqueYawCoeff " + torqueYawCoeff.ToString("F2"));

                print("rotation speed = " + PlayCon.rotSpeed[0].ToString("F2") + 
                      " " + PlayCon.rotSpeed[1].ToString("F2") + 
                      " " + PlayCon.rotSpeed[2].ToString("F2") + 
                      " " + PlayCon.rotSpeed[3].ToString("F2"));
            }
        }
        //rb.transform.eulerAngles = new Vector3(desiredPitch, desiredYaw, desiredRoll);


        //print(measRoll);
        //print(Mathf.Cos(measRoll));
        //print(thrust);

        //Debug.Log(measRoll);
        //Debug.Log((torqueRoll));

        //Debug.Log(PlayCon._rotDragCoeff);
        //Debug.Log(thrust / (4.0f * PlayCon._rotDragCoeff));
        //Debug.Log(rotSpeed[0]);
        //Debug.Log(rotSpeed[1]);
        //Debug.Log(rotSpeed[2]);
        //Debug.Log(rotSpeed[3]);
    }

    void FixAngles(ref float meas, ref float measOld, ref float measAccum)
    {
        if (measOld - measAccum * 360.0f - meas > 350.0f)
        {
            measAccum = measAccum + 1;
        }
        else
        {
            if (measOld - measAccum * 360.0f - meas < -350.0f)
            {
                    measAccum = measAccum - 1;
            }
        }

        meas = meas + measAccum * 360.0f;
    }
}