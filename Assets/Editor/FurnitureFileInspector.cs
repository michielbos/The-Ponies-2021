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
	private bool skinsFoldout;
	private bool occupiedTilesFoldout;
	private bool placementRestrictionsFoldout;
	private bool needStatsFoldout;
	private bool skillStatsFoldout;

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
		skinsFoldout = EditorGUILayout.Foldout(skinsFoldout, "Skins");
		if (skinsFoldout) {
			presetMaterials = JaggedArrayGuiField(presetMaterials, MaterialGuiField);
		}
		presetData.rotationOffset = EditorGUILayout.Vector3Field("Rotation offset", presetData.rotationOffset);
		presetData.positionOffset = EditorGUILayout.Vector3Field("Position offset", presetData.positionOffset);
		occupiedTilesFoldout = EditorGUILayout.Foldout(occupiedTilesFoldout, "Occupied tiles");
		if (occupiedTilesFoldout) {
			presetData.occupiedTiles = ArrayGuiField(presetData.occupiedTiles, Vector2IntGuiField);
		}
		CreatePlacementRestrictionsFields(presetData);
		CreateNeedStatsFields(presetData);
		CreateSkillStatsFields(presetData);
		presetData.requiredAge = (RequiredAge) EditorGUILayout.EnumPopup("Required age", presetData.requiredAge);
		if (GUILayout.Button("Apply changes")) {
			ApplyChanges();
		}
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Content:");
		GUILayout.Box(GetContent());
	}
	
	private void CreatePlacementRestrictionsFields (FurniturePresetData fpd) {
		placementRestrictionsFoldout = EditorGUILayout.Foldout(placementRestrictionsFoldout, "Placement restrictions");
		if (!placementRestrictionsFoldout) 
			return;
		if (fpd.placementRestrictions == null) {
			fpd.placementRestrictions = new PlacementRestriction[0];
		}
		string[] enumNames = Enum.GetNames(typeof(PlacementRestriction));
		string[] options = new string[enumNames.Length + 1];
		int[] values = new int[options.Length];
		options[0] = "Remove";
		values[0] = -1;
		Array.Copy(enumNames, 0, options, 1, enumNames.Length);
		Array.Copy(Enum.GetValues(typeof(PlacementRestriction)), 0, values, 1, enumNames.Length);
		for (int i = 0; i < fpd.placementRestrictions.Length; i++) {
			int value = EditorGUILayout.IntPopup("Option " + i, (int) fpd.placementRestrictions[i], options, values);
			if (value != -1) {
				fpd.placementRestrictions[i] = (PlacementRestriction) value;
			} else {
				//Shift all elements after the removed element left.
				for (int r = 0; r < fpd.placementRestrictions.Length; r++) {
					if (r > i) {
						fpd.placementRestrictions[r - 1] = fpd.placementRestrictions[r];
					}
				}
				PlacementRestriction[] oldRestrictions = fpd.placementRestrictions;
				fpd.placementRestrictions = new PlacementRestriction[oldRestrictions.Length - 1];
				Array.Copy(oldRestrictions, fpd.placementRestrictions, fpd.placementRestrictions.Length);
			}
		}
		options[0] = "Select to add...";
		int addValue = EditorGUILayout.IntPopup("New", -1, options, values);
		if (addValue != -1) {
			PlacementRestriction[] tempArr = fpd.placementRestrictions;
			fpd.placementRestrictions = new PlacementRestriction[tempArr.Length + 1];
			Array.Copy(tempArr, fpd.placementRestrictions, tempArr.Length);
			fpd.placementRestrictions[tempArr.Length] = (PlacementRestriction) addValue;
		}
	}

	//This actually kind of belongs in NeedStats, but for the sake of separating editor code...
	private void CreateNeedStatsFields (FurniturePresetData fpd) {
		needStatsFoldout = EditorGUILayout.Foldout(needStatsFoldout, "Need stats");
		if (!needStatsFoldout) 
			return;
		NeedStats needStats = fpd.needStats;
		needStats.hunger = EditorGUILayout.IntField("Hunger", needStats.hunger);
		needStats.energy = EditorGUILayout.IntField("Energy", needStats.energy);
		needStats.comfort = EditorGUILayout.IntField("Comfort", needStats.comfort);
		needStats.fun = EditorGUILayout.IntField("Fun", needStats.fun);
		needStats.hygiene = EditorGUILayout.IntField("Hygiene", needStats.hygiene);
		needStats.social = EditorGUILayout.IntField("Social", needStats.social);
		needStats.bladder = EditorGUILayout.IntField("Bladder", needStats.bladder);
		needStats.room = EditorGUILayout.IntField("Room", needStats.room);
	}
	
	//Same here for SkillStats...
	private void CreateSkillStatsFields (FurniturePresetData fpd) {
		skillStatsFoldout = EditorGUILayout.Foldout(skillStatsFoldout, "Skill stats");
		if (!skillStatsFoldout) 
			return;
		SkillStats skillStats = fpd.skillStats;
		//The spaghetti of 0's and 1's really just means we're converting between ints and booleans.
		skillStats.cooking = EditorGUILayout.Toggle("Cooking", skillStats.cooking != 0) ? 1 : 0;
		skillStats.mechanical = EditorGUILayout.Toggle("Mechanical", skillStats.mechanical != 0) ? 1 : 0;
		skillStats.charisma = EditorGUILayout.Toggle("Charisma", skillStats.charisma != 0) ? 1 : 0;
		skillStats.logic = EditorGUILayout.Toggle("Logic", skillStats.logic != 0) ? 1 : 0;
		skillStats.body = EditorGUILayout.Toggle("Body", skillStats.body != 0) ? 1 : 0;
		skillStats.creativity = EditorGUILayout.Toggle("Creativity", skillStats.creativity != 0) ? 1 : 0;
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
