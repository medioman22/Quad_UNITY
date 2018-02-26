using UnityEngine;
using System.Collections;

public static class Parameters {

	//COMMUNICATION
	//vicon output -> unity3D input
	public static int NUMBER_OF_CONTROLS_COMMAND_FLOAT = 5; //3 for the fixedwing //4 for the quad
	public enum InputQt
	{				  
		ROLL = 0,  	
		PITCH = 1,
		YAW = 2,
		COSE = 3,
	    BATTERY = 4
	};

    public static int NUMBER_TEST = 1; //3 for the fixedwing //4 for the quad
    public enum InputQtTest
    {                 
        Test = 0
    };

    public static int NUMBER_OF_LOW_LEVEL_COMMANDS = 4; //3 for the fixedwing //4 for the quad
	public enum InputUDP
	{				  
		LEFT_DEFLECTION = 0,  	
		RIGHT_DEFLECTION = 1,
		RUDDER_DEFLECTION = 2,
		THRUST = 3				
	};

	//PLATFORM CONFIGURATION
	public static int NUMBER_OF_KEYJOY = 4; //number of low level commands comming from UDP
	public enum outputKeyJoy
	{
		ROLL_KJ = 0,  			  // [-1 ... 1]	
		PITCH_KJ = 1,  			  // [-1 ... 1]
		YAW_KJ = 2,
		THRUST_KJ = 3			  // [ 0 ... 1]
	};

    public enum enum_controller
    {
        attitudeint = 0,
        rate = 1
    };

    public enum phase_Exp_closedLoop
    {
        //setup
        //MATB si s2
        s3_openloop = 0,
        s3_closedloop = 1,
        s4_evaluation = 2,
        s6_stress_in_air = 3,
        s7_village = 4,
        s8_village = 5,
        freeflight = 6
    };

    /*
        s5: in the air, with more distant waypoints than in s3 & s4
        s6: idem + math
        s7: in the village, with waypoints equal or closer than in s3 & s4
        s8: idem + math
        - arrow, shows only next waypoints, star number of reached waypoints below, 2mn, if crash starts at the last waypoints (one life less, from 3 hearts), distributed more less equally
    */

    //Sensors
    public static int NUMBER_OF_SENSORS = 14;
	public enum SENSORS_INPUT //relatively to North East Down (NED) bodyframe & semi-body frame
	{
		ROLL_SENSOR             = 0, // [-180;180] [°] (after tools.convertAngle in main)
		PITCH_SENSOR            = 1, // [-180;180] [°] (after tools.convertAngle in main)
		YAW_SENSOR              = 2, // [0;360] [°] = heading
		ROLL_RATE_SENSOR        = 3, // [°/s]
		PITCH_RATE_SENSOR       = 4, // [°/s]
		YAW_RATE_SENSOR         = 5, // [°/s] = heading rate
        VELOCITY_BODY_X         = 6, // [m/s]
        VELOCITY_BODY_Y         = 7, // [m/s]
        VELOCITY_BODY_Z         = 8, // [m/s]
        VELOCITY_SEMI_LOCAL_X   = 9, // [m/s]
        VELOCITY_SEMI_LOCAL_Y   = 10, // [m/s]
        VELOCITY_SEMI_LOCAL_Z   = 11, // [m/s]
        CORR_ROLL               = 12,
        CORR_PITCH              = 13
    };

    //MAPPING
    /*ROLL   = 0
     *PITCH  = 1 
     *THRUST = 2 
     *Y_RATE = 0
     *Z_VEL	 = 1 (semi-local frame)
     *X_VEL  = 2 (local frame)
    */

    //Avatar
    public enum enum_flightStyle
    {
        torso = 0,
        torso_arm = 1,
        hands_birdly = 2,
        personnalized = 3
    }

    public static int NUMBER_OF_AVATAR_ANGLES = 15; //21; //3*7
}
