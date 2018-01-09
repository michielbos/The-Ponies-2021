using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallData {
	public int x;
	public int y;
	public int coverFront;
	public int coverBack;

	public WallData (int x, int y, int coverFront, int coverBack) {
		this.x = x;
		this.y = y;
		this.coverFront = coverFront;
		this.coverBack = coverBack;
	}
}
