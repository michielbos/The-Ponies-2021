using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using PoneCrafter.Json;
using PoneCrafter.Model;
using UnityEngine;
using Util.Importer;
using Terrain = PoneCrafter.Model.Terrain;

namespace PoneCrafter {

public class PoneCrafterImporter {
    private const string PROPERTIES_FILE = "properties.json";
    private const string PCC_EXTENSION = ".pcc";
    // The number of tasks to run asynchronously.
    private const int TaskPoolSize = 16;
    
    public List<Floor> loadedFloors;
    public List<WallCover> loadedWallCovers;
    public List<Roof> loadedRoofs;
    public List<Terrain> loadedTerrains;
    public List<Furniture> loadedFurniture;
    private static PoneCrafterImporter instance;

    private GltfLoader gltfLoader;

    private PoneCrafterImporter() {
        loadedFloors = new List<Floor>();
        loadedWallCovers = new List<WallCover>();
        loadedRoofs = new List<Roof>();
        loadedTerrains = new List<Terrain>();
        loadedFurniture = new List<Furniture>();
    }

    public static PoneCrafterImporter Instance => instance ?? (instance = new PoneCrafterImporter());

    /// <summary>
    /// Import all PoneCrafter files.
    /// This should be done only once, at the start of the game.
    /// </summary>
    public async void Import() {
        // TODO: Apply content UUID checks on content folder.
        gltfLoader = new GameObject("GltfLoader").AddComponent<GltfLoader>();
        gltfLoader.Prepare();
        await ImportFolder(Application.dataPath + "/Content/");
        await ImportFolder(Application.dataPath + "/../Mods/");
    }

    private async Task ImportFolder(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        List<Task> tasks = new List<Task>(TaskPoolSize);
        foreach (string file in Directory.GetFiles(path)) {
            if (!file.ToLower().EndsWith(PCC_EXTENSION)) {
                continue;
            }
            tasks.Add(StartImportZipTask(file));
            if (tasks.Count >= TaskPoolSize) {
                Task task = tasks.First();
                await task;
                tasks.Remove(task);
            }
        }
    }
    
    private bool DoesUuidExist(Guid uuid) {
        return loadedFurniture.Any(it => it.uuid == uuid) ||
               loadedFloors.Any(it => it.uuid == uuid) ||
               loadedWallCovers.Any(it => it.uuid == uuid) ||
               loadedRoofs.Any(it => it.uuid == uuid) ||
               loadedTerrains.Any(it => it.uuid == uuid);
    }

    private async Task StartImportZipTask(string zipFilePath) {
        try {
            await ImportZip(zipFilePath);
        } catch (ImportException e) {
            // TODO: Collect and forward errors to user.
            Debug.LogWarning("Failed to import " + zipFilePath + ": " + e.Message);
        }
    }

    private async Task ImportZip(string file) {
        using (ZipArchive zipArchive = ZipFile.Open(file, ZipArchiveMode.Read)) {
            ZipArchiveEntry propertiesEntry = zipArchive.GetEntry("properties.json");
            if (propertiesEntry == null) {
                throw new ImportException("Content file did not contain a properties.json entry!");
            }
            using (StreamReader reader = new StreamReader(propertiesEntry.Open())) {
                string properties = reader.ReadToEnd();
                await LoadContent(zipArchive, properties);
            }
        }
    }

    private async Task LoadContent(ZipArchive zipArchive, string properties) {
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
            case "wallcover":
                loadedWallCovers.Add(LoadWallCover(zipArchive, properties));
                break;
            case "roof":
                loadedRoofs.Add(LoadRoof(zipArchive, properties));
                break;
            case "terrain":
                loadedTerrains.Add(LoadTerrain(zipArchive, properties));
                break;
            case "furniture":
                Task<Furniture> task = LoadFurniture(zipArchive, properties);
                await task;
                loadedFurniture.Add(task.Result);
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
    
    private WallCover LoadWallCover(ZipArchive zipArchive, string properties) {
        JsonWallCover jsonFloor = JsonUtility.FromJson<JsonWallCover>(properties);
        Texture2D texture = LoadTexture(zipArchive);
        return new WallCover(jsonFloor, texture);
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
    
    private async Task<Furniture> LoadFurniture(ZipArchive zipArchive, string properties) {
        JsonFurniture jsonFurniture = JsonUtility.FromJson<JsonFurniture>(properties);
        Task<GameObject> task = gltfLoader.LoadItem(zipArchive);
        await task;
        GameObject loadedObject = task.Result;
        return new Furniture(jsonFurniture, loadedObject);
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
}

}