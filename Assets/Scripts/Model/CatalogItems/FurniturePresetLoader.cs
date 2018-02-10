using UnityEngine;

/// <summary>
/// Class for loading the furniture presets from the resources, as well as the mod packages.
/// </summary>
public class FurniturePresetLoader : PresetLoader<FurniturePreset, FurniturePresetData> {
	const string furnitureFolder = "Furniture/";
	const string furnitureExtension = ".furniture";

	public FurniturePresetLoader () : 
		base(furnitureFolder, furnitureExtension) { }

	public override FurniturePreset CreateFromData (FurniturePresetData presetData) {
		return new FurniturePreset(presetData);
	}

	protected override bool ValidatePreset (FurniturePreset preset) {
		//We are probably still missing a lot of checks.
		if (preset.furnitureSkins == null || preset.furnitureSkins.Length <= 0) {
			Debug.LogWarning("Preset has no furniture skins.");
			return false;
		}
		return true;
	}

}
