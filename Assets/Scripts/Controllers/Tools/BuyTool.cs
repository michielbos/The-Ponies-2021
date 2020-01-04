using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Playmode;
using Controllers.Singletons;
using JetBrains.Annotations;
using Model.Property;
using UnityEngine;
using Util;

namespace Controllers.Tools {

/// <summary>
/// Tool for buy/build mode that deals with buying, moving and selling furniture.
/// </summary>
public class BuyTool : MonoBehaviour, ITool {
    private int defaultLayer;
    private int terrainLayer;
    private int slotRaycastLayer;

    public BuyToolMarker buildMarkerPrefab;

    private FurniturePreset placingPreset;
    private int placingSkin;

    private PropertyObject movingObject;

    private BuyToolMarker buildMarker;
    private IObjectSlot targetSlot;
    private bool pressingTile;
    private bool canPlace;

    private void Start() {
        terrainLayer = LayerMask.GetMask("Terrain");
        defaultLayer = LayerMask.GetMask("Default");
        slotRaycastLayer = LayerMask.GetMask("Terrain", "Default");
    }

    public void OnDisable() {
        ClearSelection();
    }

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (buildMarker != null) {
            buildMarker.UpdateMarker();
            if (pressingTile) {
                HandlePlacementHolding();
            } else {
                UpdateBuildMarker(false);
            }
        } else {
            HandleHovering();
        }

        if (Input.GetKey(KeyCode.Delete)) {
            SellSelection();
        }
    }

    public void OnCatalogSelect(CatalogItem item, int skin) {
        FurniturePreset furniturePreset = item as FurniturePreset;
        if (furniturePreset == null) {
            Debug.LogWarning(item + " is not a FurniturePreset, cannot be set to BuyTool.");
            return;
        }

        ClearSelection();
        placingPreset = furniturePreset;
        placingSkin = skin;
        CreateBuildMarker(placingPreset, skin, ObjectRotation.SouthEast);
    }

    public void Enable() {
        pressingTile = false;
        movingObject = null;
        buildMarker = null;
        placingPreset = null;
    }

    public void Disable() {
        if (buildMarker != null) {
            if (movingObject != null) {
                movingObject.SetVisibility(true);
            }

            Destroy(buildMarker.gameObject);
        }
    }

    private void CreateBuildMarker(FurniturePreset preset, int skin, ObjectRotation rotation) {
        buildMarker = Instantiate(buildMarkerPrefab);
        buildMarker.Init(preset, skin, rotation);
    }
    
    private FurniturePreset GetMovingPreset() {
        if (placingPreset != null)
            return placingPreset;
        return movingObject != null ? movingObject.preset : null;
    }

    private void UpdateBuildMarker(bool ignoreClick, bool force = false) {
        IObjectSlot newSlot = GetSlotUnderCursorFor(GetMovingPreset().placementType);
        if (newSlot != null) {
            if (newSlot != targetSlot ||  force) {
                BuildMarkerMoved(newSlot);
            }

            if (!ignoreClick && Input.GetMouseButtonDown(0)) {
                pressingTile = true;
            }
        } else if (targetSlot != null) {
            buildMarker.transform.position = new Vector3(0, -100, 0);
            targetSlot = null;
        }
    }

    [CanBeNull]
    private IObjectSlot GetSlotUnderCursorFor(PlacementType placementType) {
        if (placementType == PlacementType.Surface ||
            placementType == PlacementType.GroundOrSurface ||
            placementType == PlacementType.Counter)
            return GetSlotUnderCursor();
        return GetTileUnderCursor();
    }

