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
	public Dictionary<Guid, FurniturePreset> LoadAllPresets () {
		Dictionary<Guid, FurniturePreset> presets = new Dictionary<Guid, FurniturePreset>();
		LoadResourcePresets(presets);
		LoadModPresets(presets);
		return presets;
	}

	private void LoadResourcePresets (Dictionary<Guid, FurniturePreset> presets) {
		string[] files = Directory.GetFiles(furniturePath);
		foreach (string s in files) {
			if (!s.EndsWith(furnitureExtension))
				continue;
			FurniturePreset preset = LoadPreset(s);
			uint packIdentifier = GuidUtil.GetPackIdentifier(preset.guid);
			if (packIdentifier != 0) {
				Debug.LogWarning("Furniture preset in file " + s + " has pack identifier " + packIdentifier + " instead of 0.");
			}
			AddFurniturePreset(presets, s, preset);
		}
	}

	private void LoadModPresets (Dictionary<Guid, FurniturePreset> presets) {
		foreach (Mod mod in ModLoader.GetLoadedMods()) {
			AssetBundle bundle = mod.assetBundle;
			foreach (string assetName in bundle.GetAllAssetNames()) {
				if (!assetName.StartsWith("assets/furniture/"))
					continue;
				TextAsset textAsset = bundle.LoadAsset<TextAsset>(assetName);
				if (textAsset == null) {
					Debug.Log(assetName + " is not a furniture asset. Skipping.");
					continue;
				}
				try {
					FurniturePreset preset =
						new FurniturePreset(JsonUtility.FromJson<FurniturePresetData>(textAsset.text)) {assetBundle = bundle};
					uint packId = GuidUtil.GetPackIdentifier(preset.guid);
					if (packId == mod.modInfoData.packId) {
						AddFurniturePreset(presets, assetName, preset);
					} else {
						Debug.LogWarning("Furniture item " + assetName + " in " + bundle.name + " has packId " + packId + " while the mod has packId " + mod.modInfoData.packId + ". Skipping.");
					}
				} catch (Exception e) {
					Debug.LogError("Exception when trying to load furniture item " + assetName + " from " + bundle.name + "! Not loading furniture item.");
					Debug.LogException(e);
				}
			}
		}
	}
	
	private void AddFurniturePreset (Dictionary<Guid, FurniturePreset> presets, string file, FurniturePreset preset) {
		if (!presets.ContainsKey(preset.guid)) {
			if (ValidateFurniturePreset(preset)) {
				presets.Add(preset.guid, preset);
			} else {
				Debug.LogWarning("Furniture item " + preset.guid + " (" + file + ") didn't validate. Not loading.");
			}
		} else {
			Debug.LogWarning("Furniture item with GUID " + preset.guid + " already exists. Not loading file " + file + ".");
		}
	}

	private bool ValidateFurniturePreset (FurniturePreset preset) {
		//We are probably still missing a lot of checks.
		if (preset.furnitureSkins == null || preset.furnitureSkins.Length <= 0) {
			Debug.LogWarning("Preset has no furniture skins.");
			return false;
		}
		return true;
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
