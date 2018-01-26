using UnityEngine;

public class Mod {
	public AssetBundle assetBundle;
	public ModInfoData modInfoData;

	public Mod (AssetBundle assetBundle, ModInfoData modInfoData) {
		this.assetBundle = assetBundle;
		this.modInfoData = modInfoData;
	}
}
