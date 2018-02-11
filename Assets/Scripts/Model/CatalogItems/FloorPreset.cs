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
			material = new Material(Shader.Find("Standard"));
			material.mainTexture = GetTexture();
		}
		return material;
	}

	/// <summary>
	/// Place a floor GameObject with the texture of this floor preset.
	/// </summary>
	/// <param name="prefab">The floor prefab to instantiate.</param>
	/// <param name="position">The position of the floor tile.</param>
	public FloorTileDummy PlaceFloor (GameObject prefab, Vector3 position) {
		GameObject gameObject = Object.Instantiate(prefab, position, Quaternion.identity);
		return gameObject.GetComponent<FloorTileDummy>();
	}

	/// <summary>
	/// Apply this floor preset's material to the given GameObject.
	/// </summary>
	/// <param name="gameObject">The GameObject to apply to. It must have a renderer.</param>
	public void ApplyToGameObject (GameObject gameObject) {
		gameObject.GetComponent<Renderer>().material = GetMaterial();
	}

}
