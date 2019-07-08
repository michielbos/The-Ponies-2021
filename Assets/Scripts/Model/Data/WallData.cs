using Model.Property;

[System.Serializable]
public class WallData {
	public int x;
	public int y;
	public int direction;
	public string coverFrontUuid;
	public string coverBackUuid;

	public WallDirection Direction => (WallDirection) direction;

	public WallData (int x, int y, int direction, string coverFrontUuid, string coverBackUuid) {
		this.x = x;
		this.y = y;
		this.direction = direction;
		this.coverFrontUuid = coverFrontUuid;
		this.coverBackUuid = coverBackUuid;
	}
}
