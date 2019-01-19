using System;
using PoneCrafter.Json;
using UnityEngine;

namespace PoneCrafter.Model {

public class Roof : BaseModel {
    public Guid uuid;
    public string name;
    public string description;
    public Texture2D texture;

    public Roof(JsonRoof jsonRoof, Texture2D texture) : base(jsonRoof.GetUuid()) {
        name = jsonRoof.name;
        description = jsonRoof.description;
        this.texture = texture;
    }
}

}