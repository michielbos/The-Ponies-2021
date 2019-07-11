using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Playmode;
using Controllers.Singletons;
using JetBrains.Annotations;
using Model.Property;
using UnityEngine;

namespace Controllers.Tools {

/// <summary>
/// Tool for buy/build mode that deals with buying, moving and selling furniture.
/// </summary>
/// 
public class BuyTool : MonoBehaviour, ITool {
    private const int LAYER_TERRAIN = 8;

    public GameObject buildMarkerPrefab;
    public GameObject buyMarkingPrefab;
    public Material buyMarkingNormalMaterial;
    public Material buyMarkingDisallowedMaterial;

    private FurniturePreset placingPreset;
    private int placingSkin;

    private PropertyObject movingObject;

    private GameObject buildMarker;
    private Transform buildMarkerModel;
    private ObjectRotation markerRotation = ObjectRotation.SouthEast;
    private readonly List<GameObject> buyMarkings;
    private TerrainTile targetTile;
    private bool pressingTile;
    private bool canPlace;

    public ObjectRotation MarkerRotation {
        get { return markerRotation; }
        set {
            markerRotation = value;
            if (buildMarkerModel != null) {
                GetMovingPreset().FixModelTransform(buildMarkerModel, markerRotation);
                UpdateCanPlace();
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
        buildMarkerModel = null;
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
        buildMarkerModel = buildMarker.transform.GetChild(0);
        preset.ApplyToModel(buildMarkerModel.gameObject, placingSkin);
        SetBuildMarkerPosition(0, 0);
        ObjectRotation rotation = MarkerRotation;
        MarkerRotation = ObjectRotation.SouthEast;
        PlaceBuyMarkings();
        MarkerRotation = rotation;
        buildMarker.transform.position = new Vector3(0, -100, 0);
    }

    private void PlaceBuyMarkings() {
        foreach (Vector2Int tile in GetMovingPreset().occupiedTiles) {
            buyMarkings.Add(Instantiate(buyMarkingPrefab, new Vector3(tile.x + 0.5f, 0.01f, tile.y + 0.5f),
                buyMarkingPrefab.transform.rotation, buildMarkerModel.transform));
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
        UpdateCanPlace();
        Vector2Int tilePosition = targetTile.TilePosition;
        SetBuildMarkerPosition(tilePosition.x, tilePosition.y);
    }

    private void UpdateCanPlace() {
        if (targetTile == null) {
            canPlace = false;
            return;
        }

        canPlace = CanPlaceObject();
        foreach (GameObject buyMarking in buyMarkings) {
            buyMarking.GetComponent<Renderer>().material =
                canPlace ? buyMarkingNormalMaterial : buyMarkingDisallowedMaterial;
        }
    }

    private bool CanPlaceObject() {
        FurniturePreset movingPreset = GetMovingPreset();
        Vector2Int[] requiredTiles = movingPreset.GetOccupiedTiles(targetTile.TilePosition, MarkerRotation);

        bool canPlace = placingPreset == null || MoneyController.Instance.CanAfford(placingPreset.price);

        if (!canPlace)
            return false;

        if (!CheatsController.Instance.moveObjectsMode) {
            List<PropertyObject> tileObjects = PropertyController.Instance.property.GetObjectsOnTiles(requiredTiles);
            foreach (PropertyObject tileObject in tileObjects) {
                if (tileObject != movingObject)
                    return false;
            }

            List<Wall> walls = PropertyController.Instance.property.GetOccupiedWalls(requiredTiles);
            if (walls.Count > 0) {
                return false;
            }
        }

        Property property = PropertyController.Instance.property;
        foreach (Vector2Int tile in requiredTiles) {
            if (tile.x < 0 || tile.y < 0 || tile.x >= property.TerrainWidth || tile.y >= property.TerrainHeight) {
                return false;
            }
        }

        if (movingPreset.placementType == PlacementType.Ground ||
            movingPreset.placementType == PlacementType.GroundOrSurface)
            return true;

        if (movingPreset.placementType == PlacementType.Terrain)
            return requiredTiles.Count(tile => property.GetFloorTile(tile.x, tile.y) != null) == 0;

        if (movingPreset.placementType == PlacementType.Floor)
            return requiredTiles.Count(tile => property.GetFloorTile(tile.x, tile.y) == null) == 0;

        //TODO: Check for walls, surfaces, ceilings.
        return false;
    }

    private void SetBuildMarkerPosition(int x, int y) {
        buildMarker.transform.position = new Vector3(x, 0, y);
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

}