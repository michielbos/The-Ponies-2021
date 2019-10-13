using System;
using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using UnityEngine;

namespace Model.Property {

/// <summary>
/// A tile that is part of the terrain.
/// </summary>
[Serializable]
public class TerrainTile : MonoBehaviour, IActionTarget {
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

    public ICollection<PonyAction> GetActions(Pony pony) {
        return ActionManager.GetActionsForTile(pony, this);
    }
}

}