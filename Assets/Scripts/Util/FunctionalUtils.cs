using System;

namespace Util {

/// <summary>
/// Basic functional tricks that are missing in C#.
/// </summary>
public static class FunctionalUtils {
    /// <summary>
    /// Perform the given operation on an object and return the result.
    /// </summary>
    public static R Let<T, R>(this T item, Func<T, R> op) {
        return op(item);
    }
    
    /// <summary>
    /// Perform the given operation on an object and return the object itself.
    /// </summary>
    public static T Also<T>(this T item, Action<T> op) {
        op(item);
        return item;
    }
}

}