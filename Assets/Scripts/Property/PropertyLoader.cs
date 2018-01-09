using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PropertyLoader : MonoBehaviour {
	public PropertyData toSave;
	public PropertyData loaded;
	public bool writeToFile;
	public bool readFromFile;

	void Update () {
		if (writeToFile) {
			writeToFile = false;
			WriteFile();
		}
		if (readFromFile) {
			readFromFile = false;
			ReadFile();
		}
	}
		
	void WriteFile() {
		string path = "Assets/Resources/test.txt";
		if (File.Exists(path))
			File.Delete(path);
		StreamWriter writer = new StreamWriter(path, true);
		//writer.WriteLine(JsonUtility.ToJson(toSave));
		string data = JsonUtility.ToJson(toSave);
		Debug.Log("Writing data: " + data);
		writer.WriteLine(data);
		writer.Close();
	}
		
	void ReadFile() {
		string path = "Assets/Resources/test.txt";
		StreamReader reader = new StreamReader(path);
		string data = reader.ReadToEnd();
		Debug.Log("Reading data: " + data);
		reader.Close();
		loaded = JsonUtility.FromJson<PropertyData>(data);
	}
}
