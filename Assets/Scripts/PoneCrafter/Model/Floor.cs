using System;
using PoneCrafter.Json;

namespace PoneCrafter.Model {

public class Floor {
    public Guid uuid;
    public string name;
    public string description;
    public int price;

    public Floor(JsonFloor jsonFloor) {
        uuid = jsonFloor.GetUuid();
        name = jsonFloor.name;
        description = jsonFloor.description;
        price = jsonFloor.price;
    }
}

}