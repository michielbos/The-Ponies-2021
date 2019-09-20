using System;

namespace Util {

/// <summary>
/// Exception to indicate that the requested content could not be loaded.
/// </summary>
public class ContentLoaderException : Exception {
    public ContentLoaderException(string message) : base(message) { }
    public ContentLoaderException(string message, Exception innerException) : base(message, innerException) { }
}

}