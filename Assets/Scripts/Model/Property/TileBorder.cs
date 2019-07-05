namespace Model.Property {

/// <summary>
/// Struct specifying a tile border.
/// Can be useful for specifying potential wall positions.
/// </summary>
public struct TileBorder {
    public int x;
    public int y;
    public WallDirection wallDirection;

    public TileBorder(int x, int y, WallDirection wallDirection) {
        this.x = x;
        this.y = y;
        this.wallDirection = wallDirection;
    }
}

}