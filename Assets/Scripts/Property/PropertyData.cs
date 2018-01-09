using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyData {
	public string name;
	public string streetName;
	public string description;
	public PropertyType propertyType;
	public List<TerrainTile> terrainTiles;
	public List<Wall> walls;
	public List<Roof> roofs;
	public List<PropertyObject> propertyObjects;

	PropertyData () {
		terrainTiles = new List<TerrainTile>();
		walls = new List<Wall>();
		roofs = new List<Roof>();
		propertyObjects = new List<PropertyObject>();
	}
}
