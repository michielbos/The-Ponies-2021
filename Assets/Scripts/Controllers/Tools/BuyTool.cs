using System;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using JetBrains.Annotations;
using Model.Property;
using UnityEngine;

/// <summary>
/// Tool for buy/build mode that deals with buying, moving and selling furniture.
/// </summary>
/// 
[CreateAssetMenu(fileName = "BuyTool", menuName = "Tools/Buy Tool", order = 10)]
public class BuyTool : ScriptableObject, ITool {
    private const int LAYER_TERRAIN = 8;

    public GameObject buildMarkerPrefab;
    public GameObject buyMarkingPrefab;
    public Material buyMarkingNormalMaterial;
    public Material buyMarkingDisallowedMaterial;

    private FurniturePreset placingPreset;
    private int placingSkin;

    private PropertyObject movingObject;

    private GameObject buildMarker;
    private ObjectRotation markerRotation = ObjectRotation.SouthEast;
    private readonly List<GameObject> buyMarkings;
    private TerrainTile targetTile;
    private bool pressingTile;
    private bool canPlace;

    public ObjectRotation MarkerRotation {
        get { return markerRotation; }
        set {
            markerRotation = value;
            if (buildMarker != null) {
                buildMarker.transform.eulerAngles = ObjectRotationUtil.GetRotationVector(MarkerRotation);
            }
        }
    }

    public BuyTool() {
        buyMarkings = new List<GameObject>();
    }

