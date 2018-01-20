using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[System.Serializable]
public class FurniturePreset {
	public int id;
	public string name;
	public string description;
	public int price;
	public ObjectCategory category;

	public FurniturePreset (int id, string name, string description, int price, ObjectCategory category) {
		this.id = id;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
	}

	public FurniturePreset (FurniturePresetData furniturePresetData) 
		: this(furniturePresetData.id,
			furniturePresetData.name,
			furniturePresetData.description,
			furniturePresetData.price,
			(ObjectCategory) furniturePresetData.category) {

	}

	public FurniturePresetData GetFurniturePresetData () {
		return new FurniturePresetData(id, name, description, price, (int) category);
	}
}
