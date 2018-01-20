using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Property {
	public static GameObject propertyObjectPrefab;
	public int id;
	public string name;
	public string description;
	public string streetName;
	public PropertyType propertyType;
	public List<TerrainTile> terrainTiles;
	public List<FloorTile> floorTiles;
	public List<Wall> walls;
	public List<Roof> roofs;
	public List<PropertyObject> propertyObjects;

	public Property () {
		terrainTiles = new List<TerrainTile>();
		floorTiles = new List<FloorTile>();
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
		foreach (PropertyObjectData pod in propertyData.propertyObjectDatas) {
			FurniturePreset preset = FurniturePresets.Instance.GetFurniturePreset(pod.type);
			if (preset != null) {
				propertyObjects.Add(new PropertyObject(pod, preset));
			} else {
				Debug.LogWarning("No furniture preset for id " + pod.type + ". Not loading property object " + pod.id + ".");
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

	private TerrainTileData[] CreateTerrainTileDataArray (List<TerrainTile> terrainTiles) {
		TerrainTileData[] terrainTileDataArr = new TerrainTileData[terrainTiles.Count];
		for (int i = 0; i < terrainTiles.Count; i++) {
			terrainTileDataArr[i] = terrainTiles[i].GetTerrainTileData();
		}
		return terrainTileDataArr;
	}

	private FloorTileData[] CreateFloorTileDataArray (List<FloorTile> floorTiles) {
		FloorTileData[] floorTileDataArr = new FloorTileData[floorTiles.Count];
		for (int i = 0; i < floorTiles.Count; i++) {
			floorTileDataArr[i] = floorTiles[i].GetFloorTileData();
		}
		return floorTileDataArr;
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
}
