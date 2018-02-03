using System;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Preset for furniture items. These are the objects that are displayed in the catalog.
/// </summary>
[Serializable]
public class FurniturePreset : CatalogItem {
	public readonly bool pickupable;
	public readonly bool sellable;
	public readonly string modelName;
	public readonly FurnitureSkinData[] furnitureSkins;
	public readonly Vector3 rotationOffset;
	public readonly Vector3 positionOffset;
	public readonly Vector2Int[] occupiedTiles;
	public AssetBundle assetBundle;
	private Mesh mesh;
	private Material[][] materials;
	private RenderTexture[] previewTextures;

	public FurniturePreset (FurniturePresetData fpd) : 
		base(new Guid(fpd.guid), fpd.name, fpd.description, fpd.price, (ObjectCategory) fpd.category) {
		pickupable = fpd.pickupable;
		sellable = fpd.sellable;
		modelName = fpd.modelName;
		furnitureSkins = fpd.furnitureSkins;
		rotationOffset = fpd.rotationOffset;
		positionOffset = fpd.positionOffset;
		occupiedTiles = fpd.occupiedTiles;
		materials = new Material[furnitureSkins.Length][];
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

	/// <summary>
	/// Get the materials for the given skin.
	/// </summary>
	/// <param name="skin"></param>
	/// <returns></returns>
	public Material[] GetMaterials (int skin) {
		if (materials[skin] == null) {
			string[] materialPaths = furnitureSkins[skin].materialPaths;
			materials[skin] = new Material[materialPaths.Length];
			for (int i = 0; i < materials[skin].Length; i++) {
				if (assetBundle != null) {
					materials[skin][i] = assetBundle.LoadAsset<Material>("assets/" + materialPaths[i]);
				} else {
					materials[skin][i] = Resources.Load<Material>(materialPaths[i]);
				}
			}
		}
		return materials[skin];
	}

	public override Texture[] GetPreviewTextures () {
		if (previewTextures == null) {
			previewTextures = new RenderTexture[furnitureSkins.Length];
		}
		Texture[] textures = new Texture[furnitureSkins.Length];
		for (int i = 0; i < previewTextures.Length; i++) {
			if (previewTextures[i] == null || !previewTextures[i].IsCreated()) {
				GameObject previewGeneratorObj = GameObject.FindGameObjectWithTag("PreviewGenerator");
				PreviewGenerator previewGenerator = previewGeneratorObj.GetComponent<PreviewGenerator>();
				previewTextures[i] = previewGenerator.CreatePreview(this, i);
			}
			textures[i] = previewTextures[i];
		}
		return textures;
	}

	/// <summary>
	/// Place a GameObject with the base rotation, model and materials of this furniture preset.
	/// </summary>
	/// <param name="prefab">The prefab to instantiate. It needs to have a MeshFilter and MeshRenderer.</param>
	/// <param name="position">The position to place the object.</param>
	public GameObject PlaceObject (GameObject prefab, Vector3 position, int skin) {
		GameObject gameObject = Object.Instantiate(prefab);
		ApplyToGameObject(gameObject, position, Vector3.zero, skin, true);
		return gameObject;
	}

	/// <summary>
	/// Update a GameObject by applying the rotation/position offsets, model and materials of this furniture preset to it.
	/// </summary>
	/// <param name="gameObject">The GameObject to apply the update to.</param>
	/// <param name="position">The position of the item, to which the offset will be added.</param>
	/// <param name="rotation">The rotation of the item, to which the offset will be added.</param>
	/// <param name="skin">The skin of the furniture item.</param>
	/// <param name="adjustToTiles">Whether AdjustToTiles should be called.
	/// If true, the object will be positioned with its lowest tile on the given position, instead of its center point.</param>
	public void ApplyToGameObject (GameObject gameObject, Vector3 position, Vector3 rotation, int skin, bool adjustToTiles) {
		ApplyOffsets(gameObject.transform, position, rotation);
		if (adjustToTiles) {
			AdjustToTiles(gameObject.transform);
		}
		if (GetMesh() != null) {
			gameObject.GetComponent<MeshFilter>().mesh = GetMesh();
			gameObject.GetComponent<MeshRenderer>().materials = GetMaterials(skin);
			MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
			if (meshCollider != null) {
				meshCollider.sharedMesh = GetMesh();
			}
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

	/// <summary>
	/// Adjust the given transform to this preset's occupied tiles.
	/// This will move the lower-left tile of this object to the current center position.
	/// </summary>
	/// <param name="transform">The transform to adjust.</param>
	public void AdjustToTiles (Transform transform) {
		int widthTiles = 0;
		int heightTiles = 0;
		foreach (Vector2Int tile in occupiedTiles) {
			if (tile.x >= widthTiles) {
				widthTiles = tile.x + 1;
			}
			if (tile.y >= heightTiles) {
				heightTiles = tile.y + 1;
			}
		}
		transform.position += new Vector3((widthTiles - 1) * 0.5f, 0, (heightTiles - 1) * 0.5f);
	}
	
	/// <summary>
	/// Get the coordinates of the tiles that would be occupied by this PropertyObject if it was on the given position.
	/// </summary>
	/// <returns>A Vector2Int array of all coordinates that would be occupied.</returns>
	public Vector2Int[] GetOccupiedTiles (Vector2Int position) {
		Vector2Int[] occupied = new Vector2Int[occupiedTiles.Length];
		for (int i = 0; i < occupied.Length; i++) {
			occupied[i] = position + occupiedTiles[i];
		}
		return occupied;
	}
}
