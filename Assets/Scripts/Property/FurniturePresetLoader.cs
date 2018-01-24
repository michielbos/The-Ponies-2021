using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class for loading the furniture presets from the resources, as well as the mod packages.
/// </summary>
public class FurniturePresetLoader {
	const string contentPath = "Assets/Resources/Content/";
	const string furniturePath = contentPath + "Furniture/";
	const string furnitureExtension = ".furniture";

	public FurniturePresetLoader () {
		TryCreateDirectory(furniturePath);
	}

	/// <summary>
	/// Load all furniture presets, from both the resources and the mod packages.
	/// </summary>
	/// <returns>A dictionary with all loaded furniture presets.</returns>
	public Dictionary<int, FurniturePreset> LoadAllPresets () {
		Dictionary<int, FurniturePreset> presets = new Dictionary<int, FurniturePreset>();
		LoadResourcePresets(presets);
		LoadModPresets(presets);
		return presets;
	}

	private void LoadResourcePresets (Dictionary<int, FurniturePreset> presets) {
		string[] files = Directory.GetFiles(furniturePath);
		foreach (string s in files) {
			if (!s.EndsWith(furnitureExtension))
				continue;
			AddFurniturePreset(presets, s, LoadPreset(s));
		}
	}

	private void LoadModPresets (Dictionary<int, FurniturePreset> presets) {
		AssetBundleLoader.LoadAllBundles();
		foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles()) {
			string basePath = "assets/" + bundle.name + "/";
			foreach (string assetName in bundle.GetAllAssetNames()) {
				if (!assetName.StartsWith(basePath + "furniture/"))
					continue;
				TextAsset textAsset = bundle.LoadAsset<TextAsset>(assetName);
				if (textAsset == null) {
					Debug.Log(assetName + " is not a furniture asset. Skipping.");
					continue;
				}
				FurniturePreset preset =
					new FurniturePreset(JsonUtility.FromJson<FurniturePresetData>(textAsset.text)) {assetBundle = bundle};
				AddFurniturePreset(presets, assetName, preset);
			}
		}
	}
	
	private void AddFurniturePreset (Dictionary<int, FurniturePreset> presets, string file, FurniturePreset preset) {
		if (!presets.ContainsKey(preset.id)) {
			presets.Add(preset.id, preset);
		} else {
			Debug.LogWarning("Furniture item with id " + preset.id + " already exists. Not loading file " + file + ".");
		}
	}

	private FurniturePreset LoadPreset (string path) {
		return new FurniturePreset(LoadFurniturePresetData(path));
	}

	private FurniturePresetData LoadFurniturePresetData (string path) {
		return JsonUtility.FromJson<FurniturePresetData>(ReadFile(path));
	}
		
	private string ReadFile (string path) {
		StreamReader reader = new StreamReader(path);
		string data = reader.ReadToEnd();
		reader.Close();
		return data;
	}

	private void TryCreateDirectory (string path) {
		if (!Directory.Exists(path)) {
			Debug.Log("Created directory: " + path);
			Directory.CreateDirectory(path);
		}
	}
}
