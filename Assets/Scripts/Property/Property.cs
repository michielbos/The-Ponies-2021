﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property {
	public int id;
	public string name;
	public string description;
	public string streetName;
	public PropertyType propertyType;
	public List<TerrainTile> terrainTiles;
	public List<Wall> walls;
	public List<Roof> roofs;
	public List<PropertyObject> propertyObjects;

	public Property () {
		terrainTiles = new List<TerrainTile>();
		walls = new List<Wall>();
		roofs = new List<Roof>();
		propertyObjects = new List<PropertyObject>();
	}

	public Property (int id, string name, string description, string streetName, PropertyType propertyType) : this() {
		this.id = id;
		this.name = name;
		this.streetName = streetName;
		this.description = description;
	}

	public Property (PropertyData propertyData) : this(propertyData.id,
												propertyData.name,
												propertyData.description,
												propertyData.streetName,
												propertyData.propertyType == 0 ? PropertyType.RESIDENTIAL : PropertyType.COMMUNITY) {

	}

	public PropertyData GetPropertyData () {
		return new PropertyData(id, name, description, streetName, propertyType == PropertyType.RESIDENTIAL ? 0 : 1);
	}
}
