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

    private void Start() {
        wallLayer = LayerMask.GetMask("Walls");
    }

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (selectedPreset == null)
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000, wallLayer))
            return;
        
        Wall wall = hit.transform.GetComponent<Wall>();
        if (Input.GetMouseButtonDown(0)) {
            if (ApplyPreset(wall, wall.IsFrontOfWall(hit.point))) {
                SoundController.Instance.PlaySound(SoundType.PlaceFloor);
            }
        }
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