using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {
    public int height;
    public int width;
    public TerrainTile[,] terrain;
    public List<Wall> walls;
    public Wall wall;
    public TileGraph graph;
    public Pathfinding path;
    public TerrainManager()
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
    public void AddStuff(GameObject t, int i, int j)
    {
        if (i<width&&j<height&&0<=j&&0<=i&&terrain[i, j] == null) { 
            Mesh mesh = new Mesh();
            GameObject g = Instantiate(t, new Vector3(i, 0, j), Quaternion.identity, this.transform);
            g.GetComponent<MeshFilter>().mesh = mesh;
            mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 0),new Vector3(0, 0, 1) };
            mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5 };
            mesh.uv = new Vector2[] { };
            mesh.RecalculateNormals();
            TerrainUpdate(g, i,j);
        }
    }
    public void AddWall(GameObject t,TerrainTile start, TerrainTile end)
    {
        walls.Add(new Wall(start, end));
        GameObject g = Instantiate(t, new Vector3(start.x, 0,start.y), Quaternion.identity, this.transform);
        g.transform.localScale =new Vector3(1,1,Mathf.Sqrt((start.x - end.x)* (start.x - end.x)+ (start.y - end.y)* (start.y - end.y)));
        g.transform.Rotate(new Vector3(0, Mathf.Atan((start.x - end.x) / (start.y - end.y))));
    }
    public TileNode[] Pathfind(TerrainTile start, TerrainTile end)
    {
        return path.AStar(graph.nodes[start], graph.nodes[end]);
    }
    public void TerrainUpdate(GameObject t,int i, int j)
    {
        TerrainTile tt = t.GetComponent<TerrainTile>();
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
        return terrain[x, y];
    }
}
