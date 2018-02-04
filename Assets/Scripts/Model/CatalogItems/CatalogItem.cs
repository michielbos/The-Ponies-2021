using System;
using UnityEngine;

/// <summary>
/// Abstract class for items that can be displayed in the catalog.
/// </summary>
public abstract class CatalogItem {
    public readonly Guid guid;
    public readonly string name;
    public readonly string description;
    public readonly int price;
    public readonly ObjectCategory category;
    public readonly NeedStats needStats;
    public readonly SkillStats skillStats;
    public readonly RequiredAge requiredAge;

    public CatalogItem (Guid guid, string name, string description, int price, ObjectCategory category, NeedStats needStats, 
        SkillStats skillStats, RequiredAge requiredAge) {
        this.guid = guid;
        this.name = name;
        this.description = description;
        this.price = price;
        this.category = category;
        this.needStats = needStats;
        this.skillStats = skillStats;
        this.requiredAge = requiredAge;
    }
    
    public CatalogItem (Guid guid, string name, string description, int price, ObjectCategory category)
        : this(guid, name, description, price, category, new NeedStats(), new SkillStats(), RequiredAge.Any) {
        
    }

    /// <summary>
    /// This should return an array of textures with the previews of this catalog item with its skins, to be displayed
    /// on the catalog button, preview popup and (if more than 1 is provided) the skin selection buttons.
    /// If this catalog item has no skins, this should return an array with the normal preview texture as only element.
    /// Keep in mind that this should never return an array longer than the number of skins an item has.
    /// </summary>
    /// <returns>An array of preview Textures for this CatalogItem with its various skins.</returns>
    public abstract Texture[] GetPreviewTextures ();
}
