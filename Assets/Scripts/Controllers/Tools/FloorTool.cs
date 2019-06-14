using System;
using Assets.Scripts.Controllers;
using Model.Property;
using UnityEngine;

/// <summary>
/// Tool for build mode that deals with placing and removing floors.
/// </summary>
public class FloorTool : MonoBehaviour, ITool {
    public GameObject buildMarkerPrefab;

    private FloorPreset placingPreset;
    private GameObject buildMarker;
    private TerrainTile targetTile;
    private bool pressingTile;

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (placingPreset == null)
            return;
        TerrainTile terrainTile;
        if (tileIndex.x == -1 && tileIndex.y == -1) {
            terrainTile = null;
        } else {
            terrainTile = PropertyController.Instance.property.GetTerrainTile(tileIndex.x, tileIndex.y);
        }
        if (pressingTile) {
            HandlePlacementHolding(terrainTile);
        } else {
            UpdateBuildMarker(terrainTile);
        }
    }

    public void Enable() {
        placingPreset = null;
    }

    public void Disable() { }

    private void CreateBuildMarker() {
        if (buildMarker != null) {
            Destroy(buildMarker);
        }
        buildMarker = Instantiate(buildMarkerPrefab);
        placingPreset.ApplyToGameObject(buildMarker);
        SetBuildMarkerPosition(0, 0);
    }

    private void UpdateBuildMarker(TerrainTile newTargetTile) {
        if (newTargetTile != null) {
            if (newTargetTile != targetTile) {
                BuildMarkerMoved(newTargetTile);
            }
            if (Input.GetMouseButtonDown(0)) {
                SoundController.Instance.PlaySound(SoundType.DragStart);
                pressingTile = true;
            }
        } else if (targetTile != null) {
            buildMarker.transform.position = new Vector3(0, -100, 0);
            targetTile = null;
        }
    }

    private void BuildMarkerMoved(TerrainTile newTile) {
        targetTile = newTile;
        Vector2Int tilePosition = targetTile.TilePosition;
        SetBuildMarkerPosition(tilePosition.x, tilePosition.y);
    }

    private bool CanPlaceFloors(RectInt floorRect) {
        return floorRect.width > 0 && floorRect.height > 0;
    }

    private void SetBuildMarkerPosition(int x, int y) {
        buildMarker.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
    }

    private void HandlePlacementHolding(TerrainTile terrainTile) {
        RectInt dragRect = HandleFloorDragging(terrainTile);
        int costs = CalculateCosts(dragRect, placingPreset);
        bool canPlace = CanPlaceFloors(dragRect);
        buildMarker.SetActive(canPlace);
        if (Input.GetMouseButtonUp(0)) {
            if (dragRect.width > 0) {
                if (Input.GetKey(KeyCode.LeftControl)) {
                    if (SellFloors(dragRect)) {
                        SoundController.Instance.PlaySound(SoundType.PlaceFloor);
                    }
                } else if (canPlace && costs <= MoneyController.Instance.Funds) {
                    MoneyController.Instance.ChangeFunds(-costs);
                    SoundController.Instance.PlaySound(SoundType.PlaceFloor);
                    PlaceFloors(dragRect, placingPreset);
                } else {
                    SoundController.Instance.PlaySound(SoundType.Deny);
                }
            }
            CreateBuildMarker();
            pressingTile = false;
        }
    }

    private RectInt HandleFloorDragging(TerrainTile terrainTile) {
        if (terrainTile == null)
            return new RectInt(0, 0, 0, 0);
        Vector2Int dragPosition = terrainTile.TilePosition;
        Vector2Int tilePosition = targetTile.TilePosition;
        int x1 = Math.Min(tilePosition.x, dragPosition.x);
        int x2 = Math.Max(tilePosition.x, dragPosition.x) + 1;
        int y1 = Math.Min(tilePosition.y, dragPosition.y);
        int y2 = Math.Max(tilePosition.y, dragPosition.y) + 1;
        RectInt floorRect = new RectInt();
        floorRect.SetMinMax(new Vector2Int(x1, y1), new Vector2Int(x2, y2));
        Transform markerTransform = buildMarker.transform;
        markerTransform.localScale = new Vector3(floorRect.width, markerTransform.localScale.y, floorRect.height);
        markerTransform.position = new Vector3(
            floorRect.x + floorRect.width / 2f,
            markerTransform.position.y,
            floorRect.y + floorRect.height / 2f
        );
        buildMarker.GetComponent<Renderer>().material.mainTextureScale = new Vector2(floorRect.width, floorRect.height);
        return floorRect;
    }

    private int CalculateCosts(RectInt floorRect, FloorPreset floorPreset) {
        int costs = floorPreset.price * floorRect.width * floorRect.height;
        Property property = PropertyController.Instance.property;
        for (int x = floorRect.x; x < floorRect.xMax; x++) {
            for (int y = floorRect.y; y < floorRect.yMax; y++) {
                FloorTile currentFloor = property.GetFloorTile(x, y);
                if (currentFloor != null) {
                    costs -= currentFloor.preset.GetSellValue();
                }
            }
        }
        return costs;
    }

    private void PlaceFloors(RectInt floorRect, FloorPreset floorPreset) {
        for (int x = floorRect.x; x < floorRect.xMax; x++) {
            for (int y = floorRect.y; y < floorRect.yMax; y++) {
                PropertyController.Instance.property.PlaceFloor(x, y, floorPreset);
            }
        }
        BuildMarkerMoved(targetTile);
    }

    private bool SellFloors(RectInt floorRect) {
        Property property = PropertyController.Instance.property;
        bool sold = false;
        for (int x = floorRect.x; x < floorRect.xMax; x++) {
            for (int y = floorRect.y; y < floorRect.yMax; y++) {
                FloorTile currentFloor = property.GetFloorTile(x, y);
                if (currentFloor == null)
                    continue;
                MoneyController.Instance.ChangeFunds(currentFloor.preset.GetSellValue());
                PropertyController.Instance.property.RemoveFloor(currentFloor);
                sold = true;
            }
        }
        return sold;
    }

    public void OnCatalogSelect(CatalogItem item, int skin) {
        FloorPreset floorPreset = item as FloorPreset;
        if (floorPreset == null) {
            Debug.LogWarning(item + " is not a FloorPreset, cannot be set to FloorTool.");
            return;
        }
        placingPreset = floorPreset;
        CreateBuildMarker();
    }
}