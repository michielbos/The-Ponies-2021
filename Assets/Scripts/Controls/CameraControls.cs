using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {

	public Transform holder;
    public GameObject g;
    public GameObject Fire;
    Vector3 panStart;
	Vector3 panStartPos;
	new Camera camera;
    //Vector3 start = new Vector3();
    const float minSize = 1;
	const float maxSize = 32;
    bool clicked = false;
    TerrainTile pathstart;
    //List<GameObject> pathlist=new List<GameObject>();
	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
        holder = camera.transform;
	}
	
	// Update is called once per frame
	void Update () {
	    //I guess this is debugging code.
		//Remove or fix & refactor.
        /*if (Input.GetButtonDown("Fire1") &&Input.GetKey(KeyCode.LeftControl))
        {
            /*Vector3 click = Input.mousePosition;
            //Debug.Log("" + camera.ScreenToWorldPoint(click));
            int x = (int)(camera.ScreenToWorldPoint(click).x);
            int y = (int)(camera.ScreenToWorldPoint(click).z);
            //TODO: Fix IndexOutOfRangeException.
            Debug.Log(""+x +" "+ y);*//*
            TerrainTile tile = MousePosTile();
            if (tile == null)
            {
                //tm.AddStuff(g, x, y);
            }
            else if(tm.graph.nodes[tile].cost==1)
            {
                //MousePosTile().GetComponent<MeshRenderer>().material = null;
                foreach(TileEdge t in tm.graph.nodes[tile].edges)
                {
                    Debug.Log(t.cost);
                }
                //you don't really want to walk in to a fire.
                tm.graph.nodes[tile].ChangeCost(1000);
                GameObject fire =Instantiate(Fire);
                fire.transform.position = new Vector3(tile.dummyTile.transform.position.x+.5f,0,tile.dummyTile.transform.position.z+.5f);
            }

        }
        if (Input.GetButtonDown("Fire1") && Input.GetKey(KeyCode.LeftShift))
        {
            TerrainTile tile = MousePosTile();
            if (tile != null)
            {
                if (pathstart == null)
                {
                    pathstart = tile;
                    foreach(GameObject sphere in pathlist)Destroy(sphere);
                    Debug.Log(pathlist.Count);
                    pathlist = new List<GameObject>();
                    
                }
                else
                {
                    foreach (TileNode node in tm.Pathfind(pathstart, tile))
                    {
                        GameObject sphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
                        sphere.transform.position=node.tile.dummyTile.transform.position;
                        sphere.transform.localScale = new Vector3(.3f, .3f, .3f);

                        pathlist.Add(sphere);
                    }
                    pathstart = null;
                }

            }
        }*/
        if (Input.GetButtonDown("Fire3"))
        {
            clicked = !clicked;
            if (clicked)
            {
                //start=camera.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                //Vector3 end = camera.ScreenToWorldPoint(Input.mousePosition);
				//TODO: Fix IndexOutOfRangeException.
                /*TileNode[] path = tm.Pathfind(tm.Get((int)start.x, (int)start.z), tm.Get((int)end.x, (int)end.z));
                foreach(TileNode p in path)
                {
                    int i = p.tile.x;
                    int j = p.tile.y;
                    Instantiate(g, new Vector3(i, 2, j), Quaternion.identity, tm.transform);
                }*/
                //tm.addWall(g, tm.Get((int)start.x, (int)start.z), tm.Get((int)end.x, (int)end.z));
            }
        }
            if (Input.GetButtonDown("Fire2"))
		{
			panStart = Input.mousePosition;
			panStartPos = holder.transform.position;
		}
        if (Input.GetButton("Fire2"))
		{
            //Debug.Log("fire");
			holder.position = panStartPos + LevelVector(transform.up) * (Input.mousePosition - panStart).y * 6 * camera.orthographicSize / Screen.height
											 + LevelVector(transform.right) * (Input.mousePosition - panStart).x * 4 * camera.orthographicSize / Screen.width;
		}

		int scrollDir = (int)Mathf.Clamp(Input.mouseScrollDelta.y, -1, 1);
		camera.orthographicSize *= scrollDir == -1 ? .5f : scrollDir == 1 ? 2 : 1;
		camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minSize, maxSize);
	}

	Vector3 LevelVector (Vector3 v)
	{
		Vector3 v2 = v;
		v2.y = 0;
		return v2;
	}

	public void Rotate(bool cc)
	{
		holder.Rotate(0, cc ? -90 : 90, 0);
	}
    private TerrainTile MousePosTile()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        TerrainTile Tile=null;
        if (Physics.Raycast(ray, out hit, 1000))
        {
            new Vector3(hit.transform.position.x + 0.5f, 0, hit.transform.position.z + 0.5f);
            if (Input.GetMouseButtonDown(0))
            {
                Tile=hit.collider.GetComponent<TerrainTileDummy>().terrainTile;
            }
        }
        return Tile;
    }
}
