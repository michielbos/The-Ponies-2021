using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using PoneCrafter.Json;
using PoneCrafter.Model;
using UnityEngine;

namespace PoneCrafter {


public class PoneCrafterImporter {
    private const string PROPERTIES_FILE = "properties.json";
    private const string PCC_EXTENSION = ".pcc";
    public List<Floor> loadedFloors;
    private static PoneCrafterImporter instance;

    private PoneCrafterImporter() {
        loadedFloors = new List<Floor>();
    }

    public static PoneCrafterImporter Instance => instance ?? (instance = new PoneCrafterImporter());

    /// <summary>
    /// Import all PoneCrafter files.
    /// This should be done only once, at the start of the game.
    /// </summary>
    public void Import() {
        string directory = Application.dataPath + "/Mods/";
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
        foreach (string file in Directory.GetFiles(directory)) {
            if (!file.ToLower().EndsWith(PCC_EXTENSION)) {
                continue;
            }
            try {
                ReadZip(file);
            } catch (ImportException e) {
                // TODO: Collect and forward errors to user.
                Debug.LogWarning("Failed to import " + file + ": " + e.Message);
            }
        }
    }

    private void ReadZip(string file) {
        using (ZipArchive zipArchive = ZipFile.Open(file, ZipArchiveMode.Read)) {
            ZipArchiveEntry propertiesEntry = zipArchive.GetEntry("properties.json");
            if (propertiesEntry == null) {
                throw new ImportException("Content file did not contain a properties.json entry!");
            }
            using (StreamReader reader = new StreamReader(propertiesEntry.Open())) {
                string properties = reader.ReadToEnd();
                LoadContent(zipArchive, properties);
            }
        }
    }

    private void LoadContent(ZipArchive zipArchive, string properties) {
        BaseJsonModel baseModel = JsonUtility.FromJson<BaseJsonModel>(properties);
        switch (baseModel.type) {
            case "floor":
                loadedFloors.Add(LoadFloor(zipArchive, properties));
                break;
            default:
                throw new ImportException("Invalid content type: " + baseModel.type);
        }
    }

    private Floor LoadFloor(ZipArchive zipArchive, string properties) {
        JsonFloor jsonFloor = JsonUtility.FromJson<JsonFloor>(properties);

        ZipArchiveEntry textureEntry = zipArchive.GetEntry("texture.png");
        if (textureEntry == null) {
            throw new ImportException("Floor file did not contain a texture.png entry.");
        }
        using (Stream stream = textureEntry.Open()) {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            Texture2D texture = new Texture2D(0, 0);
            if (!texture.LoadImage(bytes)) {
                throw new ImportException("Failed to import texture.");
            }
            return new Floor(jsonFloor, texture);
        }
    }
}

}