﻿using System.IO;
using UnityEngine;

namespace Model.Property {

public class PropertyLoader {
    const string propertyPrefix = "Property";
    const string dataPath = "Assets/Resources/SaveData/";
    const string propertyPath = dataPath + "Property/";

    public PropertyLoader() {
        TryCreateDirectory(propertyPath);
    }

    public bool PropertyExists(int id) {
        return File.Exists(propertyPath + propertyPrefix + id);
    }

    public void SaveProperty(PropertyData propertyData) {
        SavePropertyData(propertyData);
    }

    public PropertyData LoadOrCreateProperty(int id) {
        if (PropertyExists(id))
            return LoadProperty(id);
        return CreateEmptyProperty(id, 40, 30);
    }

    private PropertyData CreateEmptyProperty(int id, int width, int height) {
        TerrainTileData[] terrainTileDatas = new TerrainTileData[width * height];
        FloorTileData[] floorTileDatas = new FloorTileData[0];
        WallData[] wallDatas = new WallData[0];
        RoofData[] roofDatas = new RoofData[0];
        PropertyObjectData[] propertyObjectDatas = new PropertyObjectData[0];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                terrainTileDatas[y * width + x] = new TerrainTileData(x, y, 0, 0);
            }
        }

        return new PropertyData(id, "untitled", "", "untitled street " + id, 0, terrainTileDatas, floorTileDatas,
            wallDatas, roofDatas, propertyObjectDatas);
    }

    public PropertyData LoadProperty(int id) {
        return LoadPropertyData(id);
    }

    void SavePropertyData(PropertyData propertyData) {
        WriteFile(propertyPath + propertyPrefix + propertyData.id, JsonUtility.ToJson(propertyData));
    }

    PropertyData LoadPropertyData(int id) {
        return JsonUtility.FromJson<PropertyData>(ReadFile(propertyPath + propertyPrefix + id));
    }

    void WriteFile(string path, string data) {
        if (File.Exists(path))
            File.Delete(path);
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(data);
        writer.Close();
    }

    string ReadFile(string path) {
        StreamReader reader = new StreamReader(path);
        string data = reader.ReadToEnd();
        reader.Close();
        return data;
    }

    void TryCreateDirectory(string path) {
        if (!Directory.Exists(path)) {
            Debug.Log("Created directory: " + path);
            Directory.CreateDirectory(path);
        }
    }
}

}