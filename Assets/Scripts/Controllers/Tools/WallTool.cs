using Assets.Scripts.Controllers;
using Model.Property;
using UnityEngine;

namespace Controllers.Tools {

/// <summary>
/// Tool for build mode that deals with placing and removing walls.
/// </summary>
public class WallTool : MonoBehaviour, ITool {
    public GameObject buildMarkerPrefab;
    public GameObject wallMarkerPrefab;
    public Material wallMarkerMaterial;
    public Material wallDenyMaterial;

    private CatalogItem wallPreset;

    private GameObject buildMarker;
    private GameObject wallMarker;
    private Vector2Int? currentTarget;
    private bool pressingTile;

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (wallPreset == null)
            return;

        Vector2Int? tile;
        if (tilePosition == Vector3.zero) {
            tile = null;
        } else {
            tile = new Vector2Int(Mathf.RoundToInt(tilePosition.x), Mathf.RoundToInt(tilePosition.z));
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
        ResetBuildMarker();
    }

    public void Disable() {
        if (buildMarker != null) {
            Destroy(buildMarker);
        }
        if (wallMarker != null) {
            Destroy(wallMarker);
        }
    }

    private void ResetBuildMarker() {
        if (buildMarker != null) {
            Destroy(buildMarker);
        }
        if (wallMarker != null) {
            Destroy(wallMarker);
        }

        buildMarker = Instantiate(buildMarkerPrefab);
        buildMarker.transform.position = new Vector3(0, -100, 0);
        pressingTile = false;
    }

    private void UpdateBuildMarker(Vector2Int? newTarget) {
        if (newTarget != null) {
            if (newTarget != currentTarget) {
                currentTarget = newTarget;
                SetBuildMarkerPosition(newTarget.Value);
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

    private void SetBuildMarkerPosition(Vector2Int target) {
        buildMarker.transform.position = new Vector3(target.x, 0, target.y);
    }

    private void HandlePlacementHolding(Vector2Int? target) {
        bool validTargets = currentTarget != null && target != null;

        if (validTargets) {
            Vector2Int start = currentTarget.Value;
            Vector2Int end = target.Value;
            Vector2Int firstPos = new Vector2Int(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
            Vector2Int lastPos = new Vector2Int(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));
            
            // TODO: Check money
            // TODO: Check collisions
            bool canPlace = true;

            if (Input.GetMouseButtonUp(0)) {
                if (canPlace) {
                    PlaceWalls(start, firstPos, lastPos);
                }
            } else {
                PlaceWallMarker(start, end, firstPos, lastPos);
                wallMarker.GetComponentInChildren<Renderer>().material =
                    canPlace ? wallMarkerMaterial : wallDenyMaterial;
            }
        }
        
        buildMarker.SetActive(validTargets);
        if (wallMarker != null) {
            wallMarker.SetActive(validTargets);
        }

        if (Input.GetMouseButtonUp(0)) {
            ResetBuildMarker();
        }
    }

    private void PlaceWallMarker(Vector2Int start, Vector2Int end, Vector2Int firstPos, Vector2Int lastPos) {
        int deltaX = lastPos.x - firstPos.x;
        int deltaY = lastPos.y - firstPos.y;
        int length = Mathf.Max(deltaX, deltaY);

        if (wallMarker == null) {
            wallMarker = Instantiate(wallMarkerPrefab);
        }
        Transform markerTransform = wallMarker.transform;

        // Rotate marker
        int angle = deltaX >= deltaY ? 0 : 90;
        markerTransform.rotation = Quaternion.Euler(0, angle, 0);

        // Scale marker
        Vector3 markerScale = markerTransform.localScale;
        markerScale.x = length;
        markerTransform.localScale = markerScale;

        // Position markers
        if (deltaX >= deltaY) {
            markerTransform.position = new Vector3(firstPos.x + length / 2f, 0, start.y);
            buildMarker.transform.position  = new Vector3(end.x, 0, start.y);
        } else {
            markerTransform.position = new Vector3(start.x, 0, firstPos.y + length / 2f);
            buildMarker.transform.position  = new Vector3(start.x, 0, end.y);
        }
    }

    private void PlaceWalls(Vector2Int start, Vector2Int firstPos, Vector2Int lastPos) {
        int deltaX = lastPos.x - firstPos.x;
        int deltaY = lastPos.y - firstPos.y;

        if (deltaX >= deltaY) {
            for (int x = firstPos.x; x < lastPos.x; x++) {
                PropertyController.Instance.property.PlaceWall(x, start.y, WallDirection.NorthEast);
            }
        } else {
            for (int y = firstPos.y; y < lastPos.y; y++) {
                PropertyController.Instance.property.PlaceWall(start.x, y, WallDirection.NorthWest);
            }
        }
    }
}

}