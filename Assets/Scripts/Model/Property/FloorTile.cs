using UnityEngine;

namespace Model.Property {

[System.Serializable]
public class FloorTile : MonoBehaviour {
    public FloorPreset preset;
    public Transform model;

    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

    public void Init(int x, int y, FloorPreset preset) {
        this.preset = preset;
        TilePosition = new Vector2Int(x, y);
        preset.ApplyToGameObject(model.gameObject);
    }

    public FloorTileData GetFloorTileData() {
        Vector2Int tilePosition = TilePosition;
        return new FloorTileData(tilePosition.x, tilePosition.y, preset.guid.ToString());
    }
}

}