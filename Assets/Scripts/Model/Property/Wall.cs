using UnityEngine;

namespace Model.Property {

/// <summary>
/// A wall that can be placed on the border of a tile.
/// </summary>
[System.Serializable]
public class Wall : MonoBehaviour {
    public WallDirection wallDirection;

    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

    public void Init(int x, int y, WallDirection wallDirection) {
        TilePosition = new Vector2Int(x, y);
        this.wallDirection = wallDirection;
    }

    public WallData GetWallData() {
        Vector2Int tilePosition = TilePosition;
        return new WallData(tilePosition.x,
            tilePosition.y,
            (int) wallDirection,
            "",
            "");
    }
}

}