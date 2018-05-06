using System;
using Assets.Scripts.Util;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// A tile that is part of the terrain.
/// The physical "dummy" of this object is kept in the dummyTile attribute.
/// </summary>
[Serializable]
public class TerrainTile {
	public int x;
	public int y;
	public int height;
	public int type;
	public GameObject dummyTile;

    [NonSerialized]
    private static Transform tileRoot;
    
	public TerrainTile (int x, int y, int height, int type) {
        this.x = x;
        this.y = y;
		this.height = height;
		this.type = type;
    }

	public TerrainTile (TerrainTileData terrainTileData) : this(terrainTileData.x,
														terrainTileData.y,
														terrainTileData.height,
														terrainTileData.type) {

	}

	public TerrainTileData GetTerrainTileData () {
		return new TerrainTileData(x,
			y,
			height,
			type);
	}
	
	/// <summary>
	/// Place a dummy of this terrain tile in the scene.
	/// </summary>
	/// <param name="prefab">The terrain tile prefab to instantiate.</param>
	public void PlaceTile (GameObject prefab) {
	    if (!tileRoot.Exists())
	    {
	        tileRoot = new GameObject("TileRoot").transform;
	    }

		dummyTile = Object.Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity);
        dummyTile.transform.SetParent(tileRoot, true);
		dummyTile.GetComponent<TerrainTileDummy>().terrainTile = this;
	}

	/// <summary>
	/// Refresh the dummy tile, updating its position to match the real TerrainTile.
	/// </summary>
	public void RefreshDummy () {
		dummyTile.transform.position = new Vector3(x, 0, y);
	}

	/// <summary>
	/// Remove the dummy tile from the scene.
	/// </summary>
	public void RemoveTile () {
		Object.Destroy(dummyTile);
	}
}
