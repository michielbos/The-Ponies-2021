/// <summary>
/// The buymode category for furniture items.
/// </summary>
public enum ObjectCategory {
	None = 0,
	
	//Buy mode
	Seating = 1,
	Surfaces = 2,
	Decoration = 3,
	Electronics = 4,
	Appliances = 5,
	Plumbing = 6,
	Lighting = 7,
	Misc = 8,
	
	//Build mode - Furniture
	Doors = 9,
	Windows = 10,
	Gardening = 11,
	Stairs = 12,
	Fireplaces = 13,
	
	//Build mode - Other tools
	Wall = 14,
	Floors = 15,
	WallCover = 16,
	Roofs = 17
}

/// <summary>
/// Utility class for ObjectCategory.
/// </summary>
public static class ObjectCategoryUtil {
	public static bool IsBuyCategory (ObjectCategory objectCategory) {
		int category = (int) objectCategory;
		return category >= 1 && category <= 8;
	}
	
	public static bool IsBuildCategory (ObjectCategory objectCategory) {
		int category = (int) objectCategory;
		return category >= 9 && category <= 17;
	}
	
	public static bool IsFurnitureCategory (ObjectCategory objectCategory) {
		int category = (int) objectCategory;
		return category >= 1 && category <= 13;
	}
}