    public void OnDisable() {
        ClearSelection();
    }

    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) {
        if (GetMovingPreset() != null) {
            if (pressingTile) {
                HandlePlacementHolding();
            } else {
                UpdateBuildMarker(false);
            }
            HandleRotationButtons();
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
        CreateBuildMarker(placingPreset);
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
            Destroy(buildMarker);
        }
    }

    private void CreateBuildMarker(FurniturePreset preset) {
        buildMarker = Instantiate(buildMarkerPrefab);
        preset.ApplyToGameObject(buildMarker, placingSkin, true);
        SetBuildMarkerPosition(0, 0);
        PlaceBuyMarkings(0, 0);
        buildMarker.transform.eulerAngles = ObjectRotationUtil.GetRotationVector(MarkerRotation);
    }

    private void PlaceBuyMarkings(int x, int y) {
        foreach (Vector2Int tile in GetMovingPreset().occupiedTiles) {
            buyMarkings.Add(Instantiate(buyMarkingPrefab, new Vector3(x + tile.x + 0.5f, 0.01f, y + tile.y + 0.5f),
                buyMarkingPrefab.transform.rotation, buildMarker.transform));
        }
    }

    private FurniturePreset GetMovingPreset() {
        if (placingPreset != null)
            return placingPreset;
        return movingObject != null ? movingObject.preset : null;
    }

    private void RemoveBuyMarkings() {
        foreach (GameObject buyMarking in buyMarkings) {
            Destroy(buyMarking);
        }
        buyMarkings.Clear();
    }

    private void UpdateBuildMarker(bool ignoreClick) {
        TerrainTile newTargetTile = GetTileUnderCursor();
        if (newTargetTile != null) {
            if (newTargetTile != targetTile) {
                BuildMarkerMoved(newTargetTile);
            }
            if (!ignoreClick && Input.GetMouseButtonDown(0)) {
                pressingTile = true;
            }
        } else if (targetTile != null) {
            buildMarker.transform.position = new Vector3(0, -100, 0);
            targetTile = null;
        }
    }

    [CanBeNull]
    private TerrainTile GetTileUnderCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!HUDController.GetInstance().IsMouseOverGui() && Physics.Raycast(ray, out hit, 1000, 1 << LAYER_TERRAIN)) {
            return hit.collider.GetComponent<TerrainTile>();
        }
        return null;
    }

    private void BuildMarkerMoved(TerrainTile newTile) {
        targetTile = newTile;
        canPlace = CanPlaceObject();
        foreach (GameObject buyMarking in buyMarkings) {
            buyMarking.GetComponent<Renderer>().material =
                canPlace ? buyMarkingNormalMaterial : buyMarkingDisallowedMaterial;
        }
        Vector2Int tilePosition = targetTile.TilePosition;
        SetBuildMarkerPosition(tilePosition.x, tilePosition.y);
    }

    private bool CanPlaceObject() {
        FurniturePreset movingPreset = GetMovingPreset();
        Vector2Int[] requiredTiles = movingPreset.GetOccupiedTiles(targetTile.TilePosition);
        List<PropertyObject> occupyingObjects = PropertyController.Instance.property.GetObjectsOnTiles(requiredTiles);
        bool canPlace = placingPreset == null || placingPreset.price <= MoneyController.Instance.Funds;
        if (canPlace && !CheatsController.Instance.moveObjectsMode) {
            foreach (PropertyObject occupyingObject in occupyingObjects) {
                if (occupyingObject != movingObject)
                    return false;
            }
        }
        //TODO: Check for floors, walls, tables, etc.
        return canPlace && movingPreset.AllowsPlacement(PlacementRestriction.Terrain);
    }

    private void SetBuildMarkerPosition(int x, int y) {
        FurniturePreset preset = GetMovingPreset();
        buildMarker.transform.position = new Vector3(x + 0.5f, 0, y + 0.5f);
        preset.AdjustToTiles(buildMarker.transform);
    }

    private void HandlePlacementHolding() {
        TerrainTile newTargetTile = GetTileUnderCursor();
        if (newTargetTile != null) {
            Vector2Int diff = targetTile.TilePosition - newTargetTile.TilePosition;
            ObjectRotation newRotation = MarkerRotation;
            if (diff.x != 0 && Math.Abs(diff.x) > Math.Abs(diff.y)) {
                newRotation = diff.x > 0 ? ObjectRotation.SouthWest : ObjectRotation.NorthEast;
            } else if (diff.y != 0 && Math.Abs(diff.y) > Math.Abs(diff.x)) {
                newRotation = diff.y > 0 ? ObjectRotation.SouthEast : ObjectRotation.NorthWest;
            }
            if (newRotation != MarkerRotation) {
                SoundController.Instance.PlaySound(SoundType.Rotate);
                MarkerRotation = newRotation;
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
            movingObject.TilePosition = targetTile.TilePosition;
            movingObject.Rotation = MarkerRotation;
            ClearSelection();
        } else {
            SoundController.Instance.PlaySound(SoundType.Buy);
            MoneyController.Instance.ChangeFunds(-placingPreset.price);
            Vector2Int tilePosition = targetTile.TilePosition;
            PropertyController.Instance.property.PlacePropertyObject(tilePosition.x, tilePosition.y, MarkerRotation, 
                placingPreset, placingSkin);
            if (Input.GetKey(KeyCode.LeftShift)) {
                BuildMarkerMoved(targetTile);
            } else {
                ClearSelection();
            }
        }
    }

    private void HandleHovering() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (HUDController.GetInstance().IsMouseOverGui() || !Physics.Raycast(ray, out hit, 1000))
            return;
        PropertyObject propertyObject = hit.collider.transform.GetComponentInParent<PropertyObject>();
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
        CreateBuildMarker(movingObject.preset);
        targetTile = null;
        MarkerRotation = movingObject.Rotation; 
        UpdateBuildMarker(true);
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
            Destroy(buildMarker);
            buildMarker = null;
        }
        RemoveBuyMarkings();
    }

    private void HandleRotationButtons() {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Comma)) {
            SoundController.Instance.PlaySound(SoundType.Rotate);
            MarkerRotation = ObjectRotationUtil.RotateCounterClockwise(MarkerRotation);
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Period)) {
            SoundController.Instance.PlaySound(SoundType.Rotate);
            MarkerRotation = ObjectRotationUtil.RotateClockwise(MarkerRotation);
        }
    }
}