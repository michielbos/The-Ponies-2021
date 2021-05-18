using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Model.Property;
using UnityEngine;

namespace Model.Ponies {

public static class Pathfinding {
    public const int DefaultMaxPathLength = 200;

    /// <summary>
    /// Calculate a path from the start tile to the target tile.
    /// Returns null if no path was found.
    /// </summary>
    [CanBeNull]
    public static Path PathToTile(Vector2Int start, Vector2Int target, int maxPathLength = DefaultMaxPathLength) {
        return PathToNearest(start, new[] {target}, maxPathLength);
    }

    /// <summary>
    /// Calculate a path from the start tile to the closest tile in the targets.
    /// Returns null if no path was found.
    /// </summary>
    [CanBeNull]
    public static Path PathToNearest(Vector2Int start, IEnumerable<Vector2Int> targets,
        int maxPathLength = DefaultMaxPathLength) {
        Property.Property property = PropertyController.Instance.property;
        int width = property.TerrainWidth;
        int height = property.TerrainHeight;
        HashSet<TileBorder> occupiedBorders = property.GetImpassableBorders();
        IList<Vector2Int> trimmedTargets = RemoveTilesOutsideMap(targets, width, height);

        // If we're starting on a target, return that target.
        if (trimmedTargets.Contains(start))
            return new Path(new[] {start});

        if (BuildStepMap(start, trimmedTargets, maxPathLength, out int[,] stepMap, occupiedBorders)) {
            foreach (Vector2Int target in trimmedTargets) {
                if (stepMap[target.y, target.x] > 0) {
                    return CreatePathFromStepMap(stepMap, width, height, target, occupiedBorders);
                }
            }
        }

        return null;
    }
    
    /// <summary>
    /// Calculate a path from the start tile to the closest tile in the targets.
    /// Returns null if no path was found.
    /// </summary>
    [CanBeNull]
    public static Path PathToRange(Vector2Int start, IEnumerable<Vector2Int> targets,
        int maxPathLength = DefaultMaxPathLength) {
        Property.Property property = PropertyController.Instance.property;
        int width = property.TerrainWidth;
        int height = property.TerrainHeight;
        HashSet<TileBorder> occupiedBorders = property.GetImpassableBorders();
        IList<Vector2Int> trimmedTargets = RemoveTilesOutsideMap(targets, width, height);

        // If we're starting on a target, return that target.
        if (trimmedTargets.Contains(start))
            return new Path(new[] {start});

        if (BuildStepMap(start, trimmedTargets, maxPathLength, out int[,] stepMap, occupiedBorders)) {
            foreach (Vector2Int target in trimmedTargets) {
                if (stepMap[target.y, target.x] > 0) {
                    return CreatePathFromStepMap(stepMap, width, height, target, occupiedBorders);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Build a step map
    /// </summary>
    /// <param name="start">The start position</param>
    /// <param name="targets">The targets, stripped with RemoveTilesOutsideMap</param>
    /// <param name="maxPathLength"></param>
    /// <param name="stepMap">The generated stepmap</param>
    /// <param name="occupiedBorders">All occupied borders from property.GetImpassableBorders. Shared for performance.</param>
    /// <returns>True if a path was found.</returns>
    private static bool BuildStepMap(Vector2Int start, ICollection<Vector2Int> targets, int maxPathLength,
        out int[,] stepMap, HashSet<TileBorder> occupiedBorders) {
        Property.Property property = PropertyController.Instance.property;
        int width = property.TerrainWidth;
        int height = property.TerrainHeight;
        stepMap = property.GetTileOccupancyMap();

        stepMap[start.y, start.x] = 1;
        int step;
        for (step = 1; step <= maxPathLength; step++) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (stepMap[y, x] != step) {
                        continue;
                    }
                    if (y + 1 < height && stepMap[y + 1, x] == 0 &&
                        !occupiedBorders.Contains(new TileBorder(x, y + 1, WallDirection.NorthEast))) {
                        stepMap[y + 1, x] = step + 1;
                    }
                    if (x + 1 < width && stepMap[y, x + 1] == 0 &&
                        !occupiedBorders.Contains(new TileBorder(x + 1, y, WallDirection.NorthWest))) {
                        stepMap[y, x + 1] = step + 1;
                    }
                    if (y > 0 && stepMap[y - 1, x] == 0 &&
                        !occupiedBorders.Contains(new TileBorder(x, y, WallDirection.NorthEast))) {
                        stepMap[y - 1, x] = step + 1;
                    }
                    if (x > 0 && stepMap[y, x - 1] == 0 &&
                        !occupiedBorders.Contains(new TileBorder(x, y, WallDirection.NorthWest))) {
                        stepMap[y, x - 1] = step + 1;
                    }
                }
            }

            foreach (Vector2Int target in targets) {
                if (stepMap[target.y, target.x] > 0) {
                    return true;
                }
            }
        }

        return false;
    }

    private static List<Vector2Int> RemoveTilesOutsideMap(IEnumerable<Vector2Int> targets, int mapWidth,
        int mapHeight) {
        return targets.Where(target => target.x >= 0 && target.y >= 0 && target.x < mapWidth && target.y < mapHeight)
            .ToList();
    }

    private static Path CreatePathFromStepMap(int[,] stepMap, int width, int height, Vector2Int target,
        HashSet<TileBorder> occupiedBorders) {
        int startStep = stepMap[target.y, target.x];
        Vector2Int[] tiles = new Vector2Int[startStep];
        tiles[startStep - 1] = target;
        Vector2Int currentTile = target;
        for (int step = startStep - 1; step > 0; step--) {
            int x = currentTile.x;
            int y = currentTile.y;
            if (y + 1 < height && stepMap[y + 1, x] == step &&
                !occupiedBorders.Contains(new TileBorder(x, y + 1, WallDirection.NorthEast))) {
                currentTile = new Vector2Int(x, y + 1);
            } else if (x + 1 < width && stepMap[y, x + 1] == step &&
                       !occupiedBorders.Contains(new TileBorder(x + 1, y, WallDirection.NorthWest))) {
                currentTile = new Vector2Int(x + 1, y);
            } else if (y > 0 && stepMap[y - 1, x] == step &&
                       !occupiedBorders.Contains(new TileBorder(x, y, WallDirection.NorthEast))) {
                currentTile = new Vector2Int(x, y - 1);
            } else if (x > 0 && stepMap[y, x - 1] == step &&
                       !occupiedBorders.Contains(new TileBorder(x, y, WallDirection.NorthWest))) {
                currentTile = new Vector2Int(x - 1, y);
            } else {
                throw new Exception("Failed to find tile for step " + step + " at " + target);
            }

            tiles[step - 1] = currentTile;
        }

        return new Path(tiles);
    }
}

}