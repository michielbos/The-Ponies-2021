namespace Model.Property {

/// <summary>
/// Struct specifying a tile border.
/// Can be useful for specifying potential wall positions.
/// </summary>
public struct TileBorder {
    public readonly int x;
    public readonly int y;
    public readonly WallDirection wallDirection;

    public TileBorder(int x, int y, WallDirection wallDirection) {
        this.x = x;
        this.y = y;
        this.wallDirection = wallDirection;
    }

    public bool Equals(TileBorder other) {
        return x == other.x && y == other.y && wallDirection == other.wallDirection;
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj))
            return false;
        return obj is TileBorder && Equals((TileBorder) obj);
    }

    public override int GetHashCode() {
        unchecked {
            int hashCode = x;
            hashCode = (hashCode * 397) ^ y;
            hashCode = (hashCode * 397) ^ (int) wallDirection;
            return hashCode;
        }
    }
}

}