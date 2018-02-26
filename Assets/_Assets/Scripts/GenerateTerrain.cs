using UnityEngine;
using System.Collections;

public class GenerateTerrain : MonoBehaviour {

    int heightScale = 5;
    float detailScale = 5.0f;

    public Renderer rend;
    public Vector3 pos;


    // Use this for initialization
    void Start () {

        rend = GetComponent<Renderer>();

        pos = transform.position;

        //Debug.Log(pos.x / 10);
        //Debug.Log(pos.z / 10);
        if (Mathf.Abs((pos.x / 10 + pos.z / 10) % 2)==1)
        {
            rend.material.color = new Color(0, 0, 0);
        }


        this.gameObject.AddComponent<MeshCollider>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
