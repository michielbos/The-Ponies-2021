using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Class that loads all mod asset bundles.
/// </summary>
public static class ModLoader {
	public static List<Mod> loadedMods;
	private static string[] extensions = {".ponybundle", ".ponies", ".tpmod", ".ponmd", ".pon"};

	/// <summary>
	/// Get a list of all loaded mods.
	/// If this is the first time this method is called, all mods will be loaded.
	/// </summary>
	/// <returns>A list with all loaded mods.</returns>
	public static List<Mod> GetLoadedMods () {
		if (loadedMods == null) {
			loadedMods = LoadAllBundles();
		}
		return loadedMods;
	}

	private static List<Mod> LoadAllBundles () {
		string bundleDirectory = Application.dataPath + "/Mods/PonyBundles/";
		if (!Directory.Exists(bundleDirectory)) {
			Directory.CreateDirectory(bundleDirectory);
		}
		List<Mod> mods = new List<Mod>();
		foreach (string file in Directory.GetFiles(bundleDirectory)) {
			bool validExtension = false;
			foreach (string extension in extensions) {
				if (file.EndsWith(extension)) {
					validExtension = true;
					break;
				}
			}
			if (!validExtension)
				continue;
			TryLoadMod(file, mods);
		}
		return mods;
	}

	private static void TryLoadMod (string file, List<Mod> mods) {
		try {
			AssetBundle loadedBundle = AssetBundle.LoadFromFile(file);
			if (loadedBundle != null) {
				ModInfoData modInfo = LoadModInfo(loadedBundle);
				if (ValidateMod(loadedBundle, modInfo, mods)) {
					mods.Add(new Mod(loadedBundle, modInfo));
				} else {
					loadedBundle.Unload(true);
				}
			} else {
				Debug.LogWarning("Failed to load AssetBundle from " + file + ".");
			}
		} catch (Exception e) {
			Debug.LogError("Exception when loading mod " + file + ". Not continuing loading.");
			Debug.LogException(e);
		}
	}

	private static ModInfoData LoadModInfo (AssetBundle assetBundle) {
		foreach (string assetName in assetBundle.GetAllAssetNames()) {
			if (assetName != "assets/modinfo.json")
				continue;
			TextAsset textAsset = assetBundle.LoadAsset<TextAsset>(assetName);
			return JsonUtility.FromJson<ModInfoData>(textAsset.text);
		}
		return null;
	}

	private static bool ValidateMod (AssetBundle assetBundle, ModInfoData modInfo, List<Mod> mods) {
		if (modInfo == null) {
			Debug.LogWarning("AssetBundle " + assetBundle.name + " doesn't have a modinfo file attached. Not loading mod.");
			return false;
		}
		if (modInfo.packId <= 255) {
			Debug.LogWarning("Mod " + assetBundle.name + " has packId " + modInfo.packId +
			                 " which is not allowed (id 0-255 are reserved). Not loading mod.");
			return false;
		}
		foreach (Mod mod in mods) {
			if (modInfo.packId != mod.modInfoData.packId)
				continue;
			Debug.LogWarning("PackId " + modInfo.packId + " is already taken by mod " + mod.assetBundle.name + ". Not loading mod " + assetBundle.name + ".");
			return false;
		}
		return true;
	}
}
