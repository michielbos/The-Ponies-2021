using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Controllers.Singletons;
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

            List<WallPosition> wallPositions = GetWallsToPlace(start, firstPos, lastPos);
            ExcludeExistingWalls(wallPositions);

            int cost = wallPositions.Count * wallPreset.price;
            bool canAfford = MoneyController.Instance.CanAfford(cost);
            // TODO: Check collisions
            bool canPlace = canAfford;

            if (Input.GetMouseButtonUp(0)) {
                if (canPlace) {
                    MoneyController.Instance.ChangeFunds(-cost);
                    PlaceWalls(wallPositions);
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

    /// <summary>
    /// Place ghosted walls to show the player where the walls will be built.
    /// </summary>
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
            buildMarker.transform.position = new Vector3(end.x, 0, start.y);
        } else {
            markerTransform.position = new Vector3(start.x, 0, firstPos.y + length / 2f);
            buildMarker.transform.position = new Vector3(start.x, 0, end.y);
        }
    }

    /// <summary>
    /// Get a list of all walls to place when drawing a line from the firstPos to the lastPos.
    /// </summary>
    /// <param name="start">The original point where the player started drawing the wall.</param>
    /// <param name="firstPos">The lower-left point of the two targets.</param>
    /// <param name="lastPos">The upper-right point of the two targets.</param>
    private List<WallPosition> GetWallsToPlace(Vector2Int start, Vector2Int firstPos, Vector2Int lastPos) {
        int deltaX = lastPos.x - firstPos.x;
        int deltaY = lastPos.y - firstPos.y;
        List<WallPosition> walls = new List<WallPosition>();

        if (deltaX >= deltaY) {
            for (int x = firstPos.x; x < lastPos.x; x++) {
                walls.Add(new WallPosition(x, start.y, WallDirection.NorthEast));
            }
            return walls;
        } else {
            for (int y = firstPos.y; y < lastPos.y; y++) {
                walls.Add(new WallPosition(start.x, y, WallDirection.NorthWest));
            }
            return walls;
        }
    }

    /// <summary>
    /// Remove wall positions of walls that overlap with existing walls on the property.
    /// </summary>
    private void ExcludeExistingWalls(List<WallPosition> wallPositions) {
        Property property = PropertyController.Instance.property;
        wallPositions.RemoveAll(wall => property.GetWall(wall.x, wall.y, wall.wallDirection));
    }

    /// <summary>
    /// Place the given walls on the property.
    /// </summary>
    private void PlaceWalls(List<WallPosition> walls) {
        foreach (WallPosition wall in walls) {
            PropertyController.Instance.property.PlaceWall(wall.x, wall.y, wall.wallDirection);
        }
    }

    private struct WallPosition {
        public int x;
        public int y;
        public WallDirection wallDirection;

        public WallPosition(int x, int y, WallDirection wallDirection) {
            this.x = x;
            this.y = y;
            this.wallDirection = wallDirection;
        }
    }
}

}