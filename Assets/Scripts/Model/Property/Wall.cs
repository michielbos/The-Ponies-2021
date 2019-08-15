using System;
using System.Collections.Generic;
using Controllers.Playmode;
using UnityEngine;
using Util;

namespace Model.Property {

/// <summary>
/// A wall that can be placed on the border of a tile.
/// </summary>
[Serializable]
public class Wall : MonoBehaviour {
    public Mesh fullWallMesh;
    public Mesh shortWallMesh;
    public Material unpaintedMaterial;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    private WallDirection _direction;
    private WallCoverPreset _coverFront;
    private WallCoverPreset _coverBack;

    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }
    
    public TileBorder TileBorder {
        get {
            Vector2Int tilePosition = TilePosition;
            return new TileBorder(tilePosition.x, tilePosition.y, Direction);
        }
    }

    public WallDirection Direction {
        get { return _direction; }
        set {
            _direction = value;
            float rotation;
            if (value == WallDirection.NorthEast) {
                rotation = 0;
            } else { // NorthWest
                rotation = 270;
            }
            // TODO: Add horizontal and vertical rotations
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    public WallCoverPreset CoverFront {
        set {
            _coverFront = value;
            SetVisibleCoverMaterial(false, CoverFront);
        }
        get => _coverFront;
    }

    public WallCoverPreset CoverBack {
        set {
            _coverBack = value;
            SetVisibleCoverMaterial(false, CoverBack);
        }
        get => _coverBack;
    }

    private void Start() {
        UpdateVisibility();
    }

    public void Init(int x, int y, WallDirection wallDirection) {
        TilePosition = new Vector2Int(x, y);
        Direction = wallDirection;
    }

    public void UpdateVisibility() {
        UpdateVisibility(WallVisibilityController.Instance.wallVisibility);
    }
    
    public void UpdateVisibility(WallVisibility visibility) {
        if (visibility == WallVisibility.Low)
            SetLowered(true);
        else if (visibility == WallVisibility.Partially) {
            SetLowered(HasRoomBehindWall());
        } else 
            SetLowered(false);
        
    }

    public void SetLowered(bool lowered) {
        meshFilter.sharedMesh = lowered ? shortWallMesh : fullWallMesh;
    }

    private bool HasRoomBehindWall() {
        Property property = PropertyController.Instance.property;
        CameraRotation cameraRotation = CameraController.Instance.CameraRotation;
        switch (cameraRotation) {
            case CameraRotation.North:
                return property.IsInsideRoom(TileBorder.StartPosition);
            case CameraRotation.East:
                return Direction == WallDirection.NorthWest && property.IsInsideRoom(TileBorder.StartPosition) ||
                    Direction == WallDirection.NorthEast && property.IsInsideRoom(TileBorder.StartPosition + Vector2Int.down);
            case CameraRotation.South:
                return Direction == WallDirection.NorthWest &&
                       property.IsInsideRoom(TileBorder.StartPosition + Vector2Int.left) ||
                       Direction == WallDirection.NorthEast &&
                       property.IsInsideRoom(TileBorder.StartPosition + Vector2Int.down);
            case CameraRotation.West:
                return Direction == WallDirection.NorthWest &&
                       property.IsInsideRoom(TileBorder.StartPosition + Vector2Int.left) ||
                       Direction == WallDirection.NorthEast &&
                       property.IsInsideRoom(TileBorder.StartPosition);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public IEnumerable<Wall> GetConnectedWalls(bool includeSelf) {
        List<TileBorder> borders = TileBorder.GetConnectedBorders(includeSelf);
        return PropertyController.Instance.property.GetWalls(borders);
    }

    /// <summary>
    /// Returns true if the given point touches the front side of the wall.
    /// Returns false if it touches the back side of the wall.
    /// </summary>
    public bool IsFrontOfWall(Vector3 point) {
        if (Direction == WallDirection.NorthEast)
            return point.z < transform.position.z;
        if (Direction == WallDirection.NorthWest)
            return point.x > transform.position.x;
        
        // Horizontal/vertical walls not implemented yet.
        return false;
    }
    
    /// <summary>
    /// Reset the wall covers of this wall to the real ones.
    /// </summary>
    public void RemovePreviewCovers() {
        SetVisibleCoverMaterial(true, CoverFront);
        SetVisibleCoverMaterial(false, CoverBack);
    }

    /// <summary>
    /// Update the cover material on the specified side to match the given preset.
    /// This does only changes the displayed material. Not the real cover preset of this wall.
    /// </summary>
    public void SetVisibleCoverMaterial(bool front, WallCoverPreset preset) {
        Material material = preset?.GetMaterial() ?? unpaintedMaterial;
        // Unity returns a copied array, so we solve things the fancy way.
        meshRenderer.sharedMaterials = meshRenderer.sharedMaterials.Also(it => it[front ? 0 : 1] = material);
    }

    public WallData GetWallData() {
        Vector2Int tilePosition = TilePosition;
        return new WallData(tilePosition.x,
            tilePosition.y,
            (int) Direction,
            CoverFront?.guid.ToString(),
            CoverBack?.guid.ToString());
    }
}

}