using UnityEngine;

namespace Model.Property {

/// <summary>
/// A wall that can be placed on the border of a tile.
/// </summary>
[System.Serializable]
public class Wall : MonoBehaviour {
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

    public void Init(int x, int y, WallDirection wallDirection) {
        TilePosition = new Vector2Int(x, y);
        Direction = wallDirection;
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