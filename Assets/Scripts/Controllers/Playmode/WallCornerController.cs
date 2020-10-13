using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using Model.Property;
using UnityEngine;
using Util;

namespace Controllers.Playmode {

/// <summary>
/// Controller that is responsible for spawning and removing wall corners.
/// </summary>
public class WallCornerController : SingletonMonoBehaviour<WallCornerController> {
    public WallCorner wallCornerPrefab;
    
    private Dictionary<Vector2Int, WallCorner> wallCorners = new Dictionary<Vector2Int, WallCorner>();
    private HashSet<Vector2Int> pointsToUpdate = new HashSet<Vector2Int>();
    private bool hasUpdate;

    private void Update() {
        if (hasUpdate) {
            HandleCornerUpdates();
        }
    }
    
    /// <summary>
    /// Queue a point to have its corner updated in the next Update.
    /// </summary>
    public void UpdateCorners(Vector2Int point) {
        pointsToUpdate.Add(point);
        hasUpdate = true;
    }

    private void HandleCornerUpdates() {
        foreach (Vector2Int point in pointsToUpdate) {
            DoUpdateCorners(point);
        }
        pointsToUpdate.Clear();
        hasUpdate = false;
    }

    /// <summary>
    /// Create or remove the visible corner at the given point.
    /// </summary>
    private void DoUpdateCorners(Vector2Int point) {
        bool visible = PointNeedsWallCorner(point);
        WallCorner wallCorner = wallCorners.Get(point);
        if (visible && wallCorner == null) {
            wallCorners[point] = Instantiate(wallCornerPrefab, new Vector3(point.x, 0, point.y), Quaternion.identity);
            UpdateCornerVisibility(point);
        } else if (!visible && wallCorner != null) {
            Destroy(wallCorner.gameObject);
            wallCorners.Remove(point);
        }
    }
    
    private static bool PointNeedsWallCorner(Vector2Int point) {
        IList<Wall> walls = PropertyController.Property.GetWallsOnCorner(point);
        if (walls.Count == 1)
            return true;
        if (walls.Count != 2)
            return false;
        return walls[0].Direction != walls[1].Direction;
    }
    
    /// <summary>
    /// Update the visibility of all wall corners.
    /// </summary>
    public void UpdateAllCornerVisibility() {
        foreach (Vector2Int cornerPoint in wallCorners.Keys) {
            UpdateCornerVisibility(cornerPoint);
        }
    }
    
    private void UpdateCornerVisibility(Vector2Int point) {
        bool lowered = PropertyController.Property.GetWallsOnCorner(point).All(wall => wall.Lowered);
        wallCorners[point].SetLowered(lowered);
    }
}

}