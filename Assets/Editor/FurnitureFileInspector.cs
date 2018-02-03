using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds an editor inspector on .furniture items.
/// Useful for reading and updating furniture files.
/// </summary>
[CustomEditor(typeof(DefaultAsset))]
public class FurnitureFileInspector : Editor {
	private const string RESOURCE_FOLDER = "Assets/Resources/";
	private string path;
	private DefaultAsset furnitureFile;
	private FurniturePresetData presetData;
	private Mesh presetModel;
	private Material[][] presetMaterials;

	private void OnEnable () {
		furnitureFile = (DefaultAsset)target;
		path = AssetDatabase.GetAssetPath(furnitureFile);
		if (path.ToLower().EndsWith(".furniture")) {
			presetData = GetFurniturePresetData();
			presetModel = Resources.Load<Mesh>(presetData.modelName);
			presetMaterials = new Material[presetData.furnitureSkins.Length][];
			for (int skin = 0; skin < presetMaterials.Length; skin++) {
				string[] materialPaths = presetData.furnitureSkins[skin].materialPaths;
				presetMaterials[skin] = new Material[materialPaths.Length];
				for (int i = 0; i < presetMaterials[skin].Length; i++) {
					presetMaterials[skin][i] = Resources.Load<Material>(presetData.furnitureSkins[skin].materialPaths[i]);
				}
			}
		}
	}

	public override void OnInspectorGUI() {
		if (presetData == null)
			return;
		GUI.enabled = true;
		GUILayout.Label("Furniture file: " + furnitureFile);
		presetData.guid = EditorGUILayout.TextField("GUID", presetData.guid);
		if (GUILayout.Button("Generate GUID")) {
			presetData.guid = GuidUtil.GenerateAssetGuid(0).ToString();
		}
		presetData.name = EditorGUILayout.TextField("Name", presetData.name);
		presetData.description = EditorGUILayout.TextField("Description", presetData.description);
		presetData.price = EditorGUILayout.IntField("Price", presetData.price);
		presetData.category = (int)(ObjectCategory) EditorGUILayout.EnumPopup("Category", (ObjectCategory) presetData.category);
		presetData.pickupable = EditorGUILayout.Toggle("Pickupable", presetData.pickupable);
		GUI.enabled = presetData.pickupable;
		presetData.sellable = EditorGUILayout.Toggle("Sellable", presetData.pickupable && presetData.sellable);
		GUI.enabled = true;
		presetModel = (Mesh) EditorGUILayout.ObjectField("Model", presetModel, typeof(Mesh), false);
		EditorGUILayout.LabelField("Skins");
		presetMaterials = JaggedArrayGuiField(presetMaterials, MaterialGuiField);
		presetData.rotationOffset = EditorGUILayout.Vector3Field("Rotation offset", presetData.rotationOffset);
		presetData.positionOffset = EditorGUILayout.Vector3Field("Position offset", presetData.positionOffset);
		EditorGUILayout.LabelField("Occupied tiles");
		presetData.occupiedTiles = ArrayGuiField(presetData.occupiedTiles, Vector2IntGuiField);
		if (GUILayout.Button("Apply changes")) {
			ApplyChanges();
		}
		GUILayout.Label("Content:");
		GUILayout.Box(GetContent());
	}

	private T[] ArrayGuiField <T> (T[] arr, Func<int, T, T> fieldFunc) {
		if (arr == null) {
			arr = new T[0];
		}
		int length = Mathf.Clamp(EditorGUILayout.IntField("Length", arr.Length), 0, 100);
		if (arr.Length != length) {
			T[] oldArr = arr;
			arr = new T[length];
			Array.Copy(oldArr, arr, Mathf.Min(oldArr.Length, arr.Length));
		}
		for (int i = 0; i < arr.Length; i++) {
			arr[i] = fieldFunc(i, arr[i]);
		}
		return arr;
	}
	
	private T[][] JaggedArrayGuiField <T> (T[][] arr, Func<int, T, T> fieldFunc) {
		if (arr == null) {
			arr = new T[0][];
		}
		int height = Mathf.Clamp(EditorGUILayout.IntField("Height", arr.Length), 0, 100);
		if (arr.Length != height) {
			T[][] oldArr = arr;
			arr = new T[height][];
			Array.Copy(oldArr, arr, Mathf.Min(oldArr.Length, arr.Length));
		}
		for (int h = 0; h < arr.Length; h++) {
			EditorGUILayout.LabelField("Array " + h);
			arr[h] = ArrayGuiField(arr[h], fieldFunc);
		}
		return arr;
	}
	
	private Material MaterialGuiField (int index, Material material) {
		return (Material) EditorGUILayout.ObjectField("Material " + index, material, typeof(Material), false);
	}
	
	private Vector2Int Vector2IntGuiField (int index, Vector2Int vector2Int) {
		return EditorGUILayout.Vector2IntField("Tile " + index, vector2Int);
	}
	
	private string GetContent () {
		return File.ReadAllText(path);
	}

	private FurniturePresetData GetFurniturePresetData () {
		return JsonUtility.FromJson<FurniturePresetData>(GetContent());
	}
	
	private void SetContent (string content) {
		File.WriteAllText(path, content);
	}

	private void ApplyChanges () {
		presetData.modelName = ToResourcePath(AssetDatabase.GetAssetPath(presetModel));
		presetData.furnitureSkins = new FurnitureSkinData[presetMaterials.Length];
		for (int skin = 0; skin < presetMaterials.Length; skin++) {
			presetData.furnitureSkins[skin] = new FurnitureSkinData(new string[presetMaterials[skin].Length]); 
			for (int i = 0; i < presetMaterials[skin].Length; i++) {
				presetData.furnitureSkins[skin].materialPaths[i] = ToResourcePath(AssetDatabase.GetAssetPath(presetMaterials[skin][i]));
			}
		}
		SetContent(JsonUtility.ToJson(presetData));
	}
	
	private string ToResourcePath (string path) {
		if (!path.Contains(".")) {
			Debug.LogWarning("Resource path is wrong or not set. Saving empty string.");
			return "";
		} 
		if (path.StartsWith(RESOURCE_FOLDER)) {
			return path.Substring(RESOURCE_FOLDER.Length, path.LastIndexOf('.') - RESOURCE_FOLDER.Length);
		} 
		Debug.LogWarning("Resource was not in resource path. Saved link might be wrong.");
		return path.Substring(0, path.LastIndexOf('.'));
	}
}
