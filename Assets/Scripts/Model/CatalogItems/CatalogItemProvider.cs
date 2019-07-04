using System.Collections.Generic;
using System.Linq;
using Model.CatalogItems;
using UnityEngine;

/// <summary>
/// Class that provides lists of CatalogItems to the CatalogController.
/// </summary>
public static class CatalogItemProvider {
    /// <summary>
    /// Get a list of catalog items sorted by price, that belong to the given category.
    /// </summary>
    /// <param name="objectCategory">The ObjectCategory to get the items for.</param>
    /// <returns>A List with all matching CatalogItems.</returns>
    public static List<CatalogItem> GetCatalogItems(ObjectCategory objectCategory) {
        //I wish we could somehow squeeze the casting and sorting into a nice method.
        if (ObjectCategoryUtil.IsFurnitureCategory(objectCategory)) {
            List<CatalogItem> catalogItems = FurniturePresets.Instance.GetFurniturePresets(objectCategory)
                .Cast<CatalogItem>().ToList();
            catalogItems.Sort((a, b) => a.price.CompareTo(b.price));
            return catalogItems;
        }
        if (objectCategory == ObjectCategory.Floors) {
            List<CatalogItem> catalogItems = FloorPresets.Instance.GetFloorPresets().Cast<CatalogItem>().ToList();
            catalogItems.Sort((a, b) => a.price.CompareTo(b.price));
            return catalogItems;
        }
        if (objectCategory == ObjectCategory.Wall) {
            return new List<CatalogItem> {new WallPreset("Wall", "This is a wall.", 20)};
        }
        if (objectCategory != ObjectCategory.None) {
            Debug.LogWarning("Category " + objectCategory + " has not yet been implemented.");
        }
        return new List<CatalogItem>();
    }
}