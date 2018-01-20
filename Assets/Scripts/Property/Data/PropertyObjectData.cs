using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PropertyObjectData {
	public int id;
	public int x;
	public int y;
	public int rotation;
	public int type;
	public int value;

	public PropertyObjectData (int id, int x, int y, int rotation, int type, int value) {
		this.id = id;
		this.x = x;
		this.y = y;
		this.rotation = rotation;
		this.type = type;
		this.value = value;
	}
}
