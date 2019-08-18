using System.Collections.Generic;
using Model.Actions;
using Model.Actions.Actions;
using Model.Ponies;
using UnityEngine;
using Util;

namespace Model.Property {

/// <summary>
/// An object that can be placed on a lot, usually a piece of furniture.
/// </summary>
[System.Serializable]
[RequireComponent(typeof(ModelContainer))]
public class PropertyObject : MonoBehaviour, IActionProvider {
    public int id;
    private ObjectRotation rotation;
    public FurniturePreset preset;
    public int skin;
    public int value;
    public Transform Model => GetComponent<ModelContainer>().Model.transform;

    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

    public ObjectRotation Rotation {
        get { return rotation; }
        set {
            rotation = value;
            Model.rotation = Quaternion.identity;
            preset.FixModelTransform(Model, Rotation);
        }
    }

    public void Init(int id, int x, int y, ObjectRotation rotation, FurniturePreset preset, int skin) {
        this.id = id;
        TilePosition = new Vector2Int(x, y);
        this.preset = preset;
        this.skin = skin;
        value = preset.price;
        preset.ApplyToModel(GetComponent<ModelContainer>(), skin);
        Rotation = rotation;
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
        return preset.GetOccupiedTiles(TilePosition, Rotation);
    }
    
    /// <summary>
    /// Get the coordinates of the tiles occupied by this PropertyObject.
    /// </summary>
    /// <returns>A Vector2Int array of all occupied coordinates.</returns>
    public IEnumerable<TileBorder> GetRequiredWallBorders() {
        return preset.GetRequiredWallBorders(GetOccupiedTiles(), Rotation);
    }

    public void SetVisibility(bool visible) {
        Model.gameObject.SetActive(visible);
    }

    public List<PonyAction> GetActions(Pony pony) {
        return new List<PonyAction>() {
            new FakeAction(pony, "View")
        };
    }
}

}