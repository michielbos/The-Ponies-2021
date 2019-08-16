using System;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector2Int StartPosition => new Vector2Int(x, y);

    public Vector2Int EndPosition => new Vector2Int(wallDirection != WallDirection.NorthWest ? x + 1 : x,
        wallDirection == WallDirection.NorthWest || wallDirection == WallDirection.Vertical ? y + 1 :
        wallDirection == WallDirection.Horizontal ? y - 1 : y);
    
    public List<TileBorder> GetConnectedBorders(bool includeSelf) {
        List<TileBorder> borders = new List<TileBorder>(8);
        AddConnectedBorders(StartPosition, borders);
        AddConnectedBorders(EndPosition, borders);
        if (!includeSelf)
            borders.Remove(this);
        return borders;
    }

    private void AddConnectedBorders(Vector2Int point, List<TileBorder> borderList) {
        borderList.Add(new TileBorder(point.x, point.y, WallDirection.NorthEast));
        borderList.Add(new TileBorder(point.x, point.y, WallDirection.NorthWest));
        borderList.Add(new TileBorder(point.x - 1, point.y, WallDirection.NorthEast));
        borderList.Add(new TileBorder(point.x, point.y - 1, WallDirection.NorthWest));
    }

    /// <summary>
    /// Get the border from this border's end position into the left direction.
    /// If forward is false, the border will be searched from the start tile rather than the end tile.
    /// </summary>
    public TileBorder GetLeftBorder(bool forward) {
        Vector2Int position = forward ? EndPosition : StartPosition;
        
        if (forward && wallDirection == WallDirection.NorthWest)
            position.x--;
        else if (!forward && wallDirection == WallDirection.NorthEast)
            position.y--;

        WallDirection direction = wallDirection == WallDirection.NorthEast
            ? WallDirection.NorthWest
            : WallDirection.NorthEast;
        
        return new TileBorder(position.x, position.y, direction);
    }
    
    /// <summary>
    /// Get the border from this border's end position into the forward direction.
    /// If forward is false, the border will be searched from the start tile rather than the end tile.
    /// </summary>
    public TileBorder GetForwardBorder(bool forward) {
        Vector2Int position = forward ? EndPosition : StartPosition;
        if (forward)
            return new TileBorder(position.x, position.y, wallDirection);
        if (wallDirection == WallDirection.NorthEast)
            return new TileBorder(position.x - 1, position.y, wallDirection);
        if (wallDirection == WallDirection.NorthWest)
            return new TileBorder(position.x, position.y - 1, wallDirection);
        throw new Exception("Horizontal/Vertical walls are not implemented yet.");
    }
    
    /// <summary>
    /// Get the border from this border's end position into the right direction.
    /// If forward is false, the border will be searched from the start tile rather than the end tile.
    /// </summary>
    public TileBorder GetRightBorder(bool forward) {
        Vector2Int position = forward ? EndPosition : StartPosition;
        
        if (forward && wallDirection == WallDirection.NorthEast)
            position.y--;
        else if (!forward && wallDirection == WallDirection.NorthWest)
            position.x--;
        
        WallDirection direction = wallDirection == WallDirection.NorthEast
            ? WallDirection.NorthWest
            : WallDirection.NorthEast;
        
        return new TileBorder(position.x, position.y, direction);
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

    public override string ToString() {
        return $"{nameof(x)}: {x}, {nameof(y)}: {y}, {nameof(wallDirection)}: {wallDirection}";
    }
}

}