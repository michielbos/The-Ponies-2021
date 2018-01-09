using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTileData {
	public int x;
	public int y;
	public int height;
	public int type;

	public TerrainTileData (int x, int y, int height, int type) {
		this.x = x;
		this.y = y;
		this.height = height;
		this.type = type;
	}
}
