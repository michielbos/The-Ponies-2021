using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
    public int height;
    public int width;
    public TerrainTile[,] terrain;
    public List<Wall> walls;
    public Wall wall;
    public TileGraph graph;
    public Pathfinding path;

    public TerrainGenerator()
    {
        height = 50;
        width = 50;
        terrain = new TerrainTile[height, width];
    }
    public void AddStuff(GameObject t, int i,int j,Vector3[] v)
    {
        Mesh mesh = new Mesh();
        GameObject g= Instantiate(t, new Vector3(i, 0, j), Quaternion.identity, this.transform);
        g.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = v;
        mesh.triangles =new int[] { 0,1,2, 3,4,5};
        mesh.uv = new Vector2[] { };
        mesh.RecalculateNormals();
        TerrainUpdate(g, i, j);
    }

    /*
    public TileNode[] Pathfind(TerrainTile start, TerrainTile end)
    {
        return path.AStar(graph.nodes[start], graph.nodes[end]);
    }
    */

    public void TerrainUpdate(GameObject t,int i, int j)
    {
        TerrainTile tt = t.GetComponent<TerrainTileDummy>().terrainTile;
        tt.x = i;
        tt.y = j;
        terrain[i, j] = tt;
        graph = new TileGraph();
        path = new Pathfinding(graph);
    }
    public List<TerrainTile> GetNeibors(int x,int y)
    {
        List<TerrainTile> a=new List<TerrainTile>();
        if (x > 1) a.Add(Get(x - 1, y));
        if (y > 1) a.Add(Get(x, y - 1));
        if (x < width-1) a.Add(Get(x + 1, y));
        if (y < height-1) a.Add(Get(x, y + 1));
        return a;
    }
    public List<TerrainTile> GetNeibors(TerrainTile t)
    {
        return GetNeibors(t.x,t.y);
    }
    public TerrainTile Get(int x,int y)
    {
        //Debug.Log("get"+x+y);
        if (x < 0 || x > terrain.GetLength(0) || y < 0 || y > terrain.GetLength(1))
        {
            return null;
        }
        else
        {
            return terrain[x, y];
        }
    }
}
