﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that can be placed on a lot, usually a piece of furniture.
/// </summary>
[System.Serializable]
public class PropertyObject {
	public int id;
	public int x;
	public int y;
	public ObjectRotation rotation;
	public FurniturePreset preset;
	public GameObject dummyObject;
	public int value;

	public PropertyObject (int id, int x, int y, ObjectRotation rotation, FurniturePreset preset) {
		this.id = id;
		this.x = x;
		this.y = y;
		this.rotation = rotation;
		this.preset = preset;
		value = preset.price;
	}

	public PropertyObject (PropertyObjectData pod, FurniturePreset preset) {
		id = pod.id;
		x = pod.x;
		y = pod.y;
		this.preset = preset; 
		rotation = (ObjectRotation) pod.rotation;
		value = pod.value;
	}

	public PropertyObjectData GetPropertyObjectData () {
		return new PropertyObjectData(id,
			x,
			y,
			(int) rotation,
			preset.id,
			value);
	}

	/// <summary>
	/// Place a dummy of this object in the scene.
	/// </summary>
	/// <param name="propertyObjectPrefab">The prefab to instantiate.</param>
	public void PlaceObject (GameObject propertyObjectPrefab) {
		dummyObject = Object.Instantiate(propertyObjectPrefab, new Vector3(x + 0.5f, 0, y + 0.5f), Quaternion.Euler(0, 0, 0));
		dummyObject.GetComponent<PropertyObjectDummy>().propertyObject = this;
	}

	/// <summary>
	/// Refresh the dummy object, updating its position to match the real PropertyObject.
	/// </summary>
	public void RefreshDummy () {
		dummyObject.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
	}

	/// <summary>
	/// Remove the dummy object from the scene.
	/// </summary>
	public void RemoveObject () {
		Object.Destroy(dummyObject);
	}
}
