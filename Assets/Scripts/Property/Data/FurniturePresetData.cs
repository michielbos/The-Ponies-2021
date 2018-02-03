using UnityEngine;

[System.Serializable]
public class FurniturePresetData {
	public string guid;
	public string name;
	public string description;
	public int price;
	public int category;
	public bool pickupable;
	public bool sellable;
	public string modelName;
	public FurnitureSkinData[] furnitureSkins;
	public Vector3 rotationOffset;
	public Vector3 positionOffset;
	public Vector2Int[] occupiedTiles;
	public NeedStats needStats;
	public SkillStats skillStats;
	public PonyAge? requiredAge;

	public FurniturePresetData (string guid, string name, string description, int price, int category, bool pickupable,
		bool sellable, string modelName, FurnitureSkinData[] furnitureSkins, Vector3 rotationOffset, Vector3 positionOffset,
		Vector2Int[] occupiedTiles, NeedStats needStats, SkillStats skillStats, PonyAge? requiredAge) {
		this.guid = guid;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
		this.pickupable = pickupable;
		this.sellable = sellable;
		this.modelName = modelName;
		this.furnitureSkins = furnitureSkins;
		this.rotationOffset = rotationOffset;
		this.positionOffset = positionOffset;
		this.occupiedTiles = occupiedTiles;
		this.needStats = needStats;
		this.skillStats = skillStats;
		this.requiredAge = requiredAge;
	}
}
