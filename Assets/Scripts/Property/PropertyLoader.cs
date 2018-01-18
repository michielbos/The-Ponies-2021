using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PropertyLoader {
	const string propertyPrefix = "Property";
	const string dataPath = "Assets/Resources/SaveData/";
	const string propertyPath = dataPath + "Property/";

	public PropertyLoader () {
		TryCreateDirectory(propertyPath);
	}

	public bool PropertyExists (int id) {
		return File.Exists(propertyPath + propertyPrefix + id);
	}

	public void SaveProperty (Property property) {
		SavePropertyData(property.GetPropertyData());
	}

	public Property LoadOrCreateProperty (int id) {
		if (PropertyExists(id))
			return LoadProperty(id);
		else
			return new Property(id, "untitled", "", "untitled street " + id, 0);
	}

	public Property LoadProperty (int id) {
		return new Property(LoadPropertyData(id));
	}

	void SavePropertyData (PropertyData propertyData) {
		WriteFile(propertyPath + propertyPrefix + propertyData.id, JsonUtility.ToJson(propertyData));
	}

	PropertyData LoadPropertyData (int id) {
		return JsonUtility.FromJson<PropertyData>(ReadFile(propertyPath + propertyPrefix + id));
	}
		
	void WriteFile (string path, string data) {
		if (File.Exists(path))
			File.Delete(path);
		StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine(data);
		writer.Close();
	}
		
	string ReadFile (string path) {
		StreamReader reader = new StreamReader(path);
		string data = reader.ReadToEnd();
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
