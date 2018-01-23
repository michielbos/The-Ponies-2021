using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurniturePresetData {
	public int id;
	public string name;
	public string description;
	public int price;
	public int category;
	public string modelName;
	public string[] materialPaths;
	public Vector3 addRotation;

	public FurniturePresetData (int id, string name, string description, int price, int category, string modelName, 
		string[] materialPaths, Vector3 addRotation) {
		this.id = id;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
		this.modelName = modelName;
		this.materialPaths = materialPaths;
		this.addRotation = addRotation;
	}
}
