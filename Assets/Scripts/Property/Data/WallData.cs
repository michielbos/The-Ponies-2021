using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallData {
	public int x;
	public int y;
	public int direction;
	public int coverFront;
	public int coverBack;

	public WallData (int x, int y, int direction, int coverFront, int coverBack) {
		this.x = x;
		this.y = y;
		this.direction = direction;
		this.coverFront = coverFront;
		this.coverBack = coverBack;
	}
}
