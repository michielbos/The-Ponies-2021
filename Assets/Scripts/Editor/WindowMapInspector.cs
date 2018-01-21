using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
	[CustomEditor(typeof(WindowMap))]
	public class WindowMapInspector : UnityEditor.Editor
	{
		private static Texture2D valid;
		private static Texture2D invalid;
		private static Texture2D separator;

		public override void OnInspectorGUI()
		{
			if (valid == null) valid = Resources.Load<Texture2D>("editor-valid");
			if (invalid == null) invalid = Resources.Load<Texture2D>("editor-invalid");
			if (separator == null) separator = Resources.Load<Texture2D>("editor-separator");

			SerializedProperty e = null;
			SerializedProperty p = serializedObject.FindProperty("_mapEntries");
			for (int i = 0; i < p.arraySize; i++)
			{
				e = p.GetArrayElementAtIndex(i);
				if (e.FindPropertyRelative("_windowPrefab").objectReferenceValue == null && i < p.arraySize - 1)
					p.DeleteArrayElementAtIndex(i);
				else if (DrawEntry(e, i < p.arraySize - 1))
					p.DeleteArrayElementAtIndex(i);
			}
			if (p.arraySize > 0)
				e = p.GetArrayElementAtIndex(p.arraySize - 1).FindPropertyRelative("_windowPrefab");
			if (e == null || e.objectReferenceValue != null)
			{
				p.InsertArrayElementAtIndex(p.arraySize);
				e = p.GetArrayElementAtIndex(p.arraySize - 1);
				ClearElement(e);
				DrawEntry(e, false);
			}

			Dictionary<Guid, GameObject> objs = new Dictionary<Guid, GameObject>();
			if (GUILayout.Button("Generate Cutout Textures"))
			{
				for (int i = 0; i < p.arraySize; i++)
				{
					e = p.GetArrayElementAtIndex(i);
					string guid = e.FindPropertyRelative("_windowId").stringValue;
					if (!string.IsNullOrEmpty(guid))
						objs.Add(new Guid(guid), e.FindPropertyRelative("_windowPrefab").objectReferenceValue as GameObject);
				}
			}

			if (objs.Count > 0)
			{
				WindowMapGenerator.GenerateWindowMaps(objs);
				foreach (var pair in objs)
				{
					string path = string.Format(Path.Combine(WindowMapGenerator.GeneratorDirectory, WindowMapGenerator.GeneratorFormat),
						pair.Key.ToString());
					path = Path.Combine("Assets", path);
					Texture2D img = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
					SetTexture(pair.Key, img, p);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void SetTexture(Guid key, Texture2D img, SerializedProperty root)
		{
			for (int i = 0; i < root.arraySize; i++)
			{
				var e = root.GetArrayElementAtIndex(i);
				string guid = e.FindPropertyRelative("_windowId").stringValue;
				if (!string.IsNullOrEmpty(guid) && new Guid(guid) == key)
					e.FindPropertyRelative("_windowCutoutTexture").objectReferenceValue = img;
			}
		}

		private void ClearElement(SerializedProperty prop)
		{
			prop.FindPropertyRelative("_windowPrefab").objectReferenceValue = null;
			prop.FindPropertyRelative("_windowCutoutTexture").objectReferenceValue = null;
			prop.FindPropertyRelative("_windowId").stringValue = null;
		}

		private bool DrawEntry(SerializedProperty prop, bool removable)
		{
			bool remove = false;
			SerializedProperty obj = prop.FindPropertyRelative("_windowPrefab");
			SerializedProperty texture = prop.FindPropertyRelative("_windowCutoutTexture");
			SerializedProperty guid = prop.FindPropertyRelative("_windowId");

			if (string.IsNullOrEmpty(guid.stringValue))
				guid.stringValue = Guid.NewGuid().ToString();

			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			GUILayout.Label(string.Format("GUID: {0}", guid.stringValue), EditorStyles.centeredGreyMiniLabel);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label(texture.objectReferenceValue == null ? invalid : valid, GUILayout.Width(18));
			obj.objectReferenceValue = EditorGUILayout.ObjectField(obj.objectReferenceValue, typeof(GameObject), false);
			if (removable && GUILayout.Button("-", GUILayout.Width(20))) remove = true;
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.EndVertical();

			return remove;
		}
	}
}