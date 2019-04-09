using System;
using Model.Property;
using PoneCrafter.Model;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FloorPreset : Preset {
	private Texture texture;
	private Material material;

	public FloorPreset(Floor floor) :
		base(floor.uuid, floor.name, floor.description, floor.price, ObjectCategory.Floors) {
		texture = floor.texture;
	}

	public override Texture[] GetPreviewTextures () {
		return new Texture[] {GetTexture()};
	}

	public int GetSellValue () {
		return Mathf.FloorToInt(price * 0.75f);
	}
	
	private Texture GetTexture () {
		return texture;
	}
	
	private Material GetMaterial () {
		if (material == null) {
			//TODO: Use the right shader or use a source material.
			material = new Material(Shader.Find("Cel Shading/SurfaceDoubleSided"));
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
