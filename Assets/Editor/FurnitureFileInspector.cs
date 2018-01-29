﻿using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds an editor inspector on .furniture items.
/// Useful for reading and updating furniture files.
/// </summary>
[CustomEditor(typeof(DefaultAsset))]
public class FurnitureFileInspector : Editor {
	private string path;
	private DefaultAsset furnitureFile;
	private FurniturePresetData presetData;

	private void OnEnable () {
		furnitureFile = (DefaultAsset)target;
		path = AssetDatabase.GetAssetPath(furnitureFile);
		if (path.ToLower().EndsWith(".furniture")) {
			presetData = GetFurniturePresetData();
		}
	}

	public override void OnInspectorGUI() {
		if (!path.ToLower().EndsWith(".furniture"))
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
		presetData.modelName = EditorGUILayout.TextField("Model name", presetData.modelName);
		EditorGUILayout.LabelField("Material paths");
		presetData.materialPaths = ArrayGuiField(presetData.materialPaths, StringGuiField);
		presetData.rotationOffset = EditorGUILayout.Vector3Field("Rotation offset", presetData.rotationOffset);
		presetData.positionOffset = EditorGUILayout.Vector3Field("Position offset", presetData.positionOffset);
		EditorGUILayout.LabelField("Occupied tiles");
		presetData.occupiedTiles = ArrayGuiField(presetData.occupiedTiles, Vector2IntGuiField);
		if (GUILayout.Button("Apply changes")) {
			SetContent(presetData);
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

	private string StringGuiField (int i, string str) {
		return EditorGUILayout.TextField("material " + i, str);
	}
	
	private Vector2Int Vector2IntGuiField (int i, Vector2Int vector2Int) {
		return EditorGUILayout.Vector2IntField("tile " + i, vector2Int);
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

	private void SetContent (FurniturePresetData furniturePresetData) {
		SetContent(JsonUtility.ToJson(furniturePresetData));
	}
}
