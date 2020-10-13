using System.Collections.Generic;
using System.Linq;
using Controllers.Playmode;
using Controllers.Singletons;
using Model.Property;
using UnityEngine;

namespace Controllers.Tools {

/// <summary>
/// Tool for build mode that deals with placing and removing wall covers.
/// </summary>
public class WallCoverTool : MonoBehaviour, ITool {
    private int wallLayer;

    private WallCoverPreset selectedPreset;
    private List<WallSide> selectedWalls = new List<WallSide>();
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
        WallSide wallSide = new WallSide(wall, isFrontWall);
        bool demolishing = Input.GetKey(KeyCode.LeftControl);
        bool roomMode = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetMouseButtonDown(0)) {
            draggingWallStart = wall;
            SoundController.Instance.PlaySound(SoundType.DragStart);
        }

        List<WallSide> newWalls;
        if (roomMode)
            newWalls = PropertyController.Instance.property.GetWallSideLoop(wallSide) ?? new List<WallSide>();
        else
            newWalls = draggingWallStart != null
                ? GetWallsForDragging(draggingWallStart, wall, isFrontWall)
                : new List<WallSide>(new[] {wallSide});
        
        // Only update the selected walls when the new selection is different.
        // This saves a LOT of performance, since we don't have to refresh dozens of wall materials every frame.
        if (!selectedWalls.SequenceEqual(newWalls)) {
            ClearSelectedWalls();

            selectedWalls = newWalls;
            newWalls.ForEach(addedWall => {
                addedWall.wall.RemovePreviewCovers();
                addedWall.wall.SetVisibleCoverMaterial(addedWall.front, demolishing ? null : selectedPreset);
                addedWall.wall.UpdateVisibility(WallVisibility.Full);
            });
        }

        if (Input.GetMouseButtonUp(0) && draggingWallStart != null) {
            if (demolishing) {
                if (newWalls.Count > 0) {
                    RemoveWallCovers(newWalls);
                    SoundController.Instance.PlaySound(SoundType.PlaceFloor);
                }
            } else {
                if (BuyWallCovers(newWalls)) {
                    SoundController.Instance.PlaySound(SoundType.PlaceFloor);
                } else {
                    SoundController.Instance.PlaySound(SoundType.Deny);
                }
            }
            draggingWallStart = null;
        }
    }

    private List<WallSide> GetWallsForDragging(Wall startWall, Wall endWall, bool front) {
        TileBorder start = startWall.TileBorder;
        TileBorder end = endWall.TileBorder;

        // Walls can only be dragged across either the X or Y axis, depending on the wall direction.
        if (start.wallDirection != end.wallDirection ||
            start.wallDirection == WallDirection.NorthWest && start.x != end.x ||
            start.wallDirection == WallDirection.NorthEast && start.y != end.y)
            return new List<WallSide>();

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

        return PropertyController.Instance.property.GetWalls(tileBorders).Select(wall => new WallSide(wall, front))
            .ToList();
    }

    private void ClearSelectedWalls() {
        selectedWalls.ForEach(wallSide => {
            wallSide.wall.UpdateVisibility();
            wallSide.wall.RemovePreviewCovers();
        });
        selectedWalls.Clear();
    }

    private bool BuyWallCovers(List<WallSide> walls) {
        int cost = walls.Count(wallSide => !wallSide.HasThisPreset(selectedPreset)) * selectedPreset.price;
        if (!MoneyController.Instance.CanAfford(cost))
            return false;
        walls.ForEach(wallSide => wallSide.ApplyCoverPreset(selectedPreset));
        MoneyController.Instance.ChangeFunds(-cost);
        return true;
    }

    private void RemoveWallCovers(List<WallSide> walls) {
        int refund = walls.Sum(wallSide => wallSide.wall.GetCoverPreset(wallSide.front)?.GetSellValue() ?? 0);
        walls.ForEach(wallSide => wallSide.ApplyCoverPreset(null));
        MoneyController.Instance.ChangeFunds(refund);
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