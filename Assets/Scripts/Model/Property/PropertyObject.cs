using UnityEngine;

namespace Model.Property {

/// <summary>
/// An object that can be placed on a lot, usually a piece of furniture.
/// </summary>
[System.Serializable]
public class PropertyObject : MonoBehaviour {
    public int id;
    public ObjectRotation rotation;
    public FurniturePreset preset;
    public int skin;
    public int value;
    public Transform model;

    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x - 0.5f), Mathf.RoundToInt(position.z - 0.5f));
        }
        set { transform.position = new Vector3(value.x + 0.5f, 0, value.y + 0.5f); }
    }

    public ObjectRotation Rotation {
        get { return rotation; }
        set {
            rotation = value;
            transform.eulerAngles = ObjectRotationUtil.GetRotationVector(rotation);
        }
    }

    public void Init(int id, int x, int y, ObjectRotation rotation, FurniturePreset preset, int skin) {
        this.id = id;
        TilePosition = new Vector2Int(x, y);
        Rotation = rotation;
        this.preset = preset;
        this.skin = skin;
        value = preset.price;
        preset.ApplyToPropertyObject(this, true);
    }

    public PropertyObjectData GetPropertyObjectData() {
        Vector2Int tilePosition = TilePosition;
        return new PropertyObjectData(id, tilePosition.x, tilePosition.y, (int) Rotation, preset.guid.ToString(), skin,
            value);
    }

    /// <summary>
    /// Get the coordinates of the tiles occupied by this PropertyObject.
    /// </summary>
    /// <returns>A Vector2Int array of all occupied coordinates.</returns>
    public Vector2Int[] GetOccupiedTiles() {
        return preset.GetOccupiedTiles(TilePosition);
    }

    public void SetVisibility(bool visible) {
        model.gameObject.SetActive(visible);
    }
}

}