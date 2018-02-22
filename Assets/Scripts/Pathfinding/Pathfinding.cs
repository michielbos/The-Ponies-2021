using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {
    TileGraph graph;
    List<TileNode> Q = new List<TileNode>();
    List<TileNode> path;
    public Pathfinding(TileGraph graph)
    {
        this.graph = graph;
        Dictionary<TileNode, float> distance = new Dictionary<TileNode, float>();
        //Dictionary<TileNode, TileNode> previous = new Dictionary<TileNode, TileNode>();
        foreach (TileNode n in graph.nodes.Values)
        {
            distance[n] = int.MaxValue;
            Q.Add(n);
        }
    }
    public Pathfinding()
    {
        graph = GameObject.FindGameObjectWithTag("TerrainManager").GetComponent<TerrainGenerator>().graph;
        Dictionary<TileNode, float> distance = new Dictionary<TileNode, float>();
        //Dictionary<TileNode, TileNode> previous = new Dictionary<TileNode, TileNode>();
        foreach (TileNode n in graph.nodes.Values)
        {
            distance[n] = int.MaxValue;
            Q.Add(n);
        }
    }
    public TileNode[] AStar(TileNode start, TileNode end){
        List<TileNode> closed = new List<TileNode>();
        List<TileNode> opened = new List<TileNode>();
        path = new List<TileNode>();
        opened.Add(start);
        Dictionary<TileNode, TileNode> previous = new Dictionary<TileNode, TileNode>();
        Dictionary<TileNode, float> gScore = new Dictionary<TileNode, float>();
        Dictionary<TileNode, float> fScore = new Dictionary<TileNode, float>();
        foreach(TileNode n in graph.nodes.Values)
        {
            gScore[n] = Mathf.Infinity;
            fScore[n] = Mathf.Infinity;
        }
        
        gScore[start] = 0;
        fScore[start] = Estimate(start, end);
        while (opened.Count > 0)
        {
            float smallest = Mathf.Infinity;
            TileNode current=new TileNode();
            foreach (TileNode n in opened)
            {
                if (fScore[n] < smallest)
                {
                    current = n;
                    smallest = fScore[n];
                }
                
            }
            if (current == end)
            {
                TileNode n = end;
                //Debug.Log("g" + gScore[end] + "f" + fScore[end]);
                while (n != start)
                {
                    path.Add(n);
                    n = previous[n];
                }
                path.Reverse();
                return path.ToArray();
            }
            opened.Remove(current);
            closed.Add(current);
            foreach(TileEdge e in current.edges)
            {
                TileNode n = e.node;
                if (!closed.Contains(n))
                {
                    if (!opened.Contains(n))
                    {
                        opened.Add(n);
                    }
                    float score = gScore[current] + e.cost;
                    if (score < gScore[n])
                    {
                        previous[n] = current;
                        gScore[n] = score;
                        fScore[n] = gScore[n] + Estimate(n, end);
                    }
                }
            }
        }
        return null;
        }
    float Estimate(TileNode here,TileNode there)
    {
        return Mathf.Sqrt(Mathf.Pow(here.tile.x-there.tile.x,2)+ Mathf.Pow(here.tile.y - there.tile.y, 2));
    }
    
}
