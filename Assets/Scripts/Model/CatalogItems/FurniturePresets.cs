using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class that contains all the FurniturePresets that have been loaded.
/// </summary>
public class FurniturePresets {
	private static FurniturePresets instance;
	private Dictionary<Guid, FurniturePreset> presetDictionary;

	private FurniturePresets () {
		presetDictionary = new FurniturePresetLoader().LoadAllPresets();
	}

	public static FurniturePresets Instance {
		get {
			if (instance == null) {
				instance = new FurniturePresets();
			}
			return instance;
		}
	}

	/// <summary>
	/// Get the FurniturePreset with the given id.
	/// </summary>
	/// <param name="guid">The GUID to get a FurniturePreset for.</param>
	/// <returns>The FurniturePreset matching the id. Returns null if none was found.</returns>
	public FurniturePreset GetFurniturePreset (Guid guid) {
		if (presetDictionary.ContainsKey(guid))
			return presetDictionary[guid];
		return null;
	}

	/// <summary>
	/// Get a list of all FurniturePresets that belong to the given category.
	/// </summary>
	/// <param name="category">The category to get all presets from.</param>
	/// <returns>A list with all presets that matched the category.</returns>
	public List<FurniturePreset> GetFurniturePresets (ObjectCategory category) {
		List<FurniturePreset> presets = new List<FurniturePreset>();
		foreach (FurniturePreset preset in presetDictionary.Values) {
			if (preset.category == category) {
				presets.Add(preset);
			}
		}
		return presets;
	}
}
