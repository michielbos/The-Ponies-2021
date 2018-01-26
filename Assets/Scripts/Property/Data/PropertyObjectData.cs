using System;

[Serializable]
public class PropertyObjectData {
	public int id;
	public int x;
	public int y;
	public int rotation;
	public string furnitureGuid;
	public int value;

	public PropertyObjectData (int id, int x, int y, int rotation, string furnitureGuid, int value) {
		this.id = id;
		this.x = x;
		this.y = y;
		this.rotation = rotation;
		this.furnitureGuid = furnitureGuid;
		this.value = value;
	}
}
