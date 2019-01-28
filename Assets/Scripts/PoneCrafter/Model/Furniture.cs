using System;
using PoneCrafter.Json;
using UnityEngine;

namespace PoneCrafter.Model {

public class Furniture : BaseModel {
    public string name;
    public string description;
    public int price;
    public ObjectCategory category;
    public NeedStats needStats;
    public SkillStats skillStats;
    public RequiredAge requiredAge;
    public Mesh mesh;
    public Texture2D texture;

    public Furniture(JsonFurniture jsonFurniture, Mesh mesh, Texture2D texture) : base(jsonFurniture.GetUuid()) {
        name = jsonFurniture.name;
        description = jsonFurniture.description;
        price = jsonFurniture.price;
        category = jsonFurniture.category;
        needStats = jsonFurniture.needStats;
        skillStats = jsonFurniture.skillStats;
        requiredAge = jsonFurniture.requiredAge;
        this.mesh = mesh;
        this.texture = texture;
    }
}

}