using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using PoneCrafter.Json;
using PoneCrafter.Model;
using UnityEngine;
using Util;
using Terrain = PoneCrafter.Model.Terrain;

namespace PoneCrafter {

public class PoneCrafterImporter {
    private const string PROPERTIES_FILE = "properties.json";
    private const string PCC_EXTENSION = ".pcc";
    public List<Floor> loadedFloors;
    public List<Roof> loadedRoofs;
    public List<Terrain> loadedTerrains;
    public List<Furniture> loadedFurniture;
    private static PoneCrafterImporter instance;

    private PoneCrafterImporter() {
        loadedFloors = new List<Floor>();
        loadedRoofs = new List<Roof>();
        loadedTerrains = new List<Terrain>();
        loadedFurniture = new List<Furniture>();
    }

    public static PoneCrafterImporter Instance => instance ?? (instance = new PoneCrafterImporter());

    /// <summary>
    /// Import all PoneCrafter files.
    /// This should be done only once, at the start of the game.
    /// </summary>
    public void Import() {
        // TODO: Apply content UUID checks on content folder.
        ImportFolder(Application.dataPath + "/Content/");
        ImportFolder(Application.dataPath + "/../Mods/");
    }

    private void ImportFolder(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        foreach (string file in Directory.GetFiles(path)) {
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

    private bool DoesUuidExist(Guid uuid) {
        return loadedFurniture.Any(it => it.uuid == uuid) ||
               loadedFloors.Any(it => it.uuid == uuid) ||
               loadedRoofs.Any(it => it.uuid == uuid) ||
               loadedTerrains.Any(it => it.uuid == uuid);
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
        Guid uuid = baseModel.GetUuid();
        if (!GuidUtil.IsPackagelessContent(uuid)) {
            throw new ImportException("UUID " + baseModel.GetUuid() +
                                      " does not have a valid pack id for packageless content!");
        }
        if (DoesUuidExist(baseModel.GetUuid())) {
            throw new ImportException("UUID " + baseModel.GetUuid() + " is already used by another item!");
        }
        switch (baseModel.type) {
            case "floor":
                loadedFloors.Add(LoadFloor(zipArchive, properties));
                break;
            case "roof":
                loadedRoofs.Add(LoadRoof(zipArchive, properties));
                break;
            case "terrain":
                loadedTerrains.Add(LoadTerrain(zipArchive, properties));
                break;
            case "furniture":
                loadedFurniture.Add(LoadFurniture(zipArchive, properties));
                break;
            default:
                throw new ImportException("Invalid content type: " + baseModel.type);
        }
    }

    private Floor LoadFloor(ZipArchive zipArchive, string properties) {
        JsonFloor jsonFloor = JsonUtility.FromJson<JsonFloor>(properties);
        Texture2D texture = LoadTexture(zipArchive);
        return new Floor(jsonFloor, texture);
    }

    private Roof LoadRoof(ZipArchive zipArchive, string properties) {
        JsonRoof jsonRoof = JsonUtility.FromJson<JsonRoof>(properties);
        Texture2D texture = LoadTexture(zipArchive);
        return new Roof(jsonRoof, texture);
    }

    private Terrain LoadTerrain(ZipArchive zipArchive, string properties) {
        JsonTerrain jsonTerrain = JsonUtility.FromJson<JsonTerrain>(properties);
        Texture2D texture = LoadTexture(zipArchive);
        return new Terrain(jsonTerrain, texture);
    }
    
    private Furniture LoadFurniture(ZipArchive zipArchive, string properties) {
        JsonFurniture jsonFurniture = JsonUtility.FromJson<JsonFurniture>(properties);
        Texture2D texture = LoadTexture(zipArchive);
        Mesh mesh = LoadMesh(zipArchive);
        return new Furniture(jsonFurniture, mesh, texture);
    }

    private Texture2D LoadTexture(ZipArchive zipArchive, string filename = "texture.png") {
        ZipArchiveEntry textureEntry = zipArchive.GetEntry(filename);
        if (textureEntry == null) {
            throw new ImportException("File did not contain a " + filename + " entry.");
        }
        using (Stream stream = textureEntry.Open()) {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            Texture2D texture = new Texture2D(0, 0);
            if (!texture.LoadImage(bytes)) {
                throw new ImportException("Failed to import texture.");
            }
            return texture;
        }
    }
    
    private Mesh LoadMesh(ZipArchive zipArchive, string filename = "model.obj") {
        ZipArchiveEntry meshEntry = zipArchive.GetEntry(filename);
        if (meshEntry == null) {
            throw new ImportException("File did not contain a " + filename + " entry.");
        }
        using (Stream stream = meshEntry.Open()) {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            Mesh mesh = FastObjImporter.Instance.ImportMesh(bytes);
            return mesh;
        }
    }
}

}