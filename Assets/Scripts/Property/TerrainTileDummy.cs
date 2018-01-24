using UnityEngine;

public class TerrainTileDummy : MonoBehaviour {
	public TerrainTile terrainTile;
    
	private void Start () {
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 0, 1) };
        mesh.triangles = new int[] { 0, 1, 2, 3, 4, 5 };
        mesh.uv = new Vector2[] { };
        mesh.RecalculateNormals();
		GetComponent<MeshFilter>().mesh = mesh;
	}
	
}
