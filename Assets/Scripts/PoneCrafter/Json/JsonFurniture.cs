using System;
using PoneCrafter.Model;
using UnityEngine;

namespace PoneCrafter.Json {

[Serializable]
public class JsonFurniture : BaseJsonModel {
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
    public TagJson[] tags;
    
    [Serializable]
    public class TagJson {
        public string name;
        public string value;
    }
}

}