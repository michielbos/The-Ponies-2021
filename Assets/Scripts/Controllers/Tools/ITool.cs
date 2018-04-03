using UnityEngine;
using UnityEngine.Experimental.UIElements;

/// <summary>
/// Base class for buy/build mode tools.
/// </summary>
public interface ITool
{
    void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex);
    void OnCatalogSelect(CatalogItem item, int skin);

    void Enable();
    void Disable();
    void OnClicked(MouseButton button);
}
