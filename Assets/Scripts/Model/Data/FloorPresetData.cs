
[System.Serializable]
public class FloorPresetData {
	public string guid;
	public string name;
	public string description;
	public int price;
	public string textureName;

	public FloorPresetData (string guid, string name, string description, int price, string textureName) {
		this.guid = guid;
		this.name = name;
		this.description = description;
		this.price = price;
		this.textureName = textureName;
	}
}
