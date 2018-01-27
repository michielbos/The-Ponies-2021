﻿using System;
using UnityEngine;

[Serializable]
public class FurniturePresetData {
	public string guid;
	public string name;
	public string description;
	public int price;
	public int category;
	public bool pickupable;
	public bool sellable;
	public string modelName;
	public string[] materialPaths;
	public Vector3 rotationOffset;
	public Vector3 positionOffset;

	public FurniturePresetData (string guid, string name, string description, int price, int category, bool pickupable,
		bool sellable, string modelName, string[] materialPaths, Vector3 rotationOffset, Vector3 positionOffset) {
		this.guid = guid;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
		this.pickupable = pickupable;
		this.sellable = sellable;
		this.modelName = modelName;
		this.materialPaths = materialPaths;
		this.rotationOffset = rotationOffset;
		this.positionOffset = positionOffset;
	}
}