    [CanBeNull]
    private IObjectSlot GetSlotUnderCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (HUDController.GetInstance().IsMouseOverGui() ||
            !Physics.Raycast(ray, out RaycastHit hit, 1000, slotRaycastLayer))
            return null;
        PropertyObject propertyObject = hit.collider.GetComponentInParent<PropertyObject>();
        if (propertyObject != null) {
            return propertyObject.GetClosestSurfaceSlot(hit.point);
        }
        return hit.collider.GetComponent<TerrainTile>();
    }

    [CanBeNull]
    private TerrainTile GetTileUnderCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!HUDController.GetInstance().IsMouseOverGui() &&
            Physics.Raycast(ray, out RaycastHit hit, 1000, terrainLayer))
            return hit.collider.GetComponent<TerrainTile>();
        return null;
    }

    [CanBeNull]
    private PropertyObject GetObjectUnderCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!HUDController.GetInstance().IsMouseOverGui() &&
            Physics.Raycast(ray, out RaycastHit hit, 1000, defaultLayer))
            return hit.collider.GetComponentInParent<PropertyObject>();
        return null;
    }

    private void BuildMarkerMoved(IObjectSlot newSlot) {
        targetSlot = newSlot;
        buildMarker.transform.position = newSlot.SlotPosition;
        UpdateCanPlace();
    }

    private void UpdateCanPlace() {
        canPlace = CanPlaceObject();
        buildMarker.SetCanPlace(canPlace);
    }

    private bool CanPlaceObject() {
        if (targetSlot == null)
            return false;
        FurniturePreset movingPreset = GetMovingPreset();
        ICollection<Vector2Int> requiredTiles = buildMarker.OccupiedTiles;
        List<Wall> walls = PropertyController.Instance.property.GetOccupiedWalls(requiredTiles);

        bool canPlace = placingPreset == null || MoneyController.Instance.CanAfford(placingPreset.price);

        if (!canPlace)
            return false;

        Property property = PropertyController.Instance.property;

        // Check if the required walls for Wall and ThroughWall types are in place.
        IEnumerable<TileBorder> tileBorders =
            movingPreset.GetRequiredWallBorders(requiredTiles, buildMarker.MarkerRotation);
        if (!property.AllBordersContainWalls(tileBorders)) {
            return false;
        }

        if (!CheatsController.Instance.moveObjectsMode && !(targetSlot is SurfaceSlot)) {
            List<PropertyObject> tileObjects = PropertyController.Instance.property.GetObjectsOnTiles(requiredTiles);
            if (tileObjects.Any(tileObject => tileObject != movingObject)) {
                return false;
            }

            if (walls.Count > 0) {
                if (movingPreset.placementType != PlacementType.ThroughWall) {
                    return false;
                }
                if (walls.Select(wall => wall.TileBorder).Except(tileBorders).Any()) {
                    return false;
                }
            }
        }

        if (requiredTiles.Any(tile =>
            tile.x < 0 || tile.y < 0 || tile.x >= property.TerrainWidth || tile.y >= property.TerrainHeight)) {
            return false;
        }

        switch (movingPreset.placementType) {
            case PlacementType.Ground:
            case PlacementType.GroundOrSurface:
            case PlacementType.Surface:
            case PlacementType.Counter:
            case PlacementType.Wall:
            case PlacementType.ThroughWall:
                return true;
            case PlacementType.Terrain:
                return requiredTiles.Count(tile => property.GetFloorTile(tile.x, tile.y) != null) == 0;
            case PlacementType.Floor:
                return requiredTiles.Count(tile => property.GetFloorTile(tile.x, tile.y) == null) == 0;
            case PlacementType.Ceiling:
                return requiredTiles.All(tile => property.IsInsideRoom(tile));
            default:
                return false;
        }
    }

    private void HandlePlacementHolding() {
        TerrainTile tileUnderCursor = GetTileUnderCursor();
        if (tileUnderCursor != null) {
            if (buildMarker.HandleDragRotation(tileUnderCursor.TilePosition)) {
                UpdateCanPlace();
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            PlaceObject();
            pressingTile = false;
        }
    }

    private void PlaceObject() {
        if (!canPlace) {
            SoundController.Instance.PlaySound(SoundType.Deny);
        } else if (movingObject != null) {
            SoundController.Instance.PlaySound(SoundType.Place);
            movingObject.ClearParent();
            targetSlot.PlaceObject(movingObject);
            movingObject.Rotation = buildMarker.MarkerRotation;
            ClearSelection();
        } else {
            SoundController.Instance.PlaySound(SoundType.Buy);
            MoneyController.Instance.ChangeFunds(-placingPreset.price);
            PropertyController.Instance.property.PlacePropertyObject(targetSlot, buildMarker.MarkerRotation, 
                placingPreset, placingSkin);
            if (Input.GetKey(KeyCode.LeftShift)) {
                BuildMarkerMoved(targetSlot);
            } else {
                ClearSelection();
            }
        }
    }

    private void HandleHovering() {
        PropertyObject propertyObject = GetObjectUnderCursor();
        if (propertyObject == null)
            return;
        //TODO: Highlight object if it can be picked up.
        if (!Input.GetMouseButtonDown(0))
            return;
        if (propertyObject.preset.pickupable || CheatsController.Instance.moveObjectsMode) {
            PickUpObject(propertyObject);
        } else {
            SoundController.Instance.PlaySound(SoundType.Deny);
        }
    }

    private void PickUpObject(PropertyObject propertyObject) {
        ClearSelection();
        movingObject = propertyObject;
        movingObject.SetVisibility(false);
        CreateBuildMarker(movingObject.preset, movingObject.skin, movingObject.Rotation);
        UpdateBuildMarker(propertyObject, true);
    }

    private void SellSelection() {
        if (movingObject == null) {
            ClearSelection();
        } else if (movingObject.preset.sellable || CheatsController.Instance.moveObjectsMode) {
            SoundController.Instance.PlaySound(SoundType.Sell);
            PropertyController.Instance.property.RemovePropertyObject(movingObject);
            MoneyController.Instance.ChangeFunds(movingObject.value);
            ClearSelection();
        } else {
            SoundController.Instance.PlaySound(SoundType.Deny);
        }
    }

    private void ClearSelection() {
        placingPreset = null;
        if (movingObject != null) {
            movingObject.SetVisibility(true);
            movingObject = null;
        }

        if (buildMarker != null) {
            Destroy(buildMarker.gameObject);
            buildMarker = null;
        }
    }
}

}