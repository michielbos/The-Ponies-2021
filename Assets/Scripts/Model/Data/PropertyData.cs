﻿using JetBrains.Annotations;
using Model.Data;
using Model.Property;
using UnityEngine;

[System.Serializable]
public class PropertyData : ISerializationCallbackReceiver {
    public int id;
    public string name;
    public string description;
    public string streetName;
    public int propertyType;
    public long time;
    public TerrainTileData[] terrainTileDatas;
    public FloorTileData[] floorTileDatas;
    public WallData[] wallDatas;
    public RoofData[] roofDatas;
    public PropertyObjectData[] propertyObjectDatas;
    public GamePonyData[] ponies;
    [CanBeNull] public HouseholdData householdData;

    public PropertyData(int id, string name, string description, string streetName, int propertyType, long time,
        TerrainTileData[] terrainTileDatas, FloorTileData[] floorTileDatas, WallData[] wallDatas, RoofData[] roofDatas,
        PropertyObjectData[] propertyObjectDatas, GamePonyData[] ponyDatas, HouseholdData householdData) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.streetName = streetName;
        this.propertyType = propertyType;
        this.time = time;
        this.terrainTileDatas = terrainTileDatas;
        this.floorTileDatas = floorTileDatas;
        this.wallDatas = wallDatas;
        this.roofDatas = roofDatas;
        this.propertyObjectDatas = propertyObjectDatas;
        this.ponies = ponyDatas;
        this.householdData = householdData;
    }

    public PropertyType GetPropertyType() {
        return propertyType == 0 ? PropertyType.RESIDENTIAL : PropertyType.COMMUNITY;
    }

    public void OnBeforeSerialize() {
        
    }

    public void OnAfterDeserialize() {
        if (string.IsNullOrEmpty(householdData?.householdName)) {
            householdData = null;
        }
    }
}