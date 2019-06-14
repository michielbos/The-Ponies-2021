using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Base class for loading the presets (catalog items) from the resources and mod packages.
/// </summary>
/// <typeparam name="T">The type of the preset to load.</typeparam>
/// <typeparam name="TData">The data type of the preset to load.</typeparam>
public abstract class PresetLoader <T, TData> where T : Preset {
	private const string RESOURCE_CONTENT_PATH = "Assets/Resources/Content/";
	private const string MOD_CONTENT_PATH = "Assets/";
	private string presetFolder;
	private string presetExtension;

	/// <summary>
	/// Construct a PresetLoader.
	/// </summary>
	/// <param name="presetFolder">The folder of the preset, for example "Furniture/".
	/// Remember that the whole path will be "Assets/Resources/Content/(folder)" for resources and "Assets/(folder)" for mods.</param>
	/// <param name="presetExtension">The extension of the preset, for example ".furniture".</param>
	public PresetLoader (string presetFolder, string presetExtension) {
		this.presetFolder = presetFolder;
		this.presetExtension = presetExtension;
	}
	
	/// <summary>
	/// Create an instance of a preset with the given preset data.
	/// </summary>
	/// <param name="presetData">The data to construct the preset from.</param>
	/// <returns>Should return a preset constructed from the given data.</returns>
	public abstract T CreateFromData (TData presetData);

	/// <summary>
	/// Load all presets, from both the resources and the mod packages.
	/// </summary>
	/// <returns>A dictionary with all loaded presets.</returns>
	public Dictionary<Guid, T> LoadAllPresets () {
		TryCreateDirectory(RESOURCE_CONTENT_PATH + presetFolder);
		Dictionary<Guid, T> presets = new Dictionary<Guid, T>();
		LoadResourcePresets(presets);
		LoadModPresets(presets);
		return presets;
	}

	private void LoadResourcePresets (Dictionary<Guid, T> presets) {
		string[] files = Directory.GetFileSystemEntries(RESOURCE_CONTENT_PATH + presetFolder, "*", SearchOption.AllDirectories);
		foreach (string s in files) {
			if (!s.EndsWith(presetExtension))
				continue;
			T preset = CreateFromData(JsonUtility.FromJson<TData>(ReadFile(s)));
			uint packIdentifier = GuidUtil.GetPackIdentifier(preset.guid);
			if (packIdentifier != 0) {
				Debug.LogWarning("Preset in file " + s + " has pack identifier " + packIdentifier + " instead of 0.");
			}
			AddPreset(presets, s, preset);
		}
	}

	private void LoadModPresets (Dictionary<Guid, T> presets) {
		foreach (Mod mod in ModLoader.GetLoadedMods()) {
			AssetBundle bundle = mod.assetBundle;
			foreach (string assetName in bundle.GetAllAssetNames()) {
				if (!assetName.StartsWith(MOD_CONTENT_PATH + presetFolder))
					continue;
				TextAsset textAsset = bundle.LoadAsset<TextAsset>(assetName);
				if (textAsset == null) {
					Debug.Log(assetName + " is not a text asset. Skipping.");
					continue;
				}
				try {
					T preset = CreateFromData(JsonUtility.FromJson<TData>(textAsset.text));
					// TODO: Apparently there is still some AssetBundle junk left. Destroy it.
					// preset.assetBundle = bundle;
					uint packId = GuidUtil.GetPackIdentifier(preset.guid);
					if (packId == mod.modInfoData.packId) {
						AddPreset(presets, assetName, preset);
					} else {
						Debug.LogWarning("Preset item " + assetName + " in " + bundle.name + " has packId " + packId + " while the mod has packId " + mod.modInfoData.packId + ". Skipping.");
					}
				} catch (Exception e) {
					Debug.LogError("Exception when trying to load preset item " + assetName + " from " + bundle.name + "! Not loading item.");
					Debug.LogException(e);
				}
			}
		}
	}
	
	private void AddPreset (Dictionary<Guid, T> presets, string file, T preset) {
		if (!presets.ContainsKey(preset.guid)) {
			if (ValidatePreset(preset)) {
				presets.Add(preset.guid, preset);
			} else {
				Debug.LogWarning("Preset item " + preset.guid + " (" + file + ") didn't validate. Not loading.");
			}
		} else {
			Debug.LogWarning("Preset item with GUID " + preset.guid + " already exists. Not loading file " + file + ".");
		}
	}

	/// <summary>
	/// Validate the preset for errors. If the validation fails, the preset will not be loaded.
	/// Any errors found during validation should be logged.
	/// </summary>
	/// <param name="preset">The preset to validate.</param>
	/// <returns>Should return true if the validation passed, false otherwise.</returns>
	protected abstract bool ValidatePreset (T preset);

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
