using System.Collections.Generic;
using System.Linq;
using Controllers.Singletons;
using Model.Property;
using UnityEngine;

namespace Controllers.Tools {

/// <summary>
/// Tool for build mode that deals with placing and removing walls.
/// </summary>
public class WallTool : MonoBehaviour, ITool {
    private const int SellValue = 10;
    
    public GameObject buildMarkerPrefab;
    public GameObject wallMarkerPrefab;
    public Material wallMarkerMaterial;
    public Material wallDenyMaterial;
    public Material wallDemolishMaterial;

    private CatalogItem wallPreset;

    private GameObject buildMarker;
    private readonly GameObject[] wallMarkers = new GameObject[4];
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
        DestroyWallMarkers();
    }

    private void DestroyWallMarkers() {
        foreach (GameObject wallMarker in wallMarkers) {
            if (wallMarker != null) {
                Destroy(wallMarker);
            }
        }
    }
    
    /// <summary>
    /// Destroy all wall markers, except the main one.
    /// </summary>
    private void DestroyRoomWallMarkers() {
        for (int i = 1; i < wallMarkers.Length; i++) {
            if (i == 0)
                continue;
            if (wallMarkers[i] != null) {
                Destroy(wallMarkers[i]);
            }
        }
    }

    private void ResetBuildMarker() {
        if (buildMarker != null) {
            Destroy(buildMarker);
        }
        DestroyWallMarkers();

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
        bool destroyMode = Input.GetKey(KeyCode.LeftControl);
        bool roomMode = !destroyMode && Input.GetKey(KeyCode.LeftShift);

        if (validTargets) {
            Vector2Int start = currentTarget.Value;
            Vector2Int end = target.Value;
            Vector2Int firstPos = new Vector2Int(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
            Vector2Int lastPos = new Vector2Int(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));
            int deltaX = lastPos.x - firstPos.x;
            int deltaY = lastPos.y - firstPos.y;

            List<TileBorder> wallPositions;
            if (roomMode && firstPos.x != lastPos.x && firstPos.y != lastPos.y) {
                wallPositions = GetRoomTileBorders(firstPos, lastPos);
                PlaceRoomWallMarkers(firstPos, lastPos);
                buildMarker.transform.position = new Vector3(end.x, 0, end.y);
            } else {
                wallPositions = GetTileBorders(start, firstPos, lastPos);
                PlaceWallMarker(0, start, firstPos, lastPos);
                DestroyRoomWallMarkers();
                buildMarker.transform.position = deltaX >= deltaY ? new Vector3(end.x, 0, start.y) : 
                    new Vector3(start.x, 0, end.y);
            }

            Property property = PropertyController.Instance.property;
            if (destroyMode) {
                wallPositions.RemoveAll(border => property.GetWall(border) == null);
                bool canDestroy = property.CanRemoveWalls(wallPositions);
                
                SetWallMarkerMaterial(canDestroy ? wallDemolishMaterial : wallDenyMaterial);
                
                if (Input.GetMouseButtonUp(0) && canDestroy) {
                    SellWalls(wallPositions);
                    SoundController.Instance.PlaySound(SoundType.PlaceWall);
                }
            } else {
                ExcludeExistingWalls(wallPositions);

                int cost = wallPositions.Count * wallPreset.price;
                bool canAfford = MoneyController.Instance.CanAfford(cost);
                bool collides = !CheatsController.Instance.moveObjectsMode && 
                                property.GetObjectsOnBorders(wallPositions).Any();
                bool canPlace = canAfford && !collides;
                
                SetWallMarkerMaterial(canPlace ? wallMarkerMaterial : wallDenyMaterial);

                if (Input.GetMouseButtonUp(0)) {
                    if (canPlace) {
                        MoneyController.Instance.ChangeFunds(-cost);
                        PlaceWalls(wallPositions);
                        SoundController.Instance.PlaySound(SoundType.PlaceWall);
                    } else {
                        SoundController.Instance.PlaySound(SoundType.Deny);
                    }
                }
            }
        }

        buildMarker.SetActive(validTargets);
        if (!validTargets) {
            DestroyWallMarkers();
        }

        if (Input.GetMouseButtonUp(0)) {
            ResetBuildMarker();
        }
    }

    private void SetWallMarkerMaterial(Material material) {
        foreach (GameObject wallMarker in wallMarkers) {
            if (wallMarker != null) {
                wallMarker.GetComponentInChildren<Renderer>().material = material;
            }
        }
    }

