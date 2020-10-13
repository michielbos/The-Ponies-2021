using System.Collections.Generic;
using System.Linq;
using PoneCrafter.Json;
using UnityEngine;
using UnityGLTF;

namespace PoneCrafter.Model {

public class Furniture : BaseModel {
    public readonly string name;
    public readonly string description;
    public readonly int price;
    public readonly ObjectCategory category;
    public readonly bool pickupable;
    public readonly bool sellable;
    public readonly Vector2Int[] occupiedTiles;
    public readonly PlacementType placementType;
    public readonly NeedStats needStats;
    public readonly SkillStats skillStats;
    public readonly RequiredAge requiredAge;
    public readonly Dictionary<string, string> tags;
    public readonly InstantiatedGLTFObject prefab;

    /// <summary>
    /// The cutouts that are applied to walls if this is an object that is placed through a wall.
    /// </summary>
    public readonly Texture2D[] cutoutTextures;

    public Furniture(JsonFurniture jsonFurniture, InstantiatedGLTFObject prefab, Texture2D[] cutoutTextures) : base(jsonFurniture.GetUuid()) {
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
        tags = jsonFurniture.tags.ToDictionary(tag => tag.name, tag => tag.value);
        this.prefab = prefab;
        this.cutoutTextures = cutoutTextures;
    }
}

}