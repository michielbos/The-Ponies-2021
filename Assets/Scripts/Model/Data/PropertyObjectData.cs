using System;
using Model.Property;

[Serializable]
public class PropertyObjectData {
	public int id;
	public int x;
	public int y;
	public int rotation;
	public string furnitureGuid;
	public int skin;
	public int value;

	public PropertyObjectData (int id, int x, int y, int rotation, string furnitureGuid, int skin, int value) {
		this.id = id;
		this.x = x;
		this.y = y;
		this.rotation = rotation;
		this.furnitureGuid = furnitureGuid;
		this.skin = skin;
		this.value = value;
	}

	public ObjectRotation GetObjectRotation() {
		return (ObjectRotation) rotation;
	}
}
