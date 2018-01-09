using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyObject {
	public int x;
	public int y;
	public int type;

	public PropertyObject (int x, int y, int type) {
		this.x = x;
		this.y = y;
		this.type = type;
	}

	public PropertyObject (PropertyObjectData propertyObjectData) 
			: this(propertyObjectData.x,
			propertyObjectData.y,
			propertyObjectData.type) {

	}
		
}
