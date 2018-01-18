using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropertyObject {
	public int id;
	public int x;
	public int y;
	public ObjectRotation rotation;
	public int type;
	public GameObject gameObject;

	public PropertyObject (int id, int x, int y, ObjectRotation rotation, int type) {
		this.id = id;
		this.x = x;
		this.y = y;
		this.rotation = rotation;
		this.type = type;
	}

	public PropertyObject (PropertyObjectData propertyObjectData) 
		: this(propertyObjectData.id,
			propertyObjectData.x,
			propertyObjectData.y,
			(ObjectRotation) propertyObjectData.rotation,
			propertyObjectData.type) {

	}

	public PropertyObjectData GetPropertyObjectData () {
		return new PropertyObjectData(id,
			x,
			y,
			(int) rotation,
			type);
	}

	public void PlaceObject (GameObject propertyObjectPrefab) {
		gameObject = GameObject.Instantiate(propertyObjectPrefab, new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(0, 0, 0));
	}
}
