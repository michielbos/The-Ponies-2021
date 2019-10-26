using System;
using System.Collections.Generic;
using Controllers.Singletons;
using Model.Property;
using UnityEngine;
using Util;

namespace Controllers.Tools {

public class BuyToolMarker : MonoBehaviour {
    public GameObject buyMarkingPrefab;
    public Material buyMarkingNormalMaterial;
    public Material buyMarkingDisallowedMaterial;

    private ModelContainer buildMarkerModel;
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
            preset.FixModelTransform(buildMarkerModel.Model.transform, value);
        }
    }

    public ICollection<Vector2Int> OccupiedTiles => preset.GetOccupiedTiles(TilePosition, MarkerRotation);
    
    public void Init(FurniturePreset preset, int skin, ObjectRotation rotation) {
        this.preset = preset;
        buildMarkerModel = GetComponent<ModelContainer>();
        preset.ApplyToModel(buildMarkerModel, skin);
        MarkerRotation = ObjectRotation.SouthEast;
        PlaceBuyMarkings();
        MarkerRotation = rotation;
    }
    
    private void PlaceBuyMarkings() {
        foreach (Vector2Int tile in preset.occupiedTiles) {
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
}

}