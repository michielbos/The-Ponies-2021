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
        UpdateVisibility(HUDController.Instance.wallVisibility);
    }

    public void Init(int x, int y, WallDirection wallDirection) {
        TilePosition = new Vector2Int(x, y);
        Direction = wallDirection;
    }

    public void UpdateVisibility(WallVisibility visibility) {
        // TODO: Implement partial walls.
        if (visibility == WallVisibility.Low) {
            meshFilter.sharedMesh = shortWallMesh;
        } else {
            meshFilter.sharedMesh = fullWallMesh;
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