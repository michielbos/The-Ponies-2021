using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropertyData {
	public int id;
	public string name;
	public string description;
	public string streetName;
	public int propertyType;
	public TerrainTileData[] terrainTileDatas;
	public FloorTileData[] floorTileDatas;
	public WallData[] wallDatas;
	public RoofData[] roofDatas;
	public PropertyObjectData[] propertyObjectDatas;

	public PropertyData (int id, string name, string description, string streetName, int propertyType, TerrainTileData[] terrainTileDatas, 
			FloorTileData[] floorTileDatas, WallData[] wallDatas, RoofData[] roofDatas, PropertyObjectData[] propertyObjectDatas) {
		this.id = id;
		this.name = name;
		this.description = description;
		this.streetName = streetName;
		this.propertyType = propertyType;
		this.terrainTileDatas = terrainTileDatas;
		this.floorTileDatas = floorTileDatas;
		this.wallDatas = wallDatas;
		this.roofDatas = roofDatas;
		this.propertyObjectDatas = propertyObjectDatas;
	}

}
