using PoneCrafter.Json;
using UnityEngine;
using UnityGLTF;

namespace PoneCrafter.Model {

public class Furniture : BaseModel {
    public string name;
    public string description;
    public int price;
    public ObjectCategory category;
    public bool pickupable;
    public bool sellable;
    public Vector2Int[] occupiedTiles;
    public PlacementType placementType;
    public NeedStats needStats;
    public SkillStats skillStats;
    public RequiredAge requiredAge;
    public InstantiatedGLTFObject prefab;

    public Furniture(JsonFurniture jsonFurniture, InstantiatedGLTFObject prefab) : base(jsonFurniture.GetUuid()) {
        name = jsonFurniture.name;
        description = jsonFurniture.description;
        price = jsonFurniture.price;
        category = jsonFurniture.category;
        needStats = jsonFurniture.needStats;
        skillStats = jsonFurniture.skillStats;
        requiredAge = jsonFurniture.requiredAge;
        pickupable = jsonFurniture.pickupable;
        sellable = jsonFurniture.sellable;
        occupiedTiles = jsonFurniture.occupiedTiles;
        placementType = jsonFurniture.placementType;
        this.prefab = prefab;
    }
}

}