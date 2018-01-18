using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    TerrainTile startTile;
    TerrainTile endTile;
    int length;
    int direction=0;
    public int x;
    public int y;
	public WallDirection wallDirection;
    List<TerrainTile> path;
    TerrainManager tm = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainManager>();

	public Wall (int x, int y, WallDirection wallDirection) {
		this.x = x;
		this.y = y;
		this.wallDirection = wallDirection;
	}

    public Wall(TerrainTile s, TerrainTile e)
    {
        /*start and end should be interchangable
          assume that gven tiles are right
          direction 0:・,1:-,2:\,3:|,4:/
         */
        startTile = s;
        endTile = e;
        int xdif = (e.x - s.x);
        int ydif = Mathf.Abs( e.y - s.y);
        if (ydif != 0)
        {
            direction = 3;
            if (xdif != 0) direction += (int)Mathf.Sign(xdif);
        }
        else if (xdif != 0) direction = 1;
        if (xdif == 0 || ydif == 0)
        {
            length =Mathf.Abs( xdif + ydif);
        }else length=ydif;
        x = Mathf.Min(s.x, e.x);
        y = Mathf.Min(s.y, e.y);
        
    }

    public Wall(int coordx,int coordy, int dir)
    {
        x = coordx;
        y = coordy;
        direction = dir;
    }

	public WallData GetWallData () {
		return new WallData(x,
			y,
			(int)wallDirection,
			0,
			0);
	}
}
