using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Property {
	public int id;
	public string name;
	public string description;
	public string streetName;
	public PropertyType propertyType;
	public TerrainTile[,] terrainTiles;
	public FloorTile[,,] floorTiles;
	public List<Wall> walls;
	public List<Roof> roofs;
	public List<PropertyObject> propertyObjects;

	public Property () {
		walls = new List<Wall>();
		roofs = new List<Roof>();
		propertyObjects = new List<PropertyObject>();
	}

	public Property (int id, string name, string description, string streetName, PropertyType propertyType) : this() {
		this.id = id;
		this.name = name;
		this.streetName = streetName;
		this.description = description;
		this.propertyType = propertyType;
	}

	public Property (PropertyData propertyData) : this(propertyData.id,
												propertyData.name,
												propertyData.description,
												propertyData.streetName,
												propertyData.propertyType == 0 ? PropertyType.RESIDENTIAL : PropertyType.COMMUNITY) {
		LoadTerrainTiles(propertyData.terrainTileDatas);
		LoadWalls(propertyData.wallDatas);
		LoadFloorTiles(propertyData.floorTileDatas);
		LoadPropertyObjects(propertyData.propertyObjectDatas);
	}

	private void LoadTerrainTiles (TerrainTileData[] terrainTileDatas) {
		int width = 0;
		int height = 0;
		foreach (TerrainTileData ttd in terrainTileDatas) {
			if (ttd.x >= width) {
				width = ttd.x + 1;
			}
			if (ttd.y >= height) {
				height = ttd.y + 1;
			}
		}
		terrainTiles = new TerrainTile[height,width];
		foreach (TerrainTileData ttd in terrainTileDatas) {
			if (terrainTiles[ttd.y, ttd.x] == null) {
				terrainTiles[ttd.y, ttd.x] = new TerrainTile(ttd);
			} else {
				Debug.LogWarning("There is already a terrain tile for (" + ttd.x + ", " + ttd.y + "). Not loading another one.");
			}
		}
	}

	private void LoadWalls (WallData[] wallDatas) {
		foreach (WallData wd in wallDatas) {
			walls.Add(new Wall(wd));
		}
	}
	
	private void LoadFloorTiles (FloorTileData[] floorTileDatas) {
		int width = terrainTiles.GetLength(1);
		int height = terrainTiles.GetLength(0);
		floorTiles = new FloorTile[1,height,width];
		//TODO: Floor levels
		foreach (FloorTileData ftd in floorTileDatas) {
			if (floorTiles[0, ftd.y, ftd.x] == null) {
				try {
					FloorPreset preset = FloorPresets.Instance.GetFloorPreset(new Guid(ftd.floorGuid));
					if (preset != null) {
						floorTiles[0, ftd.y, ftd.x] = new FloorTile(ftd, preset);
					} else {
						Debug.LogWarning("No floor preset for GUID " + ftd.floorGuid + ". Not loading floor at (" + ftd.x + ", " + ftd.y + ").");
					}
				} catch (Exception e) {
					Debug.LogError("Exception when trying to load floor at (" + ftd.x + ", " + ftd.y + ")! Not loading floor.");
					Debug.LogException(e);
				}
			} else {
				Debug.LogWarning("There is already a floor tile for (" + ftd.x + ", " + ftd.y + "). Not loading another one.");
			}
		}
	}

	private void LoadPropertyObjects (PropertyObjectData[] propertyObjectDatas) {
		foreach (PropertyObjectData pod in propertyObjectDatas) {
			try {
				FurniturePreset preset = FurniturePresets.Instance.GetFurniturePreset(new Guid(pod.furnitureGuid));
				if (preset != null) {
					propertyObjects.Add(new PropertyObject(pod, preset));
				} else {
					Debug.LogWarning("No furniture preset for GUID " + pod.furnitureGuid + ". Not loading property object " + pod.id + ".");
				}
			} catch (Exception e) {
				Debug.LogError("Exception when trying to load property object with id " + pod.id + "! Not loading property object.");
				Debug.LogException(e);
			}
		}
	}

	public PropertyData GetPropertyData () {
		return new PropertyData(id,
			name,
			description,
			streetName,
			propertyType == PropertyType.RESIDENTIAL ? 0 : 1,
			CreateTerrainTileDataArray(terrainTiles),
			CreateFloorTileDataArray(floorTiles),
			CreateWallDataArray(walls),
			CreateRoofDataArray(roofs),
			CreatePropertyObjectDataArray(propertyObjects));
	}

	private TerrainTileData[] CreateTerrainTileDataArray (TerrainTile[,] terrainTiles) {
		TerrainTileData[] terrainTileDataArr = new TerrainTileData[terrainTiles.Length];
		for (int y = 0; y < terrainTiles.GetLength(0); y++) {
			for (int x = 0; x < terrainTiles.GetLength(1); x++) {
				if (terrainTiles[y, x] != null) {
					terrainTileDataArr[y * terrainTiles.GetLength(1) + x] = terrainTiles[y, x].GetTerrainTileData();
				}
			}
		}
		return terrainTileDataArr;
	}

	private FloorTileData[] CreateFloorTileDataArray (FloorTile[,,] floorTiles) {
		int floorCount = GetNumberOfFloors();
		FloorTileData[] floorTileDataArr = new FloorTileData[floorCount];
		int index = 0;
		for (int h = 0; h < floorTiles.GetLength(0); h++) {
			for (int y = 0; y < floorTiles.GetLength(1); y++) {
				for (int x = 0; x < floorTiles.GetLength(2); x++) {
					if (floorTiles[h, y, x] != null) {
						floorTileDataArr[index++] = floorTiles[h, y, x].GetFloorTileData();
					}
				}
			}
		}
		return floorTileDataArr;
	}

	private int GetNumberOfFloors () {
		int floorCount = 0;
		for (int h = 0; h < floorTiles.GetLength(0); h++) {
			for (int y = 0; y < floorTiles.GetLength(1); y++) {
				for (int x = 0; x < floorTiles.GetLength(2); x++) {
					if (floorTiles[h, y, x] != null) {
						floorCount++;
					}
				}
			}
		}
		return floorCount;
	}

	private WallData[] CreateWallDataArray (List<Wall> walls) {
		WallData[] wallDataArr = new WallData[walls.Count];
		for (int i = 0; i < walls.Count; i++) {
			wallDataArr[i] = walls[i].GetWallData();
		}
		return wallDataArr;
	}

	private RoofData[] CreateRoofDataArray (List<Roof> roofs) {
		RoofData[] roofDataArr = new RoofData[roofs.Count];
		for (int i = 0; i < roofs.Count; i++) {
			roofDataArr[i] = roofs[i].GetRoofData();
		}
		return roofDataArr;
	}

	private PropertyObjectData[] CreatePropertyObjectDataArray (List<PropertyObject> propertyObjects) {
		PropertyObjectData[] propertyObjectDataArr = new PropertyObjectData[propertyObjects.Count];
		for (int i = 0; i < propertyObjects.Count; i++) {
			propertyObjectDataArr[i] = propertyObjects[i].GetPropertyObjectData();
		}
		return propertyObjectDataArr;
	}
	
	/// <summary>
	/// Get all property objects that have a tile overlapping the given position.
	/// </summary>
	/// <param name="x">The X coordinate of the tile.</param>
	/// <param name="y">The Y coordinate of the tile.</param>
	/// <returns>A list of all PropertyObjects with a tile overlapping the given X and Y.</returns>
	public List<PropertyObject> GetObjectsOnTile (int x, int y) {
		return GetObjectsOnTiles(new Vector2Int[] {new Vector2Int(x, y)});
	}
	
	/// <summary>
	/// Get all property objects that have a tile overlapping one of the given positions.
	/// </summary>
	/// <param name="tiles">The coordinates of the tiles.</param>
	/// <returns>A list of all PropertyObjects with a tile overlapping at least one of the given positions.</returns>
	public List<PropertyObject> GetObjectsOnTiles (Vector2Int[] tiles) {
		//This might come with a performance overhead when there are a lot of objects.
		//If performance becomes an issue, we could remember all overlapping objects inside each terrain tile.
		List<PropertyObject> objectsOnTiles = new List<PropertyObject>();
		foreach (PropertyObject propertyObject in propertyObjects) {
			foreach (Vector2Int occupiedTile in propertyObject.GetOccupiedTiles()) {
				bool overlaps = false;
				foreach (Vector2Int tile in tiles) {
					if (occupiedTile.x == tile.x && occupiedTile.y == tile.y) {
						objectsOnTiles.Add(propertyObject);
						overlaps = true;
						break;
					}
				}
				if (overlaps)
					break;
			}
		}
		return objectsOnTiles;
	}

	public FloorTile GetFloorTile (int x, int y) {
		//TODO: Add floor level
		return floorTiles[0, y, x];
	}
}

