using System;
using System.Collections.Generic;
using System.Linq;
using Model.Actions;
using Model.Ponies;
using UnityEngine;

namespace Model.Property {

/// <summary>
/// A tile that is part of the terrain.
/// </summary>
[Serializable]
public class TerrainTile : MonoBehaviour, IActionTarget, IObjectSlot {
    public int height;
    public int type;
    public Transform model;

    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

    public Vector3 SlotPosition => transform.position;

    /// <summary>
    /// Implemented for compatibility. Don't use them.
    /// </summary>
    public PropertyObject SlotObject {
        get => null;
        set { }
    }

    public void PlaceObject(PropertyObject propertyObject) {
        Transform objectTransform = propertyObject.transform;
        propertyObject.ClearParent();
        objectTransform.position = transform.position;
        propertyObject.OnPlaced();
    }

    public void Init(int x, int y, int height, int type) {
        TilePosition = new Vector2Int(x, y);
        this.height = height;
        this.type = type;
    }

    public TerrainTileData GetTerrainTileData() {
        Vector2Int tilePosition = TilePosition;
        return new TerrainTileData(tilePosition.x, tilePosition.y, height, type);
    }

    public void SetVisible(bool visible) {
        model.GetComponent<MeshRenderer>().enabled = visible;
    }

    /// <summary>
    /// Get all actions that the given pony can do on this terrain tile.
    /// If showInvisible is true, invisible actions that are normally not invokable by the player are also included.
    /// </summary>
    public ICollection<PonyAction> GetActions(Pony pony, bool showInvisible) {
        ICollection<PonyAction> actions = ActionManager.GetActionsForTile(pony, this);
        return showInvisible ? actions : actions.Where(action => action.Visible).ToList();
    }
}

}