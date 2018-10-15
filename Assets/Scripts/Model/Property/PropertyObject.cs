﻿using UnityEngine;

/// <summary>
/// An object that can be placed on a lot, usually a piece of furniture.
/// The physical "dummy" of this object is kept in the dummyObject attribute.
/// </summary>
[System.Serializable]
public class PropertyObject {
	public int id;
	public int x;
	public int y;
	public ObjectRotation rotation;
	public FurniturePreset preset;
	public int skin;
	public GameObject dummyObject;
	public int value;

	public PropertyObject (int id, int x, int y, ObjectRotation rotation, FurniturePreset preset, int skin) {
		this.id = id;
		this.x = x;
		this.y = y;
		this.rotation = rotation;
		this.preset = preset;
		this.skin = skin;
		value = preset.price;
	}

	public PropertyObject (PropertyObjectData pod, FurniturePreset preset) {
		id = pod.id;
		x = pod.x;
		y = pod.y;
		this.preset = preset;
		skin = pod.skin;
		rotation = (ObjectRotation) pod.rotation;
		value = pod.value;
	}

	public PropertyObjectData GetPropertyObjectData () {
		return new PropertyObjectData(id,
			x,
			y,
			(int) rotation,
			preset.guid.ToString(),
			skin,
			value);
	}

	/// <summary>
	/// Place a dummy of this object in the scene.
	/// </summary>
	/// <param name="prefab">The property object prefab to instantiate.</param>
	public void PlaceObject (GameObject prefab) {
		dummyObject = preset.PlaceObject(prefab, new Vector3(x + 0.5f, 0, y + 0.5f), skin);
		dummyObject.GetComponent<PropertyObjectDummy>().propertyObject = this;
	}

	/// <summary>
	/// Refresh the dummy object, updating its position to match the real PropertyObject.
	/// </summary>
	public void RefreshDummy () {
		dummyObject.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
		dummyObject.transform.eulerAngles = ObjectRotationUtil.GetRotationVector(rotation);
		preset.ApplyOffsets(dummyObject.transform);
		preset.AdjustToTiles(dummyObject.transform);
	}

	/// <summary>
	/// Remove the dummy object from the scene.
	/// </summary>
	public void RemoveObject () {
		Object.Destroy(dummyObject);
	}

	/// <summary>
	/// Get the coordinates of the tiles occupied by this PropertyObject.
	/// </summary>
	/// <returns>A Vector2Int array of all occupied coordinates.</returns>
	public Vector2Int[] GetOccupiedTiles () {
		return preset.GetOccupiedTiles(new Vector2Int(x, y));
	}
}