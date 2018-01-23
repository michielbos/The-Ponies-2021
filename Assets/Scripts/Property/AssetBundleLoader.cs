using System.IO;
using UnityEngine;

/// <summary>
/// Small class that loads all asset bundles.
/// </summary>
public class AssetBundleLoader {
	private static bool loaded;
	private static string[] extensions = {".ponybundle", ".ponies", ".tpmod", ".ponmd", ".pon"};

	private AssetBundleLoader () {
		//Private constructor
	}

	/// <summary>
	/// Load all asset bundles in the asset bundles folder.
	/// If this method has already been called, nothing will happen.
	/// </summary>
	public static void LoadAllBundles () {
		if (loaded)
			return;
		
		string bundleDirectory = Application.dataPath + "/Mods/PonyBundles/";
		if (!Directory.Exists(bundleDirectory)) {
			Directory.CreateDirectory(bundleDirectory);
		}

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
			AssetBundle loadedBundle = AssetBundle.LoadFromFile(file);
			if (loadedBundle == null) {
				Debug.Log("Failed to load AssetBundle from " + file + ".");
				return;
			}
		}
		loaded = true;
	}
}
