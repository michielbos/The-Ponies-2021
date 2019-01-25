using System;

namespace PoneCrafter.Json {

[Serializable]
public class JsonFurniture : BaseJsonModel {
    public string name;
    public string description;
    public int price;
}

}