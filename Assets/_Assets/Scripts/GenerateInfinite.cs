using UnityEngine;
using System.Collections;

class Tile
{
    public GameObject theTile;
    public float creationTime;

    public Tile(GameObject t, float ct)
    {
        theTile = t;
        creationTime = ct;
    }
}


public class GenerateInfinite : MonoBehaviour
{

    public GameObject plane;
    public GameObject player;

    int planeSize = 10;
    public int tileNumberX = 10;
    public int tileNumberZ = 10;

    Vector3 startPos;

    Hashtable tiles = new Hashtable();


    // Use this for initialization
    void Start()
    {
        this.gameObject.transform.position = Vector3.zero;
        startPos = Vector3.zero;

        float updateTime = Time.realtimeSinceStartup;

        for (int x = -tileNumberX; x < tileNumberX; x++)
        {
            for (int z = -tileNumberZ; z < tileNumberZ; z++)
            {
                Vector3 pos = new Vector3((x * planeSize + startPos.x),
                                           0,
                                           z * planeSize + startPos.z);

                //print(pos);

                GameObject t = (GameObject)Instantiate(plane, pos, Quaternion.identity);

                string tileName = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                //Debug.Log(tileName);
                t.name = tileName;
                Tile tile = new Tile(t, updateTime);
                tiles.Add(tileName, tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //determine how far the character moved since last update
        int xMove = (int)(player.transform.position.x - startPos.x);
        int zMove = (int)(player.transform.position.z - startPos.z);

        if (Mathf.Abs(xMove) >= planeSize || Mathf.Abs(zMove) >= planeSize)
        {
            float updateTime = Time.realtimeSinceStartup;

            //force integer and round to nearest tile
            int playerX = (int)(Mathf.Floor(player.transform.position.x / planeSize) * planeSize);
            int playerZ = (int)(Mathf.Floor(player.transform.position.z / planeSize) * planeSize);

            for (int x = -tileNumberX; x < tileNumberX; x++)
            {
                for (int z = -tileNumberZ; z < tileNumberZ; z++)
                {
                    Vector3 pos = new Vector3((x * planeSize + playerX),
                                              0,
                                              (z * planeSize + playerZ));

                    string tileName = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();

                    if (!tiles.ContainsKey(tileName))
                    {
                        GameObject t = (GameObject)Instantiate(plane, pos, Quaternion.identity);

                        t.name = tileName;
                        Tile tile = new Tile(t, updateTime);
                        tiles.Add(tileName, tile);
                    }
                    else
                    {
                        (tiles[tileName] as Tile).creationTime = updateTime;
                    }
                }
            }

            //destroy all tiles not just created or with time updated 
            //and put new tiles and tiles to be kept in a new hashtable
            Hashtable newTerrain = new Hashtable();
            foreach (Tile tls in tiles.Values)
            {
                if (tls.creationTime != updateTime)
                {
                    Destroy(tls.theTile);
                }
                else
                {
                    newTerrain.Add(tls.theTile.name, tls);
                }
            }

            // copy to new hashtable
            tiles = newTerrain;

            startPos = player.transform.position;
        }
    }
}
