namespace Model.Property {

/// <summary>
/// The rotations a wall can have.
/// NorthEast and NorthWest follow a tile border from the south corner into the given direction.
/// Horizontal and Vertical go through the tile that the wall is placed on.
/// All rotations are based on the default camera orientation.
/// </summary>
public enum WallDirection {
    NorthEast = 1,
    NorthWest = 2,
    Horizontal = 3,
    Vertical = 4
}

}