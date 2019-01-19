using System;
using PoneCrafter.Json;
using UnityEngine;

namespace PoneCrafter.Model {

public class Terrain {
    public Guid uuid;
    public string name;
    public string description;
    public int price;
    public Texture2D texture;

    public Terrain(JsonTerrain jsonTerrain, Texture2D texture) {
        uuid = jsonTerrain.GetUuid();
        name = jsonTerrain.name;
        description = jsonTerrain.description;
        price = jsonTerrain.price;
        this.texture = texture;
    }
}

}