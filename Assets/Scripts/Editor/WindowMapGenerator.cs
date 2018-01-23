using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Editor
{
	public class WindowMapGenerator
	{
		public const string GeneratorDirectory = "Generated/WindowCutouts";
		public const string GeneratorFormat = "cutout_{0}.png";

		private static GameObject _plane;
		private static GameObject _camObj;

		public static void GenerateWindowMaps(Dictionary<Guid, GameObject> toArray)
		{
			Camera cam = SetupRenderScene();

			int w = 128;
			int h = w * 3;
			RenderTexture rt = RenderTexture.GetTemporary(w, h, 16, RenderTextureFormat.ARGB32);
			RenderTexture.active = rt;
			cam.targetTexture = rt;

			foreach (KeyValuePair<Guid, GameObject> go in toArray)
			{
				GameObject obj = CreateWindowObject(go.Value, Vector3.zero);
				if (obj == null) continue;

				Debug.LogFormat("Rendering Window: {0}", go.Key);
				cam.Render();

				Texture2D tex = new Texture2D(rt.width, rt.height);
				tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
				tex.Apply();

				string path = Path.Combine(Application.dataPath, GeneratorDirectory);
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				path = Path.Combine(path, string.Format(GeneratorFormat, go.Key.ToString()));
				File.WriteAllBytes(path, tex.EncodeToPNG());

				Object.DestroyImmediate(obj);
			}

			RenderTexture.active = null;

			AssetDatabase.Refresh();
			Object.DestroyImmediate(_plane);
			Object.DestroyImmediate(_camObj);
			//	EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
		}

		private static GameObject CreateWindowObject(Object prefab, Vector3 pos)
		{
			GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			if (obj == null)
				return null;

			obj.transform.position = pos + Vector3.forward * 5;
			obj.transform.localScale = Vector3.one * 3;
			obj.layer = LayerMask.NameToLayer("EditorRender");
			Renderer r = obj.GetComponent<Renderer>();
			Material mat = new Material(Shader.Find("Unlit/Color"));
			mat.SetColor("_Color", Color.black);
			Material[] mats = r.sharedMaterials;
			for (int i = 0; i < mats.Length; i++)
				mats[i] = mat;
			r.sharedMaterials = mats;
			return obj;
		}

		private static Camera SetupRenderScene()
		{
			//	EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
			Material mat = new Material(Shader.Find("Unlit/Color"));
			mat.SetColor("_Color", Color.white);

			_plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			_plane.transform.position = Vector3.forward * 5;
			_plane.transform.rotation = Quaternion.Euler(-90, 0, 0);
			_plane.transform.localScale = new Vector3(1 / 3f, 1, 1);
			_plane.layer = LayerMask.NameToLayer("EditorRender");
			_plane.GetComponent<Renderer>().sharedMaterial = mat;


			_camObj = new GameObject("Camera");
			Camera cam = _camObj.AddComponent<Camera>();
			cam.cullingMask = LayerMask.GetMask("EditorRender");
			cam.orthographic = true;
			cam.orthographicSize = 5;
			return cam;
		}
	}
}
