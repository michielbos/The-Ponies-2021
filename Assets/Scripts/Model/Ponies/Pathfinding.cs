using System;
using System.Collections.Generic;
using System.Linq;
using Model.Property;
using UnityEngine;

namespace Model.Ponies {

public static class Pathfinding {
    private const int MaxPathLength = 200;

    public static Path PathToTile(Vector2Int start, Vector2Int target) {
        return PathToNearest(start, new[] {target});
    }

    public static Path PathToNearest(Vector2Int start, IEnumerable<Vector2Int> targets) {
        Property.Property property = PropertyController.Instance.property;
        int width = property.TerrainWidth;
        int height = property.TerrainHeight;
        int[,] stepMap = property.GetTileOccupancyMap();
        targets = RemoveTilesOutsideMap(targets, width, height);
        HashSet<TileBorder> occupiedBorders = property.GetImpassableBorders();

        // If we're starting on a target, return that target.
        if (targets.Contains(start))
            return new Path(new[] {start});
        
        stepMap[start.y, start.x] = 1;
        int step;
        for (step = 1; step <= MaxPathLength; step++) {
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
                    return CreatePathFromStepMap(stepMap, width, height, target, occupiedBorders);
                }
            }
        }

        return null;
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