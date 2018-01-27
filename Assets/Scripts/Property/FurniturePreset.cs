﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FurniturePreset {
	public Guid guid;
	public string name;
	public string description;
	public int price;
	public ObjectCategory category;
	public string modelName;
	public string[] materialPaths;
	public Vector3 rotationOffset;
	public Vector3 positionOffset;
	public AssetBundle assetBundle;
	private Mesh mesh;
	private Material[] materials;
	private RenderTexture previewTexture;

	public FurniturePreset (Guid guid, string name, string description, int price, ObjectCategory category, 
		string modelName, string[] materialPaths, Vector3 rotationOffset, Vector3 positionOffset) {
		this.guid = guid;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
		this.modelName = modelName;
		this.materialPaths = materialPaths;
		this.rotationOffset = rotationOffset;
		this.positionOffset = positionOffset;
	}

	public FurniturePreset (FurniturePresetData fpd) 
		: this(new Guid(fpd.guid),
			fpd.name,
			fpd.description,
			fpd.price,
			(ObjectCategory) fpd.category,
			fpd.modelName,
			fpd.materialPaths,
			fpd.rotationOffset,
			fpd.positionOffset) {

	}

	public Mesh GetMesh () {
		if (mesh == null) {
			if (assetBundle != null) {
				mesh = assetBundle.LoadAsset<Mesh>("assets/" + modelName);
			} else {
				mesh = Resources.Load<Mesh>(modelName);
			}
		}
		return mesh;
	}

	public Material[] GetMaterials () {
		if (materials == null) {
			materials = new Material[materialPaths.Length];
			for (int i = 0; i < materialPaths.Length; i++) {
				if (assetBundle != null) {
					materials[i] = assetBundle.LoadAsset<Material>("assets/" + materialPaths[i]);
				} else {
					materials[i] = Resources.Load<Material>(materialPaths[i]);
				}
			}
		}
		return materials;
	}

	public Texture GetPreviewTexture () {
		if (previewTexture == null || !previewTexture.IsCreated()) {
			GameObject previewGeneratorObj = GameObject.FindGameObjectWithTag("PreviewGenerator");
			PreviewGenerator previewGenerator = previewGeneratorObj.GetComponent<PreviewGenerator>();
			previewTexture = previewGenerator.CreatePreview(this);
		}
		return previewTexture;
	}
	
	/// <summary>
	/// Place a GameObject with the base rotation, model and materials of this furniture preset.
	/// </summary>
	/// <param name="prefab">The prefab to instantiate. It needs to have a MeshFilter and MeshRenderer.</param>
	/// <param name="position">The position to place the object.</param>
	public GameObject PlaceObject (GameObject prefab, Vector3 position) {
		GameObject gameObject = Object.Instantiate(prefab);
		ApplyToGameObject(gameObject, position, Vector3.zero);
		return gameObject;
	}

	/// <summary>
	/// Update a GameObject by applying the rotation/position offsets, model and materials of this furniture preset to it.
	/// </summary>
	/// <param name="gameObject">The GameObject to apply the update to.</param>
	/// <param name="position">The position of the item, to which the offset will be added.</param>
	/// <param name="rotation">The rotation of the item, to which the offset will be added.</param>
	public void ApplyToGameObject (GameObject gameObject, Vector3 position, Vector3 rotation) {
		ApplyOffsets(gameObject.transform, position, rotation);
		if (GetMesh() != null) {
			gameObject.GetComponent<MeshFilter>().mesh = GetMesh();
			gameObject.GetComponent<MeshRenderer>().materials = GetMaterials();
		}
	}

	/// <summary>
	/// Apply the rotation and position offsets for this furniture preset to a Transform.
	/// </summary>
	/// <param name="transform">The Transform to apply the offsets to.</param>
	public void ApplyOffsets (Transform transform) {
		ApplyOffsets(transform, transform.position, transform.eulerAngles);
	}
	
	/// <summary>
	/// Apply the rotation and position offsets for this furniture preset to a Transform.
	/// </summary>
	/// <param name="transform">The GameObject to apply the offsets to.</param>
	/// <param name="position">The position of the item, to which the offset will be added.</param>
	/// <param name="rotation">The rotation of the item, to which the offset will be added.</param>
	public void ApplyOffsets (Transform transform, Vector3 position, Vector3 rotation) {
		transform.rotation = Quaternion.Euler(rotation + rotationOffset);
		transform.position = position + positionOffset;
	}
}
