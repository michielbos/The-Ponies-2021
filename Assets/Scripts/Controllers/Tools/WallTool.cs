using UnityEngine;

namespace Assets.Scripts.Controls.Tools {

public class WallTool : MonoBehaviour, ITool {
    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex) { }

    public void OnCatalogSelect(CatalogItem item, int skin) { }

    public void Enable() { }

    public void Disable() { }
}

}