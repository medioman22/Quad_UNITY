using UnityEngine;
using System.Collections;

public class GenerateTerrain : MonoBehaviour {

    public Renderer rend;
    public Vector3 pos;


    // Use this for initialization
    void Start () {

        rend = GetComponent<Renderer>();

        pos = transform.position;

        if (Mathf.Abs((pos.x / 10 + pos.z / 10) % 2)==1)
        {
            rend.material.color = new Color(0, 0, 0);
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
