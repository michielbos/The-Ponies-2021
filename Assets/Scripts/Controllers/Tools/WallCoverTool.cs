using System.Collections.Generic;
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
            return;
        }

        Wall wall = hit.transform.GetComponent<Wall>();
        bool isFrontWall = wall.IsFrontOfWall(hit.point);
        List<Wall> newWalls = new List<Wall>(new[] {wall});
        
        selectedWalls.ForEach(removedWall => removedWall.RemovePreviewCovers());
        selectedWalls = newWalls;
        newWalls.ForEach(addedWall => {
            addedWall.RemovePreviewCovers();
            addedWall.SetVisibleCoverMaterial(isFrontWall, selectedPreset);
        });

        if (Input.GetMouseButtonDown(0)) {
            if (ApplyPreset(wall, isFrontWall)) {
                SoundController.Instance.PlaySound(SoundType.PlaceFloor);
            }
        }
    }

    private void ClearSelectedWalls() {
        selectedWalls.ForEach(wall => wall.RemovePreviewCovers());
        selectedWalls.Clear();
    }

    public bool ApplyPreset(Wall wall, bool front) {
        if (front) {
            if (wall.CoverFront != selectedPreset) {
                wall.CoverFront = selectedPreset;
                return true;
            }
        } else {
            if (wall.CoverBack != selectedPreset) {
                wall.CoverBack = selectedPreset;
                return true;
            }
        }
        return false;
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