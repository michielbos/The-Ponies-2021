using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FurniturePreset {
	public int id;
	public string name;
	public string description;
	public int price;
	public ObjectCategory category;
	public string modelName;
	public string[] materialPaths;
	public Vector3 addRotation;
	public AssetBundle assetBundle;
	private Mesh mesh;
	private Material[] materials;
	private RenderTexture previewTexture;

	public FurniturePreset (int id, string name, string description, int price, ObjectCategory category, 
		string modelName, string[] materialPaths, Vector3 addRotation) {
		this.id = id;
		this.name = name;
		this.description = description;
		this.price = price;
		this.category = category;
		this.modelName = modelName;
		this.materialPaths = materialPaths;
		this.addRotation = addRotation;
	}

	public FurniturePreset (FurniturePresetData fpd) 
		: this(fpd.id,
			fpd.name,
			fpd.description,
			fpd.price,
			(ObjectCategory) fpd.category,
			fpd.modelName,
			fpd.materialPaths,
			fpd.addRotation) {

	}

	public Mesh GetMesh () {
		if (mesh == null) {
			if (assetBundle != null) {
				mesh = assetBundle.LoadAsset<Mesh>("assets/" + assetBundle.name + "/" + modelName);
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
					materials[i] = assetBundle.LoadAsset<Material>("assets/" + assetBundle.name + "/" + materialPaths[i]);
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
		GameObject gameObject = Object.Instantiate(prefab, position, Quaternion.identity);
		ApplyToGameObject(gameObject);
		return gameObject;
	}

	/// <summary>
	/// Update a GameObject by applying the base rotation, model and materials of this furniture preset to it.
	/// </summary>
	/// <param name="gameObject"></param>
	public void ApplyToGameObject (GameObject gameObject) {
		gameObject.transform.rotation = Quaternion.Euler(addRotation);
		if (GetMesh() != null) {
			gameObject.GetComponent<MeshFilter>().mesh = GetMesh();
			gameObject.GetComponent<MeshRenderer>().materials = GetMaterials();
		}
	}
}
