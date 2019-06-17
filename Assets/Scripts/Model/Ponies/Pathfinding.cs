using System;
using UnityEngine;

namespace Model.Ponies {

public static class Pathfinding {
    private const int MaxPathLength = 100;

    public static Path PathToTile(Vector2Int start, Vector2Int target) {
        Property.Property property = PropertyController.Instance.property;
        int width = property.TerrainWidth;
        int height = property.TerrainHeight;
        int[,] stepMap = property.GetTileOccupancyMap();
        stepMap[start.y, start.x] = 1;
        int step;
        for (step = 1; step <= MaxPathLength; step++) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (stepMap[y, x] != step) {
                        continue;
                    }
                    if (y + 1 < height && stepMap[y + 1, x] == 0) {
                        stepMap[y + 1, x] = step + 1;
                    }
                    if (x + 1 < width && stepMap[y, x + 1] == 0) {
                        stepMap[y, x + 1] = step + 1;
                    }
                    if (y > 0 && stepMap[y - 1, x] == 0) {
                        stepMap[y - 1, x] = step + 1;
                    }
                    if (x > 0 && stepMap[y, x - 1] == 0) {
                        stepMap[y, x - 1] = step + 1;
                    }
                }
            }
            if (stepMap[target.y, target.x] > 0) {
                return CreatePathFromStepMap(stepMap, width, height, target);
            }
        }

        return null;
    }

    private static Path CreatePathFromStepMap(int[,] stepMap, int width, int height, Vector2Int target) {
        int startStep = stepMap[target.y, target.x];
        Vector2Int[] tiles = new Vector2Int[startStep];
        tiles[startStep - 1] = target;
        Vector2Int currentTile = target;
        for (int step = startStep - 1; step > 0; step--) {
            int x = currentTile.x;
            int y = currentTile.y;
            if (y + 1 < height && stepMap[y + 1, x] == step) {
                currentTile = new Vector2Int(x, y + 1);
            } else if (x + 1 < width && stepMap[y, x + 1] == step) {
                currentTile = new Vector2Int(x + 1, y);
            } else if (y > 0 && stepMap[y - 1, x] == step) {
                currentTile = new Vector2Int(x, y - 1);
            } else if (x > 0 && stepMap[y, x - 1] == step) {
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