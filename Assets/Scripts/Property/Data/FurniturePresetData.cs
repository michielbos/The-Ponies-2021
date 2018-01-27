using System;
using UnityEngine;

[Serializable]
public class FurniturePresetData {
	public string guid;
	public string name;
	public string description;
	public int price;
	public int category;
	public string modelName;
	public string[] materialPaths;
	public Vector3 addRotation;

	public FurniturePresetData (string guid, string name, string description, int price, int category, string modelName, 
		string[] materialPaths, Vector3 addRotation) {
		this.guid = guid;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
		this.modelName = modelName;
		this.materialPaths = materialPaths;
		this.addRotation = addRotation;
	}
}
