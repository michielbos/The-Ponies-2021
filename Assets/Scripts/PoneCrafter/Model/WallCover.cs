using PoneCrafter.Json;
using UnityEngine;

namespace PoneCrafter.Model {

public class WallCover : BaseModel {
    public string name;
    public string description;
    public int price;
    public Texture2D texture;

    public WallCover(JsonWallCover jsonWallCover, Texture2D texture) : base(jsonWallCover.GetUuid()) {
        name = jsonWallCover.name;
        description = jsonWallCover.description;
        price = jsonWallCover.price;
        this.texture = texture;
    }
}

}