using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile {
	public int x;
	public int y;
	public int type;

	public FloorTile (int x, int y, int type) {
		this.x = x;
		this.y = y;
		this.type = type;
	}

	public FloorTile (FloorTileData floorTileData) : this (floorTileData.x,
		                                                floorTileData.y,
														floorTileData.type) {

	}
}