    /// <summary>
    /// Place ghosted walls to show the player where the walls will be built.
    /// </summary>
    private void PlaceWallMarker(int index, Vector2Int start, Vector2Int firstPos, Vector2Int lastPos) {
        int deltaX = lastPos.x - firstPos.x;
        int deltaY = lastPos.y - firstPos.y;
        int length = Mathf.Max(deltaX, deltaY);

        if (wallMarkers[index] == null) {
            wallMarkers[index] = Instantiate(wallMarkerPrefab);
        }
        Transform markerTransform = wallMarkers[index].transform;

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
        } else {
            markerTransform.position = new Vector3(start.x, 0, firstPos.y + length / 2f);
        }
    }
    
    private void PlaceWallMarker(int index, Vector2Int firstPos, Vector2Int lastPos) {
        PlaceWallMarker(index, firstPos, firstPos, lastPos);
    }

    /// <summary>
    /// Place ghosted walls in a room shape.
    /// </summary>
    private void PlaceRoomWallMarkers(Vector2Int firstPos, Vector2Int lastPos) {
        PlaceWallMarker(0, firstPos, new Vector2Int(firstPos.x, lastPos.y));
        PlaceWallMarker(1, firstPos, new Vector2Int(lastPos.x, firstPos.y));
        PlaceWallMarker(2, new Vector2Int(firstPos.x, lastPos.y), lastPos);
        PlaceWallMarker(3, new Vector2Int(lastPos.x, firstPos.y), lastPos);
        
    }

    /// <summary>
    /// Get a list of all tile borders when drawing a line from the firstPos to the lastPos.
    /// </summary>
    /// <param name="start">The original point where the player started drawing the wall.</param>
    /// <param name="firstPos">The lower-left point of the two targets.</param>
    /// <param name="lastPos">The upper-right point of the two targets.</param>
    private List<TileBorder> GetTileBorders(Vector2Int start, Vector2Int firstPos, Vector2Int lastPos) {
        int deltaX = lastPos.x - firstPos.x;
        int deltaY = lastPos.y - firstPos.y;
        List<TileBorder> walls = new List<TileBorder>();

        if (deltaX >= deltaY) {
            for (int x = firstPos.x; x < lastPos.x; x++) {
                walls.Add(new TileBorder(x, start.y, WallDirection.NorthEast));
            }
            return walls;
        } else {
            for (int y = firstPos.y; y < lastPos.y; y++) {
                walls.Add(new TileBorder(start.x, y, WallDirection.NorthWest));
            }
            return walls;
        }
    }
    
    /// <summary>
    /// Get a list of all tile borders when drawing a square room from the firstPos to the lastPos.
    /// </summary>
    /// <param name="firstPos">The lower-left point of the two targets.</param>
    /// <param name="lastPos">The upper-right point of the two targets.</param>
    private List<TileBorder> GetRoomTileBorders(Vector2Int firstPos, Vector2Int lastPos) {
        List<TileBorder> walls = new List<TileBorder>();
        
        for (int x = firstPos.x; x < lastPos.x; x++) {
            walls.Add(new TileBorder(x, firstPos.y, WallDirection.NorthEast));
            walls.Add(new TileBorder(x, lastPos.y, WallDirection.NorthEast));
        }
        
        for (int y = firstPos.y; y < lastPos.y; y++) {
            walls.Add(new TileBorder(firstPos.x, y, WallDirection.NorthWest));
            walls.Add(new TileBorder(lastPos.x, y, WallDirection.NorthWest));
        }
        
        return walls;
    }

    /// <summary>
    /// Remove wall positions of walls that overlap with existing walls on the property.
    /// </summary>
    private void ExcludeExistingWalls(List<TileBorder> wallPositions) {
        Property property = PropertyController.Instance.property;
        wallPositions.RemoveAll(tileBorder => property.GetWall(tileBorder));
    }

    /// <summary>
    /// Place the given walls on the property.
    /// </summary>
    private void PlaceWalls(List<TileBorder> walls) {
        foreach (TileBorder wall in walls) {
            PropertyController.Instance.property.PlaceWall(wall.x, wall.y, wall.wallDirection, false);
        }
        PropertyController.Instance.property.UpdateRooms();
    }
    
    /// <summary>
    /// Sell any walls on the given tile borders.
    /// </summary>
    private void SellWalls(List<TileBorder> borders) {
        Property property = PropertyController.Instance.property;
        
        foreach (TileBorder border in borders) {
            Wall wall = property.GetWall(border);
            MoneyController.Instance.ChangeFunds(SellValue);
            property.RemoveWall(wall, false);
        }
        property.UpdateRooms();
    }
}

}