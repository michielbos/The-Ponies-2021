using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileNode{

    public TerrainTile tile;
    public TileEdge[] edges;
    public float cost = 1;
    public void ChangeCost(float a)
    {
        foreach(TileEdge e in edges)
        {
            e.cost =a;
        }
    }

		
	
}
