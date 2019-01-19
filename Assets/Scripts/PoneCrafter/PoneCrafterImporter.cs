using System.Collections.Generic;
using System.IO;
using PoneCrafter.Json;
using PoneCrafter.Model;
using UnityEngine;

namespace PoneCrafter {

public class PoneCrafterImporter {
    public List<Floor> loadedFloors;
    private const string PROPERTIES_FILE = "properties.json";
    private const string PCC_EXTENSION = ".json";

    public PoneCrafterImporter() {
        loadedFloors = new List<Floor>();
    }

    public void Import() {
        string directory = Application.dataPath + "/Mods/";
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        foreach (string file in Directory.GetFiles(directory)) {
            if (!file.EndsWith("/" + PROPERTIES_FILE)) {
                continue;
            }
            string content = File.ReadAllText(file);
            Debug.Log("Content: " + content);
            LoadContent(content);
        }
    }

    private void LoadContent(string properties) {
        BaseJsonModel baseModel = JsonUtility.FromJson<BaseJsonModel>(properties);
        switch (baseModel.type) {
            case "floor":
                loadedFloors.Add(LoadFloor(properties));
                break;
            default:
                throw new ImportException("Invalid content type: " + baseModel.type);
        }
    }

    private Floor LoadFloor(string properties) {
        JsonFloor jsonFloor = JsonUtility.FromJson<JsonFloor>(properties);
        // TODO: Import texture
        return new Floor(jsonFloor);
    }
}

}