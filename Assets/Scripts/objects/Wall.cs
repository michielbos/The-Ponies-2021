using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
    TerrainTile startTile;
    TerrainTile endTile;
    List<TerrainTile> path;
    TerrainManager tm = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainManager>();
    public Wall(TerrainTile s,TerrainTile e)
    {
        startTile = s;
        endTile = e;
        if (s.x > e.x)
        {
            for (int i = s.x; i < e.x; i--)
            {

            }
        }
        for(int i = s.x; i <= e.x; i++)
        {
            
        }
    }
}
