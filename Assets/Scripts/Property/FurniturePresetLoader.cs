using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FurniturePresetLoader {
	const string contentPath = "Assets/Resources/Content/";
	const string furniturePath = contentPath + "Furniture/";
	const string furnitureExtension = ".furniture";

	public FurniturePresetLoader () {
		TryCreateDirectory(furniturePath);
	}

	public Dictionary<int, FurniturePreset> LoadAllPresets () {
		string[] files = Directory.GetFiles(furniturePath);
		Dictionary<int, FurniturePreset> presets = new Dictionary<int, FurniturePreset>();
		foreach (string s in files) {
			if (!s.EndsWith(furnitureExtension))
				continue;
			FurniturePreset preset = LoadPreset(s);
			if (!presets.ContainsKey(preset.id)) {
				presets.Add(preset.id, preset);
			} else {
				Debug.LogWarning("Furniture item with id " + preset.id + " already exists. Not loading file " + s + ".");
			}
		}
		return presets;
	}

	public FurniturePreset LoadPreset (string path) {
		return new FurniturePreset(LoadFurniturePresetData(path));
	}

	FurniturePresetData LoadFurniturePresetData (string path) {
		return JsonUtility.FromJson<FurniturePresetData>(ReadFile(path));
	}
		
	string ReadFile (string path) {
		StreamReader reader = new StreamReader(path);
		string data = reader.ReadToEnd();
		reader.Close();
		return data;
	}

	void TryCreateDirectory (string path) {
		if (!Directory.Exists(path)) {
			Debug.Log("Created directory: " + path);
			Directory.CreateDirectory(path);
		}
	}
}
