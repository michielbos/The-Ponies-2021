using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurniturePreset {
	public int id;
	public string name;
	public string description;
	public int price;

	public FurniturePreset (int id, string name, string description, int price) {
		this.id = id;
		this.name = name;
		this.description = description;
		this.price = price;
	}

	public FurniturePreset (FurniturePresetData furniturePresetData) 
		: this(furniturePresetData.id,
			furniturePresetData.name,
			furniturePresetData.description,
			furniturePresetData.price) {

	}

	public FurniturePresetData GetPropertyObjectData () {
		return new FurniturePresetData(id, name, description, price);
	}
}
