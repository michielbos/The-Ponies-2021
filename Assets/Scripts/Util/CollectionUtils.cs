using System.Collections.Generic;

namespace Util {

public static class CollectionUtils {
    /// <summary>
    /// Get the value that the given key is mapped to, or default value if the given key does not exist.
    /// </summary>
    public static TV Get<TK, TV>(this Dictionary<TK, TV> dictionary, TK key, TV defaultValue = default) {
        return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
    }
    
    /// <summary>
    /// Get the value that the given key is mapped to.
    /// If the value does not exist, or is not of the desired type, the default value will be returned.
    /// </summary>
    public static TV GetTyped<TK, TV>(this Dictionary<TK, object> dictionary, TK key, TV defaultValue = default) {
        if (dictionary.ContainsKey(key)) {
            return dictionary[key] is TV typedValue ? typedValue : defaultValue;
        }
        return defaultValue;
    }
}

}