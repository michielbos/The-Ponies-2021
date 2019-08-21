using System;
using System.Collections.Generic;
using PoneCrafter;
using PoneCrafter.Model;

/// <summary>
/// Singleton class that contains all the wall cover presets that have been loaded.
/// </summary>
public class WallCoverPresets {
	private static WallCoverPresets instance;
	private Dictionary<Guid, WallCoverPreset> presetDictionary;

	private WallCoverPresets () {
		presetDictionary = new Dictionary<Guid, WallCoverPreset>();
		foreach (WallCover wallCover in PoneCrafterImporter.Instance.loadedWallCovers) {
			presetDictionary.Add(wallCover.uuid, new WallCoverPreset(wallCover));
		}
	}

	public static WallCoverPresets Instance => instance ?? (instance = new WallCoverPresets());

	/// <summary>
	/// Get the WallCoverPreset with the given id.
	/// </summary>
	/// <param name="guid">The GUID to get a WallCoverPreset for.</param>
	/// <returns>The WallCoverPreset matching the id. Returns null if none was found.</returns>
	public WallCoverPreset GetWallCoverPreset (Guid guid) {
		if (presetDictionary.ContainsKey(guid))
			return presetDictionary[guid];
		return null;
	}

	/// <summary>
	/// Get a list of all WallCoverPresets.
	/// </summary>
	/// <returns>A list with all presets that matched the category.</returns>
	public List<WallCoverPreset> GetWallCoverPresets () {
		List<WallCoverPreset> presets = new List<WallCoverPreset>();
		foreach (WallCoverPreset preset in presetDictionary.Values) {
			presets.Add(preset);
		}
		return presets;
	}
}
