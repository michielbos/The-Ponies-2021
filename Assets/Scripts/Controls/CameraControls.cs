using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {

	public Transform holder;
    public TerrainManager tm;
    public GameObject g;
    Vector3 panStart;
	Vector3 panStartPos;
	Camera camera;
    Vector3 start = new Vector3();
    const float minSize = 1;
	const float maxSize = 32;
    bool clicked = false;
	// Use this for initialization
	void Start () {
        camera = GetComponent<Camera>();
        holder = camera.transform;
        tm = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 click = Input.mousePosition;
            Debug.Log("" + camera.ScreenToWorldPoint(click));
            int x = (int)(camera.ScreenToWorldPoint(click).x);
            int y = (int)(camera.ScreenToWorldPoint(click).z);
            if (tm.Get(x, y) == null)
            {
                tm.AddStuff(g, x, y);
            }
            else
            {
                tm.Get(x, y).GetComponent<MeshRenderer>().material = null;
                foreach(TileEdge t in tm.graph.nodes[tm.Get(x, y)].edges)
                {
                    Debug.Log(t.cost);
                }
                tm.graph.nodes[tm.Get(x, y)].ChangeCost(1000);
            }

        }
        if (Input.GetButtonDown("Fire3"))
        {
            clicked = !clicked;
            if (clicked)
            {
                start=camera.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                Vector3 end = camera.ScreenToWorldPoint(Input.mousePosition);
                TileNode[] path = tm.Pathfind(tm.Get((int)start.x, (int)start.z), tm.Get((int)end.x, (int)end.z));
                foreach(TileNode p in path)
                {
                    int i = p.tile.x;
                    int j = p.tile.y;
                    Instantiate(g, new Vector3(i, 2, j), Quaternion.identity, tm.transform);
                }
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
            Debug.Log("fire");
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
}
