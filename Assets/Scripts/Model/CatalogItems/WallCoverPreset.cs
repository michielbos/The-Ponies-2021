using System;
using PoneCrafter.Model;
using UnityEngine;

/// <summary>
/// Preset for wall covers.
/// </summary>
[Serializable]
public class WallCoverPreset : Preset {
	private Texture texture;
	private Material material;
	
    public WallCoverPreset(WallCover wallCover) :
		base(wallCover.uuid, wallCover.name, wallCover.description, wallCover.price, ObjectCategory.WallCover) {
		texture = wallCover.texture;
	}

	public override Texture[] GetPreviewTextures () {
		return new[] {GetTexture()};
	}

	public int GetSellValue () {
		return Mathf.FloorToInt(price * 0.75f);
	}
	
	private Texture GetTexture () {
		return texture;
	}

	public Material GetMaterial () {
		if (material == null) {
			material = new Material(Shader.Find("Cel Shading/RegularV2"));
            material.mainTexture = GetTexture();
		}
		return material;
	}
}
