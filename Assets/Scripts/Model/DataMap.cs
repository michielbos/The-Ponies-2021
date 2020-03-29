using System.Collections.Generic;
using System.Linq;
using Model.Data;
using UnityEngine;
using Util;

namespace Model {

/// <summary>
/// Container for all key-value data pairs in loadable items (property objects, ponies, actions).
/// </summary>
public class DataMap {
    private readonly Dictionary<string, object> data = new Dictionary<string, object>();

    public void Put(string key, object value) {
        data[key] = value;
    }

    public int GetInt(string key, int defaultValue) => data.GetTyped(key, defaultValue);
    
    public float GetFloat(string key, float defaultValue) => data.GetTyped(key, defaultValue);
    
    public bool GetBool(string key, bool defaultValue) => data.GetTyped(key, defaultValue);
    
    public string GetString(string key, string defaultValue) => data.GetTyped(key, defaultValue);

    public Vector2Int GetVector2Int(string key, Vector2Int defaultValue) => data.GetTyped(key, defaultValue);
    
    public Vector2Int? GetVector2IntOrNull(string key) => data.GetTyped(key, (Vector2Int?) null);

    /// <summary>
    /// Generate an array of data pairs, which are used for saving the data to a file.
    /// </summary>
    public DataPair[] GetDataPairs() {
        return data.Select(pair => DataPair.FromValues(pair.Key, pair.Value)).ToArray();
    }
}

}