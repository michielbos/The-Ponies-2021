using UnityEngine;
using UnityEngine.Experimental.UIElements;

/// <summary>
/// Base class for buy/build mode tools.
/// </summary>
public interface ITool
{
    /// <summary>
    /// Called on each update tick while the tool is enabled.
    /// It also provides tile positions when the mouse is over a terrain tile and not over the GUI.
    /// </summary>
    /// <param name="tilePosition">The real world position where the mouse touches the terrain. Vector3.zero if none is touched.</param>
    /// <param name="tileIndex">The tile position where the mouse touches the terrain. (-1, -1) if none is touched.</param>
    void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex);
    void OnCatalogSelect(CatalogItem item, int skin);

    void Enable();
    void Disable();
    void OnClicked(MouseButton button);
}
