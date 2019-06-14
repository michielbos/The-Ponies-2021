using UnityEngine;

namespace Model.Property {

/// <summary>
/// A wall that can be placed on the border of a tile.
/// The physical "dummy" of this wall is kept in the dummyWall attribute.
/// </summary>
[System.Serializable]
public class Wall : MonoBehaviour {
    public int x;
    public int y;
    public WallDirection wallDirection;

    public void Init (int x, int y, WallDirection wallDirection) {
        this.x = x;
        this.y = y;
        this.wallDirection = wallDirection;
    }

    public WallData GetWallData () {
        return new WallData(x,
            y,
            (int)wallDirection,
            -1,
            -1);
    }
}

}
