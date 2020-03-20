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
    private static readonly int WallMask = Shader.PropertyToID("_WallMask");
    
    public Mesh fullWallMesh;
    public Mesh shortWallMesh;
    public Material unpaintedMaterial;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Texture cutoutTexture;

    private WallDirection _direction;
    private WallCoverPreset _coverFront;
    private WallCoverPreset _coverBack;
    /// <summary>
    /// This is set to true, one or both materials have been instanced.
    /// </summary>
    private bool instancedMaterials;

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
            SetVisibleCoverMaterial(true, CoverFront);
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
            SetLowered(HasRoomBehindWall() || WallVisibilityController.Instance.IsWallLoweredByMouse(this));
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
        UpdateMaterial(front ? 0 : 1, preset);
    }

    public void RefreshMaterials() {
        UpdateMaterial(0, CoverFront);
        UpdateMaterial(1, CoverBack);
    }

    private void UpdateMaterial(int sideIndex, WallCoverPreset preset) {
        //Debug.Log(sideIndex);
        Material material = preset?.GetMaterial() ?? unpaintedMaterial;
        if (instancedMaterials) {
            Destroy(meshRenderer.materials[sideIndex]);
        }
        meshRenderer.sharedMaterials = meshRenderer.sharedMaterials.Also(it => it[sideIndex] = material);
        UpdateCutout(sideIndex);
    }

    public void UpdateCutout(int sideIndex) {
        if (cutoutTexture == null)
            return;
        // Accessing the materials property automatically converts all materials to instances.
        // Thanks to Unity's clear API.
        instancedMaterials = true;
        Material[] materials = meshRenderer.materials;
        materials[sideIndex].SetTexture(WallMask, cutoutTexture);
        meshRenderer.materials = materials;
    }

    private void OnDestroy() {
        // Clean up instanced materials, if we have them.
        if (instancedMaterials) {
            foreach (Material material in meshRenderer.materials) {
                Destroy(material);
            }
        }
    }

    public WallCoverPreset GetCoverPreset(bool front) {
        return front ? CoverFront : CoverBack;
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

/// <summary>
/// Simple class to define the front or back side of a wall.
/// </summary>
public class WallSide : IEquatable<WallSide> {
    public readonly Wall wall;
    public readonly bool front;

    public WallSide(Wall wall, bool front) {
        this.wall = wall;
        this.front = front;
    }

    public void ApplyCoverPreset(WallCoverPreset preset) {
        if (front)
            wall.CoverFront = preset;
        else
            wall.CoverBack = preset;
    }
    
    public bool HasThisPreset(WallCoverPreset preset) {
        return front ? wall.CoverFront == preset : wall.CoverBack == preset;
    }

    public bool Equals(WallSide other) {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Equals(wall, other.wall) && front == other.front;
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((WallSide) obj);
    }

    public override int GetHashCode() {
        unchecked {
            return ((wall != null ? wall.GetHashCode() : 0) * 397) ^ front.GetHashCode();
        }
    }

    public static bool operator ==(WallSide left, WallSide right) {
        return Equals(left, right);
    }

    public static bool operator !=(WallSide left, WallSide right) {
        return !Equals(left, right);
    }
}

}