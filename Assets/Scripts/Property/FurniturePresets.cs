using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePresets {
	private static FurniturePresets instance;
	private Dictionary<int, FurniturePreset> presetDictionary;

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

	public FurniturePreset GetFurniturePreset (int id) {
		if (presetDictionary.ContainsKey(id))
			return presetDictionary[id];
		else
			return null;
	}
}
