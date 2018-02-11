
[System.Serializable]
public class FloorTileData {
	public int x;
	public int y;
	public string floorGuid;

	public FloorTileData (int x, int y, string floorGuid) {
		this.x = x;
		this.y = y;
		this.floorGuid = floorGuid;
	}
}
