using System;
using PoneCrafter.Model;
using UnityEngine;

/// <summary>
/// Abstract class for catalog items that can be loaded with a preset loader.
/// </summary>
public abstract class Preset : CatalogItem {
    public readonly Guid guid;
    public AssetBundle assetBundle;

    protected Preset (Guid guid, string name, string description, int price, ObjectCategory category,
        NeedStats needStats, SkillStats skillStats, RequiredAge requiredAge) :
        base(name, description, price, category, needStats, skillStats, requiredAge) {
        this.guid = guid;
    }

    protected Preset (Guid guid, string name, string description, int price, ObjectCategory category) :
        base(name, description, price, category) {
        this.guid = guid;
    }

}
