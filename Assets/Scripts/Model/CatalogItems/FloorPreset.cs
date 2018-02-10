using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FloorPreset : Preset {
	private readonly string textureName;
	private Texture texture;
	private Material material;
	
	public FloorPreset (FloorPresetData fpd) : 
		base(new Guid(fpd.guid), fpd.name, fpd.description, fpd.price, ObjectCategory.Floors) {
		textureName = fpd.textureName;
	}

	public override Texture[] GetPreviewTextures () {
		return new Texture[] {GetTexture()};
	}
	
	private Texture GetTexture () {
		Debug.Log(textureName);
		if (texture == null) {
			if (assetBundle != null) {
				texture = assetBundle.LoadAsset<Texture>("assets/" + textureName);
			} else {
				texture = Resources.Load<Texture>(textureName);
			}
		}
		//TODO: Use placeholder texture if still null...
		return texture;
	}
	
	private Material GetMaterial () {
		if (material == null) {
			//TODO: Use the right shader or use a source material.
			material = new Material(Shader.Find("Diffuse"));
		}
		return material;
	}

	/// <summary>
	/// Place a floor GameObject with the texture of this floor preset.
	/// </summary>
	/// <param name="prefab">The floor prefab to instantiate.</param>
	/// <param name="x">The X coordinate of the floor tile.</param>
	/// <param name="y">The Y coordinate of the floor tile.</param>
	public GameObject PlaceFloor (GameObject prefab, int x, int y) {
		GameObject gameObject = Object.Instantiate(prefab);
		return gameObject;
	}

}
