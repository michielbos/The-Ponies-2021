using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGraph {
    public Dictionary<TerrainTile, TileNode> nodes;
    public TileGraph()
    {
        TerrainManager q = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainManager>();
        TileGraph t=new TileGraph(q);
        nodes = t.nodes;
    }
        public TileGraph(TerrainManager terrainmanager)
    {
        nodes=new Dictionary<TerrainTile, TileNode>();
        for(int x = 0; x < terrainmanager.width; x++)
        {
            for(int y = 0; y < terrainmanager.height; y++)
            {
                TerrainTile t=terrainmanager.Get(x,y);
                if (t!=null)
                {
                    TileNode n = new TileNode();
                    n.tile = t;
                    nodes.Add(t, n);
                }


            }
        }
        int ec = 0;
        foreach (TerrainTile t in nodes.Keys)
        {
            TileNode n=nodes[t];
            List<TileEdge> edges = new List<TileEdge>();
            foreach(TerrainTile neibors in terrainmanager.GetNeibors(t))
            {
                if (neibors!=null&&nodes.ContainsKey(neibors))
                {
                    TileEdge e = new TileEdge();
                    e.cost = 1;
                    e.node = nodes[neibors];
                    edges.Add(e);
                    ec++;
                }
            }
            n.edges = edges.ToArray();
        }
        Debug.Log(ec);
    }
	
	public int getsize()
    {
        return nodes.Keys.Count;
    }
    public void pathfind()
    {

    }
	
}
