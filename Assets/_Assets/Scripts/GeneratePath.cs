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


public class GeneratePath : MonoBehaviour
{

    public GameObject tube;
    public GameObject turn;
    public GameObject player;

    float pathSize = 1.0f;

    public int pathLength = 10;

    Vector3 startPos;

    float updateTime;

    // Use this for initialization
    void Start()
    {

        // var wall = GameObject.Find("WallL").gameObject.GetComponent<Tube>();

        this.gameObject.transform.position = Vector3.zero;
        // startPos = player.transform;

        updateTime = Time.realtimeSinceStartup;

        for (int i = 0; i < pathLength; i++)
        {
             Vector3 pos = new Vector3(0,
                                        20,
                                        20 + i * 20);

             Quaternion rot = Quaternion.identity;

             Vector3 turnPos = pos + new Vector3(0, 0, 20);

             Quaternion turnRot = Quaternion.AngleAxis(90, Vector3.fwd);
             // Quaternion turnRot = Quaternion.identity;


             print(turnRot);
    
             GameObject t = (GameObject)Instantiate(tube, pos, rot);
    
             GameObject tu = (GameObject)Instantiate(turn, turnPos, turnRot);
    
             string tubeName = "Tube_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
             string turnName = "Turn_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();

             t.name = tubeName;
             Tube tb = new Tube(t);
             t.name = turnName;
             Turn tr = new Turn(t);
        }
    }

    // Update is called once per frame
    void Update()
    {


    }
}
