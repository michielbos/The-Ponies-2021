using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{

    public GameObject terrainTile;

    List<List<int>> heights = new List<List<int>>();

   /* string heightmap = "0000000011111111\n" +
                       "0000000011111111\n" +
                       "0000000001111111\n" +
                       "0000000001111100\n" +
                       "1100000000111000\n" +
                       "1111110000011000\n" +
                       "1111111000000000\n" +
                       "2222111110000000\n" +
                       "3332222111100000\n" +
                       "3333332211111100\n" +
                       "4444333221111110\n" +
                       "4444443321111111\n" +
                       "5554444322111111\n" +
                       "5555444332111111\n" +
                       "5555544432111111\n" +
                       "5555544432111111";*/
    string heightmap = "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000\n" +
                           "0000000000000000";

    void Start()
    {
        string[] lines = heightmap.Split('\n');
        TerrainManager tm = GetComponent<TerrainManager>() ;
        for (int i = 0; i < 16; i++)
        {
            heights.Add(new List<int>());
            for (int j = 0; j < 16; j++)
            {
                char c = lines[i][j];
                int height = int.Parse(c.ToString());
                heights[i].Add(height);
            }
        }

        Debug.Log("start");

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                var bl = new Vector3(0, heights[i][j] / 4f, 0);
                var tl = new Vector3(1, heights[i + 1][j] / 4f, 0);
                var tr = new Vector3(1, heights[i + 1][j + 1] / 4f, 1);
                var br = new Vector3(0, heights[i][j + 1] / 4f, 1);
                Vector3[] verts = GenerateVerts(bl, tl, tr, br);
                tm.AddStuff(terrainTile, i, j, verts);
                /*
                int[] tris = {0,1,2, 3,4,5};
				Vector2[] uvs = new Vector2[] { };
				GameObject t = Instantiate(terrainTile, new Vector3(i,0,j), Quaternion.identity, transform);
				Mesh mesh = new Mesh();
				t.GetComponent<MeshFilter>().mesh = mesh;
				mesh.vertices = verts;
				mesh.triangles = tris;
				mesh.uv = uvs;
				mesh.RecalculateNormals();
                */
            }
        }
        TileGraph graph = new TileGraph();
        Debug.Log("graph size: " + graph.getsize());
        Debug.Log("done");
    }

    private Vector3[] GenerateVerts(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br)
    {
        bool direction = (bl.y == tl.y && bl.y == br.y && bl.y != tr.y) || (tr.y == tl.y && tr.y == br.y && tr.y != bl.y);
        Vector3 i0 = bl;
        Vector3 i1 = direction ? br : tr;
        Vector3 i2 = tl;
        Vector3 i3 = tr;
        Vector3 i4 = direction ? tl : bl;
        Vector3 i5 = br;

        return new Vector3[] { i0, i1, i2, i3, i4, i5 };
    }
}
