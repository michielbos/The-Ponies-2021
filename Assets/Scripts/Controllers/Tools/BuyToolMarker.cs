using System;
using System.Collections.Generic;
using Controllers.Singletons;
using JetBrains.Annotations;
using Model.Property;
using UnityEngine;
using Util;

namespace Controllers.Tools {

public class BuyToolMarker : MonoBehaviour {
    public GameObject buyMarkingPrefab;
    public Material buyMarkingNormalMaterial;
    public Material buyMarkingDisallowedMaterial;

    /// <summary>
    /// Specifies whether the current target slot of this marker is a surface slot.
    /// </summary>
    private bool placedAsChild;

    /// <summary>
    /// The visual model that is used for the marker.
    /// The container can contain either a new model (when buying) or a borrowed existing model (when moving)
    /// </summary>
    private ModelContainer buildMarkerModel;

    /// <summary>
    /// The owner of the buildMarkerModel.
    /// If this is null, the model is owned by this BuyToolMarker.
    /// If this is not null, the model is owned by an existing furniture object and must be returned when destroying
    /// this marker.
    /// </summary>
    [CanBeNull]
    private PropertyObject modelOwner;
    
    private readonly List<GameObject> buyMarkings = new List<GameObject>();
    private FurniturePreset preset;

    private ObjectRotation _markerRotation = ObjectRotation.SouthEast;

    public Vector2Int TilePosition {
        get => transform.position.ToTilePosition();
        set => transform.position = value.ToWorldPosition();
    }

    public ObjectRotation MarkerRotation {
        get => _markerRotation;
        set {
            _markerRotation = value;
            Preset.FixModelTransform(buildMarkerModel.Model.transform, value, placedAsChild);
        }
    }

    public ICollection<Vector2Int> OccupiedTiles => Preset.GetOccupiedTiles(TilePosition, MarkerRotation);

    /// <summary>
    /// The preset that is used.
    /// If moving an existing model, the preset of the model owner is returned.
    /// </summary>
    private FurniturePreset Preset => modelOwner == null ? preset : modelOwner.preset;

    /// <summary>
    /// Initialize this marker for buying new furniture.
    /// This creates a new model with the given parameters.
    /// </summary>
    public void InitBuy(FurniturePreset preset, int skin, ObjectRotation rotation) {
        this.preset = preset;
        buildMarkerModel = GetComponent<ModelContainer>();
        preset.ApplyToModel(buildMarkerModel, skin);
        MarkerRotation = ObjectRotation.SouthEast;
        PlaceBuyMarkings();
        MarkerRotation = rotation;
    }
    
    /// <summary>
    /// Initialize this marker for moving existing furniture.
    /// This temporarily parents the existing model to this marker.
    /// </summary>
    public void InitMove(PropertyObject propertyObject) {
        modelOwner = propertyObject;
        buildMarkerModel = GetComponent<ModelContainer>();
        buildMarkerModel.Model = propertyObject.Model.gameObject;
        propertyObject.Model.parent = transform;
        ObjectRotation currentRotation = propertyObject.Rotation;
        MarkerRotation = ObjectRotation.SouthEast;
        PlaceBuyMarkings();
        MarkerRotation = currentRotation;
    }
    
    private void PlaceBuyMarkings() {
        foreach (Vector2Int tile in Preset.occupiedTiles) {
            buyMarkings.Add(Instantiate(buyMarkingPrefab, new Vector3(tile.x + 0.5f, 0.01f, tile.y + 0.5f),
                buyMarkingPrefab.transform.rotation, buildMarkerModel.Model.transform));
        }
    }

    public void UpdateMarker() {
        HandleRotationButtons();
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

    /// <summary>
    /// Called by the buy tool when the marker is moved to a new object slot.
    /// </summary>
    public void OnNewSlot(IObjectSlot newSlot) {
        placedAsChild = newSlot is SurfaceSlot;
        Preset.FixModelTransform(buildMarkerModel.Model.transform, MarkerRotation, placedAsChild);
    }
    
    public void SetCanPlace(bool canPlace) {
        foreach (GameObject buyMarking in buyMarkings) {
            buyMarking.GetComponent<Renderer>().material =
                canPlace ? buyMarkingNormalMaterial : buyMarkingDisallowedMaterial;
        }
    }

    public bool HandleDragRotation(Vector2Int tileUnderCursor) {
        Vector2Int diff = TilePosition - tileUnderCursor;
        ObjectRotation newRotation = MarkerRotation;
        if (diff.x != 0 && Math.Abs(diff.x) > Math.Abs(diff.y)) {
            newRotation = diff.x > 0 ? ObjectRotation.SouthWest : ObjectRotation.NorthEast;
        } else if (diff.y != 0 && Math.Abs(diff.y) > Math.Abs(diff.x)) {
            newRotation = diff.y > 0 ? ObjectRotation.SouthEast : ObjectRotation.NorthWest;
        }

        if (newRotation != MarkerRotation) {
            SoundController.Instance.PlaySound(SoundType.Rotate);
            MarkerRotation = newRotation;
            return true;
        }
        return false;
    }

    public void OnDestroy() {
        // Return the model to its owner, if we were moving an existing object.
        if (modelOwner != null) {
            foreach (GameObject marking in buyMarkings) {
                Destroy(marking);
            }
            buildMarkerModel.Model.transform.parent = modelOwner.transform;
        }
    }
}

}