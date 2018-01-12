using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyController : MonoBehaviour {
	bool loaded = false;
	public Property property;

	public void Initialize (int propertyId) {
		loaded = true;
		property = new PropertyLoader().LoadOrCreateProperty(propertyId);
	}

	public void SaveProperty () {
		new PropertyLoader().SaveProperty(property);
	}

}
