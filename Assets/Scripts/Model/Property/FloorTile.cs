
using UnityEngine;

[System.Serializable]
public class FloorTile {
	public int x;
	public int y;
	public FloorPreset preset;
	public FloorTileDummy dummyObject;

	public FloorTile (int x, int y, FloorPreset preset) {
		this.x = x;
		this.y = y;
		this.preset = preset;
	}

	public FloorTile (FloorTileData floorTileData, FloorPreset preset) : this (floorTileData.x,
		                                                floorTileData.y,
														preset) {

	}
	
	public FloorTileData GetFloorTileData () {
		return new FloorTileData(x,
			y,
			preset.guid.ToString());
	}
	
	/// <summary>
	/// Place a dummy of this floor in the scene.
	/// </summary>
	/// <param name="prefab">The floor tile dummy prefab to instantiate.</param>
	public void PlaceFloor (GameObject prefab) {
		dummyObject = preset.PlaceFloor(prefab, new Vector3(x + 0.5f, 0, y + 0.5f));
		dummyObject.GetComponent<FloorTileDummy>().floorTile = this;
		preset.ApplyToGameObject(dummyObject.gameObject);
	}

	/// <summary>
	/// Refresh the dummy, updating its position to match this FloorTile.
	/// </summary>
	public void RefreshDummy () {
		dummyObject.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
	}

	/// <summary>
	/// Remove the dummy floor from the scene.
	/// </summary>
	public void RemoveFloor () {
		Object.Destroy(dummyObject.gameObject);
	}
}
