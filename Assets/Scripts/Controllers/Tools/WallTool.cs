using Assets.Scripts.Controllers;
using Model.Property;
using UnityEngine;

namespace Controllers.Tools {

/// <summary>
/// Tool for build mode that deals with placing and removing walls.
/// </summary>
public class WallTool : MonoBehaviour, ITool {
    public GameObject buildMarkerPrefab;

    private CatalogItem wallPreset;

    private GameObject buildMarker;
    private Vector2Int? currentTarget;
    private bool pressingTile;

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (wallPreset == null)
            return;
        
        Vector2Int? tile;
        if (tileIndex.x == -1 && tileIndex.y == -1) {
            tile = null;
        } else {
            tile = tileIndex;
        }

        if (pressingTile) {
            HandlePlacementHolding(tile);
        } else {
            UpdateBuildMarker(tile);
        }
    }

    public void OnCatalogSelect(CatalogItem item, int skin) {
        wallPreset = item;
    }

    public void Enable() {
        CreateBuildMarker();
    }

    public void Disable() {
        if (buildMarker != null) {
            Destroy(buildMarker);
        }
    }

    private void CreateBuildMarker() {
        if (buildMarker != null) {
            Destroy(buildMarker);
        }

        buildMarker = Instantiate(buildMarkerPrefab);
        buildMarker.transform.position = new Vector3(0, -100, 0);
    }

    private void UpdateBuildMarker(Vector2Int? newTarget) {
        if (newTarget != null) {
            if (newTarget != currentTarget) {
                BuildMarkerMoved(newTarget.Value);
            }

            if (Input.GetMouseButtonDown(0)) {
                SoundController.Instance.PlaySound(SoundType.DragStart);
                pressingTile = true;
            }
        } else if (currentTarget != null) {
            buildMarker.transform.position = new Vector3(0, -100, 0);
            currentTarget = null;
        }
    }

    private void BuildMarkerMoved(Vector2Int newTarget) {
        currentTarget = newTarget;
        SetBuildMarkerPosition(newTarget);
    }

    private void SetBuildMarkerPosition(Vector2Int target) {
        buildMarker.transform.position = new Vector3(target.x, 0, target.y);
    }

    private void HandlePlacementHolding(Vector2Int? target) {
        bool canPlace = true;
        buildMarker.SetActive(canPlace);
        if (Input.GetMouseButtonUp(0)) {
            if (currentTarget != null) {
                PropertyController.Instance.property.PlaceWall(currentTarget.Value.x, currentTarget.Value.y, WallDirection.NorthEast);
            }

            CreateBuildMarker();
            pressingTile = false;
        }
    }
}

}