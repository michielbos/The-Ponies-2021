using System;
using PoneCrafter.Model;

namespace PoneCrafter.Json {

[Serializable]
public class JsonFurniture : BaseJsonModel {
    public string name;
    public string description;
    public int price;
    public ObjectCategory category;
    public NeedStats needStats;
    public SkillStats skillStats;
    public RequiredAge requiredAge;
}

}