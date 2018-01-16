using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyController : MonoBehaviour {
	public GameObject propertyObjectPrefab;
	private bool loaded = false;
	public Property property;

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

}
