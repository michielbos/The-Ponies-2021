﻿using System.Collections.Generic;
using System.Linq;
using Model.Actions;
using Model.Data;
using Model.Ponies;
using MoonSharp.Interpreter;
using Scripts;
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
    public readonly IDictionary<DynValue, DynValue> data = new Dictionary<DynValue, DynValue>();
    
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
    
    public void Init(int id, int x, int y, ObjectRotation rotation, FurniturePreset preset, int skin, int value) {
        this.id = id;
        TilePosition = new Vector2Int(x, y);
        this.preset = preset;
        this.skin = skin;
        this.value = value;
        preset.ApplyToModel(GetComponent<ModelContainer>(), skin);
        Rotation = rotation;
    }

    public void InitScriptData(DataPair[] data, Property property) {
        foreach (DataPair pair in data) {
            this.data[pair.GetDynKey(property)] = pair.GetDynValue(property);
        }
    }

    public PropertyObjectData GetPropertyObjectData() {
        Vector2Int tilePosition = TilePosition;
        return new PropertyObjectData(id, tilePosition.x, tilePosition.y, (int) Rotation, preset.guid.ToString(), skin,
            value, data.Select(pair => DataPair.FromDynValues(pair.Key, pair.Value)).ToArray());
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
        return ScriptManager.Instance.hooks.RequestObjectActions(pony, this);
    }
}

}