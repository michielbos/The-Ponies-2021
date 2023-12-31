﻿using System;
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
    /// <summary>
    /// The number of pixels to drag the mouse away from the press position before listening to rotations.
    /// </summary>
    private const int DragRotationMargin = 8;
    private int defaultLayer;
    private int terrainLayer;
    private int slotRaycastLayer;

    public BuyToolMarker buildMarkerPrefab;

    private FurniturePreset placingPreset;
    private int placingSkin;

    private PropertyObject movingObject;

    private BuyToolMarker buildMarker;
    private IObjectSlot targetSlot;
    // If the mouse is being pressed above a tile.
    private bool pressingTile;
    // The screen position where the mouse started being held.
    private Vector2 startPressPosition;
    private bool canPlace;

    private void Start() {
        terrainLayer = LayerMask.GetMask("Terrain");
        defaultLayer = LayerMask.GetMask("Default");
        slotRaycastLayer = LayerMask.GetMask("Terrain", "Default");
    }

    public void OnDisable() {
        ClearSelection(true);
    }

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (buildMarker != null) {
            HandleRotationButtons();
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

        ClearSelection(true);
        placingPreset = furniturePreset;
        placingSkin = skin;
        buildMarker = Instantiate(buildMarkerPrefab);
        buildMarker.InitBuy(placingPreset, skin, ObjectRotation.SouthEast);
    }

    public void Enable() {
        pressingTile = false;
        movingObject = null;
        buildMarker = null;
        placingPreset = null;
    }

    public void Disable() {
        ClearSelection(true);
    }

    private FurniturePreset GetMovingPreset() {
        if (placingPreset != null)
            return placingPreset;
        return movingObject != null ? movingObject.preset : null;
    }

    private void UpdateBuildMarker(bool ignoreClick, bool force = false) {
        IObjectSlot newSlot = GetSlotUnderCursorFor(GetMovingPreset().placementType);
        if (newSlot != null) {
            if (newSlot != targetSlot || force) {
                BuildMarkerMoved(newSlot);
            }

            if (!ignoreClick && Input.GetMouseButtonDown(0)) {
                pressingTile = true;
                startPressPosition = Input.mousePosition;
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
        buildMarker.OnNewSlot(newSlot);
        buildMarker.transform.position = newSlot.SlotPosition;
        SurfaceSlot surfaceSlot = newSlot as SurfaceSlot;
        if (surfaceSlot != null) {
            buildMarker.MarkerRotation = surfaceSlot.SlotOwner.Rotation;
        }
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

        SurfaceSlot surfaceSlot = targetSlot as SurfaceSlot;
        if (surfaceSlot != null) {
            // Skip all other checks, because a surface object can be placed as long as the slot is free.
            return surfaceSlot.SlotObject == null || surfaceSlot.SlotObject == movingObject;
        }
        
        if (!CheatsController.Instance.moveObjectsMode) {
            List<PropertyObject> tileObjects = PropertyController.Instance.property.GetObjectsOnTiles(requiredTiles);
            
            // Check if there is a collision.
            // The moving object and its children are ignored in this check.
            if (movingObject != null) {
                if (tileObjects.Except(movingObject.Children).Any(tileObject => tileObject != movingObject))
                    return false;
            } else if (tileObjects.Any()) {
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
            Vector2 relativeMouse = (Vector2) Input.mousePosition - startPressPosition;
            if (Mathf.Abs(relativeMouse.x) >= DragRotationMargin || Mathf.Abs(relativeMouse.y) >= DragRotationMargin) {
                if (buildMarker.HandleDragRotation(relativeMouse)) {
                    UpdateCanPlace();
                }
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
            ClearSelection(false);
        } else {
            SoundController.Instance.PlaySound(SoundType.Buy);
            MoneyController.Instance.ChangeFunds(-placingPreset.price);
            PropertyController.Instance.property.PlacePropertyObject(targetSlot, buildMarker.MarkerRotation, 
                placingPreset, placingSkin);
            if (Input.GetKey(KeyCode.LeftShift)) {
                BuildMarkerMoved(targetSlot);
            } else {
                ClearSelection(false);
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
        if (propertyObject.Pickupable || CheatsController.Instance.moveObjectsMode) {
            PickUpObject(propertyObject);
        } else {
            SoundController.Instance.PlaySound(SoundType.Deny);
        }
    }

    private void PickUpObject(PropertyObject propertyObject) {
        ClearSelection(true);
        movingObject = propertyObject;
        buildMarker = Instantiate(buildMarkerPrefab);
        buildMarker.InitMove(propertyObject);
        UpdateBuildMarker(propertyObject, true);
        propertyObject.OnPickedUp();
    }

    private void SellSelection() {
        if (movingObject == null) {
            ClearSelection(false);
        } else if (!movingObject.Children.Any() &&
                   (movingObject.preset.sellable || CheatsController.Instance.moveObjectsMode)) {
            SoundController.Instance.PlaySound(SoundType.Sell);
            PropertyController.Instance.property.RemovePropertyObject(movingObject);
            MoneyController.Instance.ChangeFunds(movingObject.value);
            ClearSelection(false);
        } else {
            SoundController.Instance.PlaySound(SoundType.Deny);
        }
    }

    /// <summary>
    /// Clear the current selection.
    /// </summary>
    /// <param name="canceled">Whether it was not directly cleared by the user (for example when switching a tab)</param>
    private void ClearSelection(bool canceled) {
        placingPreset = null;
        movingObject = null;

        if (buildMarker != null) {
            buildMarker.Finish(canceled);
            buildMarker = null;
        }
    }
    
    private void HandleRotationButtons() {
        if (buildMarker == null)
            return;
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Comma)) {
            SoundController.Instance.PlaySound(SoundType.Rotate);
            buildMarker.MarkerRotation = buildMarker.MarkerRotation.RotateCounterClockwise();
            UpdateCanPlace();
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Period)) {
            SoundController.Instance.PlaySound(SoundType.Rotate);
            buildMarker.MarkerRotation = buildMarker.MarkerRotation.RotateClockwise();
            UpdateCanPlace();
        }
    }
}

}