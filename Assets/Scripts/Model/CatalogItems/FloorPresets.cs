using System;
using System.Collections.Generic;
using PoneCrafter;
using PoneCrafter.Model;

/// <summary>
/// Singleton class that contains all the FurniturePresets that have been loaded.
/// </summary>
public class FloorPresets {
	private static FloorPresets instance;
	private Dictionary<Guid, FloorPreset> presetDictionary;

	private FloorPresets () {
		presetDictionary = new Dictionary<Guid, FloorPreset>();
		foreach (Floor floor in PoneCrafterImporter.Instance.loadedFloors) {
			presetDictionary.Add(floor.uuid, new FloorPreset(floor));
		}
	}

	public static FloorPresets Instance {
		get { return instance ?? (instance = new FloorPresets()); }
	}

	/// <summary>
	/// Get the FloorPreset with the given id.
	/// </summary>
	/// <param name="guid">The GUID to get a FloorPreset for.</param>
	/// <returns>The FloorPreset matching the id. Returns null if none was found.</returns>
	public FloorPreset GetFloorPreset (Guid guid) {
		if (presetDictionary.ContainsKey(guid))
			return presetDictionary[guid];
		return null;
	}

	/// <summary>
	/// Get a list of all FloorPresets.
	/// </summary>
	/// <returns>A list with all presets that matched the category.</returns>
	public List<FloorPreset> GetFloorPresets () {
		List<FloorPreset> presets = new List<FloorPreset>();
		foreach (FloorPreset preset in presetDictionary.Values) {
			presets.Add(preset);
		}
		return presets;
	}
}
