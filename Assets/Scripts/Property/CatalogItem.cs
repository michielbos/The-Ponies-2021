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

    public CatalogItem (Guid guid, string name, string description, int price, ObjectCategory category) {
        this.guid = guid;
        this.name = name;
        this.description = description;
        this.price = price;
        this.category = category;
    }

    /// <summary>
    /// This should return a preview texture for this catalog item, which will be displayed on the buttons and in the preview popup.
    /// </summary>
    /// <returns>A preview Texture for this CatalogItem.</returns>
    public abstract Texture GetPreviewTexture ();
}
