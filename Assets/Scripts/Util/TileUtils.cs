using System;
using System.Collections.Generic;
using System.Linq;
using Model.Property;
using UnityEngine;

namespace Util {

public static class TileUtils {
    /// <summary>
    /// Returns the north-west border of the given tile, for the given object rotation.
    /// The default object rotation SouthEast is used as base.
    /// </summary>
    public static TileBorder GetNorthWestBorder(Vector2Int tile, ObjectRotation rotation) {
        switch (rotation) {
            case ObjectRotation.SouthEast:
                return new TileBorder(tile.x, tile.y + 1, WallDirection.NorthEast);
            case ObjectRotation.SouthWest:
                return new TileBorder(tile.x + 1, tile.y, WallDirection.NorthWest);
            case ObjectRotation.NorthWest:
                return new TileBorder(tile.x, tile.y, WallDirection.NorthEast);
            case ObjectRotation.NorthEast:
                return new TileBorder(tile.x, tile.y, WallDirection.NorthWest);
            default:
                throw new ArgumentOutOfRangeException(nameof(rotation), rotation, null);
        }
    }

    private static IEnumerable<Vector2Int> GetBorderTiles(IEnumerable<Vector2Int> tiles, ObjectRotation direction,
        int distanceFromEnd = 0) {
        int borderHeight;
        switch (direction) {
            case ObjectRotation.SouthEast:
                borderHeight = tiles.Max(tile => tile.y) - distanceFromEnd;
                return tiles.Where(tile => tile.y == borderHeight);
            case ObjectRotation.SouthWest:
                borderHeight = tiles.Max(tile => tile.x) - distanceFromEnd;
                return tiles.Where(tile => tile.x == borderHeight);
            case ObjectRotation.NorthWest:
                borderHeight = tiles.Min(tile => tile.y) + distanceFromEnd;
                return tiles.Where(tile => tile.y == borderHeight);
            case ObjectRotation.NorthEast:
                borderHeight = tiles.Min(tile => tile.x) + distanceFromEnd;
                return tiles.Where(tile => tile.x == borderHeight);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    /// <summary>
    /// Get all TileBorders for the furthest border in the given direction in the tile collection.
    /// </summary>
    public static IEnumerable<TileBorder> GetBorders(IEnumerable<Vector2Int> tiles, ObjectRotation direction,
        int distanceFromEnd = 0) {
        return GetBorderTiles(tiles, direction, distanceFromEnd).Select(tile => GetNorthWestBorder(tile, direction));
    }
}

}