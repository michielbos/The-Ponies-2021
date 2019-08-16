using System.Collections.Generic;
using System.Linq;
using Controllers.Singletons;
using Model.Property;
using UnityEngine;

namespace Controllers.Tools {

/// <summary>
/// Tool for build mode that deals with placing and removing wall covers.
/// </summary>
public class WallCoverTool : MonoBehaviour, ITool {
    public GameObject buildMarkerPrefab;
    private int wallLayer;

    private WallCoverPreset selectedPreset;
    private List<Wall> selectedWalls = new List<Wall>();
    private Wall draggingWallStart;

    private void Start() {
        wallLayer = LayerMask.GetMask("Walls");
    }

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (selectedPreset == null) {
            ClearSelectedWalls();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000, wallLayer)) {
            ClearSelectedWalls();
            if (Input.GetMouseButtonUp(0))
                draggingWallStart = null;
            return;
        }

        Wall wall = hit.transform.GetComponent<Wall>();
        bool isFrontWall = wall.IsFrontOfWall(hit.point);

        if (Input.GetMouseButtonDown(0)) {
            draggingWallStart = wall;
        }

        List<Wall> newWalls = draggingWallStart != null
            ? GetWallsForDragging(draggingWallStart, wall)
            : new List<Wall>(new[] {wall});
        
        selectedWalls.ForEach(removedWall => removedWall.RemovePreviewCovers());
        selectedWalls = newWalls;
        newWalls.ForEach(addedWall => {
            addedWall.RemovePreviewCovers();
            addedWall.SetVisibleCoverMaterial(isFrontWall, selectedPreset);
        });
        
        if (Input.GetMouseButtonUp(0) && draggingWallStart != null) {
            if (BuyWallCovers(newWalls, isFrontWall)) {
                SoundController.Instance.PlaySound(SoundType.PlaceFloor);
            } else {
                SoundController.Instance.PlaySound(SoundType.Deny);
            }
            draggingWallStart = null;
        }
    }

    private List<Wall> GetWallsForDragging(Wall startWall, Wall endWall) {
        TileBorder start = startWall.TileBorder;
        TileBorder end = endWall.TileBorder;
            
        // Walls can only be dragged across either the X or Y axis, depending on the wall direction.
        if (start.wallDirection != end.wallDirection || 
            start.wallDirection == WallDirection.NorthWest && start.x != end.x || 
            start.wallDirection == WallDirection.NorthEast && start.y != end.y)
            return new List<Wall>();

        Vector2Int startPosition = start.StartPosition;
        Vector2Int endPosition = end.StartPosition;
        List<TileBorder> tileBorders = new List<TileBorder>();
        int distance = Mathf.RoundToInt(Vector2Int.Distance(startPosition, endPosition));
        float stepSize = distance > 0 ? 1f / distance : 1;
        for (int i = 0; i <= distance; i++) {
            Vector2 nextPosition = Vector2.Lerp(startPosition, endPosition, i * stepSize);
            tileBorders.Add(new TileBorder(Mathf.RoundToInt(nextPosition.x), Mathf.RoundToInt(nextPosition.y),
                start.wallDirection));
        }

        return PropertyController.Instance.property.GetWalls(tileBorders).ToList();
    }

    private void ClearSelectedWalls() {
        selectedWalls.ForEach(wall => wall.RemovePreviewCovers());
        selectedWalls.Clear();
    }

    private bool BuyWallCovers(List<Wall> walls, bool front) {
        int cost = walls.Count(wall => !HasThisPreset(wall, front, selectedPreset)) * selectedPreset.price;
        if (!MoneyController.Instance.CanAfford(cost))
            return false;
        walls.ForEach(wall => ApplyPreset(wall, front, selectedPreset));
        MoneyController.Instance.ChangeFunds(-cost);
        return true;
    }
    
    private bool HasThisPreset(Wall wall, bool front, WallCoverPreset preset) {
        return front ? wall.CoverFront == preset : wall.CoverBack == preset;
    }

    private void ApplyPreset(Wall wall, bool front, WallCoverPreset preset) {
        if (front)
            wall.CoverFront = selectedPreset;
        else
            wall.CoverBack = selectedPreset;
    }

    public void Enable() {
        selectedPreset = null;
    }

    public void Disable() { }

    public void OnCatalogSelect(CatalogItem item, int skin) {
        WallCoverPreset preset = item as WallCoverPreset;
        if (preset == null) {
            Debug.LogWarning(item + " is not a WallCoverPreset, cannot be set to FloorTool.");
            return;
        }
        selectedPreset = preset;
    }
}

}