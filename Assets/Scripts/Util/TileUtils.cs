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

    /// <summary>
    /// Get the tile that would be next to this one, when moving 1 tile into the given direction.
    /// </summary>
    public static Vector2Int GetNeighbourTile(this Vector2Int tile, ObjectRotation direction) {
        return tile + GetTileForDirection(direction);
    }
    
    /// <summary>
    /// Get the 4 tiles that are directly next to this one.
    /// </summary>
    public static ICollection<Vector2Int> GetNeighbourTiles(this Vector2Int tile) {
        return new[] {
            new Vector2Int(tile.x - 1, tile.y), 
            new Vector2Int(tile.x + 1, tile.y), 
            new Vector2Int(tile.x, tile.y - 1), 
            new Vector2Int(tile.x, tile.y + 1)
        };
    }

    private static Vector2Int GetTileForDirection(ObjectRotation direction) {
        switch (direction) {
            case ObjectRotation.SouthEast:
                return new Vector2Int(0, -1);
            case ObjectRotation.SouthWest:
                return new Vector2Int(-1, 0);
            case ObjectRotation.NorthWest:
                return new Vector2Int(0, 1);
            case ObjectRotation.NorthEast:
                return new Vector2Int(1, 0);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    /// <summary>
    /// Get the border between two tiles.
    /// Both tiles must be next to each other, otherwise an ArgumentException is thrown.
    /// </summary>
    /// <exception cref="ArgumentException">If the tiles are not next to each other.</exception>
    public static TileBorder GetBorderBetweenTiles(this Vector2Int tile1, Vector2Int tile2) {
        int tileDistance = Mathf.Abs(tile1.x - tile2.x) + Mathf.Abs(tile1.y - tile2.y);
        if (tileDistance != 1) {
            throw new ArgumentException("Tile distance must be 1, but was " + tileDistance);
        }
        
        if (tile1.x < tile2.x)
            return new TileBorder(tile2.x, tile2.y, WallDirection.NorthWest);
        if (tile1.x > tile2.x)
            return new TileBorder(tile1.x, tile1.y, WallDirection.NorthWest);
        if (tile1.y < tile2.y)
            return new TileBorder(tile1.x, tile2.y, WallDirection.NorthEast);
        return new TileBorder(tile2.x, tile1.y, WallDirection.NorthEast);
    }

    /// <summary>
    /// Convert a tile position to a Vector3 world position.
    /// </summary>
    public static Vector3 ToWorldPosition(this Vector2Int tilePosition) =>
        new Vector3(tilePosition.x, 0, tilePosition.y);
    
    /// <summary>
    /// Convert a world position to a tile position.
    /// This rounds the position and removes the height data.
    /// </summary>
    public static Vector2Int ToTilePosition(this Vector3 worldPosition) =>
        new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    
    /// <summary>
    /// Calculate the new position of a tile, assuming it was rotated as part of a tile group.
    /// </summary>
    public static Vector2Int RotateTileInGroup(this Vector2Int tile, ObjectRotation rotation) {
        if (rotation == ObjectRotation.NorthEast) {
            return new Vector2Int(tile.y, -tile.x);
        }
        if (rotation == ObjectRotation.SouthEast) {
            return new Vector2Int(-tile.x, -tile.y);
        }
        if (rotation == ObjectRotation.SouthWest) {
            return new Vector2Int(-tile.y, tile.x);
        }
        // NorthWest
        return tile;
    }
    
    /// <summary>
    /// Vector3 version of RotateTileInGroup.
    /// Calculate the new position, assuming it was rotated as part of a tile group.
    /// </summary>
    public static Vector3 RotateInGroup(this Vector3 tile, ObjectRotation rotation) {
        if (rotation == ObjectRotation.SouthWest) {
            return new Vector3(tile.z, tile.y, -tile.x);
        }
        if (rotation == ObjectRotation.NorthWest) {
            return new Vector3(-tile.x, tile.y, -tile.z);
        }
        if (rotation == ObjectRotation.NorthEast) {
            return new Vector3(-tile.z, tile.y, tile.x);
        }
        // SouthEast
        return tile;
    }

    public static ObjectRotation GetDirectionTo(this Vector2Int source, Vector2Int target) {
        Vector2Int distance = target - source;
        if (distance.x > 0 && distance.x > Mathf.Abs(distance.y))
            return ObjectRotation.NorthEast;
        if (distance.x < 0 && -distance.x > Mathf.Abs(distance.y))
            return ObjectRotation.SouthWest;
        if (distance.y > 0 && distance.y > Mathf.Abs(distance.x))
            return ObjectRotation.NorthWest;
        if (distance.y < 0)
            return ObjectRotation.SouthEast;
        
        // If the distance is zero or otherwise invalid.
        return ObjectRotation.NorthEast;
    }
}

}