using System.Collections.Generic;

namespace Util {

public static class CollectionUtils {
    /// <summary>
    /// Get the value that the given key is mapped to, or default value if the given key does not exist.
    /// </summary>
    public static TV Get<TK, TV>(this IDictionary<TK, TV> dictionary, TK key) {
        return dictionary.ContainsKey(key) ? dictionary[key] : default;
    }
}

}