using System;
using PoneCrafter.Json;
using UnityEngine;

namespace PoneCrafter.Model {

public class Floor : BaseModel {
    public string name;
    public string description;
    public int price;
    public Texture2D texture;

    public Floor(JsonFloor jsonFloor, Texture2D texture) : base(jsonFloor.GetUuid()) {
        name = jsonFloor.name;
        description = jsonFloor.description;
        price = jsonFloor.price;
        this.texture = texture;
    }
}

}