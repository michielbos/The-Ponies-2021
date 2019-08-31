using System;
using System.IO;
using Assets.Scripts.Controllers;
using Model.Data;
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
        return CreateEmptyProperty(id, 64, 64);
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

        HouseholdData householdData = null;
        string[] ponyUuids = {Guid.NewGuid().ToString(), Guid.NewGuid().ToString()};
        long time = 0;
        if (id == 0) {
            PonyInfoData[] ponyInfos = {
                new PonyInfoData(ponyUuids[0], "Orange Butt", 2, 2, 1),
                new PonyInfoData(ponyUuids[1], "Gilheart", 3, 1, 1),
            };
            householdData = new HouseholdData("The Placeholders", 20000, ponyInfos);
            time = TimeController.StartingTime;
        }

        GamePonyData[] ponyDatas = {
            new GamePonyData(ponyUuids[0], 4, 6),
            new GamePonyData(ponyUuids[1], 4, 3)
        };
        
        return new PropertyData(id, "untitled", "", "untitled street " + id, 0, time, terrainTileDatas, floorTileDatas,
            wallDatas, roofDatas, propertyObjectDatas, ponyDatas, householdData);
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