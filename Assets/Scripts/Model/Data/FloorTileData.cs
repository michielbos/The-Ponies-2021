using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorTileData {
	public int x;
	public int y;
	public int type;

	public FloorTileData (int x, int y, int type) {
		this.x = x;
		this.y = y;
		this.type = type;
	}
}
