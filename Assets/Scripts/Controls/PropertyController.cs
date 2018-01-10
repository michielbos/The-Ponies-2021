using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyController : MonoBehaviour {
	bool loaded = false;
	Property property;

	public void Initialize (int propertyId) {
		Debug.Log(propertyId);
		loaded = true;
		property = new PropertyLoader().LoadOrCreateProperty(propertyId);
	}

	// Use this for initialization
	void Start () {
		if (!loaded) {
			Debug.Log("Started directly from scene, loading lot 0.");
			Initialize(0);
		}
	}

}
