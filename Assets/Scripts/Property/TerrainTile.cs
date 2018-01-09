using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainTile : IPointerDownHandler
{
	public int x;
	public int y;
	public int height;
	public int type;
    Mesh mesh;
    
	TerrainTile (int x, int y, int height, int type) {
        this.x = x;
        this.y = y;
		this.height = height;
		this.type = type;
        mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 0, 1) };
        mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5 };
        mesh.uv = new Vector2[] { };
        mesh.RecalculateNormals();
    }

	TerrainTile (TerrainTileData terrainTileData) : this(terrainTileData.x,
														terrainTileData.y,
														terrainTileData.height,
														terrainTileData.type) {

	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Wasdf");
		GetComponent<MeshRenderer>().material = null;
	}
}
