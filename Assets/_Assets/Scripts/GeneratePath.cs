using UnityEngine;
using System.Collections;

class Tube
{
    public GameObject theTube;

    public Tube(GameObject t)
    {
        theTube = t;
    }
}

class Turn
{
    public GameObject theTurn;

    public Turn(GameObject t)
    {
        theTurn = t;
    }
}

class No_Turn
{
    public GameObject theNoTurn;

    public No_Turn(GameObject t)
    {
        theNoTurn = t;
    }
}


public class GeneratePath : MonoBehaviour
{

    public bool DebugMode = false;

    public GameObject tube;
    public GameObject turn;
    public GameObject no_turn;
    public GameObject player;

    float pathSize = 1.0f;

    public int pathLength = 10;

    Vector3 startPos;

    float updateTime;

    public enum _next_turn_list {Forward, Up, Right, Left, Down};

    private Vector3 pos;

    private float _next_turn = 0;
    private float _old_turn = 0;

    Quaternion rot = Quaternion.identity;

    Quaternion old_rot_turn = Quaternion.identity;

    GameObject next_gameobj;

    private float tube_l = 20;
    private float tube_d = 20;

    private float init_d = 20;

    // Use this for initialization
    void Start()
    {
        // init position is in front of drone

        Vector3 startPos = player.transform.position;

        updateTime = Time.realtimeSinceStartup;

        pos = pos + startPos  + new Vector3(0,
                                            0,
                                            init_d);

        Vector3 turnPos = pos + rot * new Vector3(0, 0, 20);

        Quaternion rot_turn = rot;

        Quaternion _old_rot = rot;

        Vector3 _old_pos = pos;

        next_gameobj = no_turn;

        for (int j = 0; j < pathLength; j++)
        {

            int i = j % 5;

            // display objects
    
            var t = (GameObject)Instantiate(tube, pos, _old_rot);
                
            var tu = (GameObject)Instantiate(next_gameobj, turnPos, rot_turn);

            string tubeName = "Tube_" + i.ToString();
            string turnName = "Turn_" + i.ToString();

            t.name = tubeName;
            Tube tb = new Tube(t);
            tu.name = turnName;
            Turn tr = new Turn(tu);

            // memo old values

            _old_pos = pos;
            _old_rot = rot;
            _old_turn = _next_turn;
            old_rot_turn = rot_turn;

            // update new values

            _next_turn = Mathf.Round(Random.Range(-0.5f, 4.5f));
            // _next_turn = 4;

            pos = turnPos + rot * new Vector3(0, 0, tube_d);

            turnPos = pos + rot * new Vector3(0, 0, 20);

            switch ((int)_next_turn)
            {
                case 0:
                    rot = rot;
                    break;
                case 1:
                    rot = rot * Quaternion.AngleAxis(90, Vector3.left);
                    break;
                case 2:
                    rot = rot * Quaternion.AngleAxis(90, Vector3.up);
                    break;
                case 3:
                    rot = rot * Quaternion.AngleAxis(90, Vector3.right);
                    break;
                case 4:
                    rot = rot * Quaternion.AngleAxis(90, Vector3.down);
                    break;
            }
                
            rot_turn = _old_rot * Quaternion.AngleAxis(90 * (_next_turn-1) , Vector3.back);

            if (_next_turn != 0)
            {             
                next_gameobj = turn;
            }
            else
            {             
                next_gameobj = no_turn;
            }

            // DEBUG

            if (DebugMode)
            {
                print(" ");
                print("----- START GENPATH DEBUG -----");
                print(" ");
    
                print("Iter = " + (j + 1));
                print("Turning = " + _next_turn);
                print("Old rot =  " + _old_rot.ToEulerAngles());
                print("New rot = " + rot.ToEulerAngles());
                print("Old turn rot =  " + old_rot_turn.ToEulerAngles());
                print("New turn rot = " + rot_turn.ToEulerAngles());
                print("Turn rot update = " + Quaternion.AngleAxis(90 * (_next_turn-1) , Vector3.back).ToEulerAngles());
                print("pos = " + pos +
                      " = " + _old_pos +
                      " + " + _old_rot * new Vector3(0, 0, tube_l + tube_d/2) +
                      " + " + rot * new Vector3(0, 0, tube_d/2));
            }

        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}
