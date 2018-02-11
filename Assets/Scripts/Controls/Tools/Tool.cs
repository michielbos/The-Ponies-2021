using UnityEngine;

/// <summary>
/// Base class for buy/build mode tools.
/// </summary>
public abstract class Tool : MonoBehaviour {
    /// <summary>
    /// Set the currently selected catalog item on this tool. 
    /// </summary>
    /// <param name="catalogItem">The catalog item that was selected.</param>
    /// <param name="skin">The selected skin. If none was selected, this is 0.</param>
    public abstract void SetSelectedPreset (CatalogItem catalogItem, int skin);
}