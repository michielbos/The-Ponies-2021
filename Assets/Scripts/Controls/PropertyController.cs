using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyController : MonoBehaviour {
	public GameObject propertyObjectPrefab;
	public Property property;
	private bool loaded = false;
	//TODO: Set on load.
	private int nextObjectId = 0;

	public void Initialize (int propertyId) {
		loaded = true;
		property = new PropertyLoader().LoadOrCreateProperty(propertyId);
		PlacePropertyObjects();
	}

	public void SaveProperty () {
		new PropertyLoader().SaveProperty(property);
	}

	public void PlacePropertyObjects () {
		foreach (PropertyObject po in property.propertyObjects) {
			po.PlaceObject(propertyObjectPrefab);
		}
	}

	public void PlacePropertyObject (int x, int y, ObjectRotation objectRotation, int type) {
		PropertyObject propertyObject = new PropertyObject(nextObjectId++, x, y, objectRotation, type);
		property.propertyObjects.Add(propertyObject);
		propertyObject.PlaceObject(propertyObjectPrefab);
	}

}
