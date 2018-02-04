using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public static List<CatalogItem> GetCatalogItems (ObjectCategory objectCategory) {
		if (ObjectCategoryUtil.IsFurnitureCategory(objectCategory)) {
			List<CatalogItem> catalogItems = FurniturePresets.Instance.GetFurniturePresets(objectCategory).Cast<CatalogItem>().ToList();
			catalogItems.Sort((a, b) => a.price.CompareTo(b.price));
			return catalogItems;
		}
		Debug.LogWarning("Category " + objectCategory + " has not yet been implemented.");
		return new List<CatalogItem>();
	}
}
