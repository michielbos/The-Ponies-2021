using UnityEngine;

/// <summary>
/// A wall that can be placed on the border of a tile.
/// The physical "dummy" of this wall is kept in the dummyWall attribute.
/// </summary>
[System.Serializable]
public class Wall {
    public int x;
    public int y;
	public WallDirection wallDirection;
	public GameObject dummyWall;

	public Wall (int x, int y, WallDirection wallDirection) {
		this.x = x;
		this.y = y;
		this.wallDirection = wallDirection;
	}
	
	public Wall (WallData wallData) : this(wallData.x,
		wallData.y,
		(WallDirection) wallData.direction) {

	}

	public WallData GetWallData () {
		return new WallData(x,
			y,
			(int)wallDirection,
			-1,
			-1);
	}
	
	/// <summary>
	/// Place a dummy of this object in the scene.
	/// </summary>
	/// <param name="prefab">The wall dummy prefab to instantiate.</param>
	public void PlaceWall (GameObject prefab) {
		//TODO: Put the wall in the right position and rotation.
		dummyWall = Object.Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity);
		dummyWall.GetComponent<WallDummy>().wall = this;
	}

	/// <summary>
	/// Refresh the dummy wall, updating its position to match the real Wall.
	/// </summary>
	public void RefreshDummy () {
		//TODO: Put the wall in the right position and rotation.
		dummyWall.transform.position = new Vector3(x, 0, y);
	}

	/// <summary>
	/// Remove the dummy wall from the scene.
	/// </summary>
	public void RemoveWall () {
		Object.Destroy(dummyWall);
	}
}
