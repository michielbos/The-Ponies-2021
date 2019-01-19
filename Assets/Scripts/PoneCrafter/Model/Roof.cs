using System;
using PoneCrafter.Json;
using UnityEngine;

namespace PoneCrafter.Model {

public class Roof {
    public Guid uuid;
    public string name;
    public string description;
    public Texture2D texture;

    public Roof(JsonRoof jsonFloor, Texture2D texture) {
        uuid = jsonFloor.GetUuid();
        name = jsonFloor.name;
        description = jsonFloor.description;
        this.texture = texture;
    }
}

}