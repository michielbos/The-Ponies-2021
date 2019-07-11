/// <summary>
/// Enum for specifying where a furniture item can be placed.
/// </summary>
public enum PlacementType {
    Ground = 0,
    Terrain = 1,
    Floor = 2,
    Surface = 3,
    GroundOrSurface = 4,
    Counter = 5,
    Wall = 6,
    ThroughWall = 7,
    Ceiling = 8
}

/// <summary>
/// Utility functions for PlacementType.
/// </summary>
public static class PlacementTypeUtil {
    public static bool CanPlaceOnTerrain(PlacementType type) {
        return type == PlacementType.Ground || type == PlacementType.Terrain || type == PlacementType.GroundOrSurface;
    }
    
    public static bool CanPlaceOnFloor(PlacementType type) {
        return type == PlacementType.Ground || type == PlacementType.Floor || type == PlacementType.GroundOrSurface;
    }
}