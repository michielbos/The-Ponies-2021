using System;
using Controllers.Playmode;
using UnityEngine;

namespace Model.Property {

/// <summary>
/// A wall that can be placed on the border of a tile.
/// </summary>
[Serializable]
public class Wall : MonoBehaviour {
    public Mesh fullWallMesh;
    public Mesh shortWallMesh;
    public MeshFilter meshFilter;
    private WallDirection _direction;

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
            meshFilter.sharedMesh = shortWallMesh;
        else if (visibility == WallVisibility.Partially)
            meshFilter.sharedMesh = HasRoomBehindWall() ? shortWallMesh : fullWallMesh;
        else 
            meshFilter.sharedMesh = fullWallMesh;
        
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

    public WallData GetWallData() {
        Vector2Int tilePosition = TilePosition;
        return new WallData(tilePosition.x,
            tilePosition.y,
            (int) Direction,
            "",
            "");
    }
}

}