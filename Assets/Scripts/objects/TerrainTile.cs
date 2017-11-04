using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainTile : MonoBehaviour, IPointerDownHandler
{
	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;
    Mesh mesh;
    
    TerrainTile(int x,int y)
    {
        this.x = x;
        this.y = y;
        mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 0, 1) };
        mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5 };
        mesh.uv = new Vector2[] { };
        mesh.RecalculateNormals();
        
    }
	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Wasdf");
		GetComponent<MeshRenderer>().material = null;
	}
}
