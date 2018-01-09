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

	public PropertyData (int id, string name, string description, string streetName, int propertyType) {
		this.id = id;
		this.name = name;
		this.description = description;
		this.streetName = streetName;
		this.propertyType = propertyType;
	}
}
