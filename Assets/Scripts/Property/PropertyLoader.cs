using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PropertyLoader : MonoBehaviour {
	public PropertyData toSave;
	public PropertyData loaded;
	public bool writeToFile;
	public bool readFromFile;
	const string dataPath = "Assets/Resources/SaveData/";
	const string propertyPath = dataPath + "Property/";

	void Start () {
		TryCreateDirectory(propertyPath);
	}

	void Update () {
		if (writeToFile) {
			writeToFile = false;
			SaveProperty(toSave);
		}
		if (readFromFile) {
			readFromFile = false;
			loaded = LoadProperty(toSave.id);
		}
	}

	void SaveProperty (PropertyData propertyData) {
		WriteFile(propertyPath + "Property" + toSave.id, JsonUtility.ToJson(toSave));
	}

	PropertyData LoadProperty (int id) {
		return JsonUtility.FromJson<PropertyData>(ReadFile(propertyPath + "Property" + id));
	}
		
	void WriteFile (string path, string data) {
		if (File.Exists(path))
			File.Delete(path);
		StreamWriter writer = new StreamWriter(path, true);
		Debug.Log("Writing data: " + data);
		writer.WriteLine(data);
		writer.Close();
	}
		
	string ReadFile (string path) {
		StreamReader reader = new StreamReader(path);
		string data = reader.ReadToEnd();
		Debug.Log("Reading data: " + data);
		reader.Close();
		return data;
	}

	void TryCreateDirectory (string path) {
		if (!Directory.Exists(path)) {
			Debug.Log("Created directory: " + path);
			Directory.CreateDirectory(path);
		}
	}
}
