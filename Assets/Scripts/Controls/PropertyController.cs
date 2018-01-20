using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for managing the currently loaded lot.
/// </summary>
public class PropertyController : MonoBehaviour {
	public GameObject propertyObjectPrefab;
	public Property property;
	private int nextObjectId;

	/// <summary>
	/// Called when the scene is just started and a property should be loaded.
	/// </summary>
	/// <param name="propertyId">The id of the property to load.</param>
	public void Initialize (int propertyId) {
		property = new PropertyLoader().LoadOrCreateProperty(propertyId);
		foreach (PropertyObject propertyObject in property.propertyObjects) {
			if (propertyObject.id >= nextObjectId) {
				nextObjectId = propertyObject.id + 1;
			}
		}
		PlacePropertyObjects();
	}

	/// <summary>
	/// Save the currently open property.
	/// </summary>
	public void SaveProperty () {
		new PropertyLoader().SaveProperty(property);
	}

	/// <summary>
	/// Place "dummy" instance of all loaded property objects, so they are visible to the player.
	/// </summary>
	public void PlacePropertyObjects () {
		foreach (PropertyObject po in property.propertyObjects) {
			po.PlaceObject(propertyObjectPrefab);
		}
	}

	/// <summary>
	/// Add a new property object to the property. This will also instantiate a dummy to make the object visible.
	/// </summary>
	/// <param name="x">The X position of the object.</param>
	/// <param name="y">The Y position of the object.</param>
	/// <param name="objectRotation">The rotation of the object.</param>
	/// <param name="preset">The FurniturePreset that this object is based on.</param>
	public void PlacePropertyObject (int x, int y, ObjectRotation objectRotation, FurniturePreset preset) {
		PropertyObject propertyObject = new PropertyObject(nextObjectId++, x, y, objectRotation, preset);
		property.propertyObjects.Add(propertyObject);
		propertyObject.PlaceObject(propertyObjectPrefab);
	}

	/// <summary>
	/// Remove a property object from the property. This will also clean up the dummy object in the scene.
	/// </summary>
	/// <param name="propertyObject"></param>
	public void RemovePropertyObject (PropertyObject propertyObject) {
		propertyObject.RemoveObject();
		property.propertyObjects.Remove(propertyObject);
	}
}
