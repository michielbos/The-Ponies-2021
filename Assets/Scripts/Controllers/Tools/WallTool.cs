using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts.Controls.Tools
{
    [CreateAssetMenu(fileName = "WallTool", menuName = "Tools/Wall Tool", order = 10)]
	public class WallTool : ScriptableObject, ITool
	{
	    public void UpdateTool(Vector3 tilePosition, Vector2Int tileIndex)
	    {
	    }

	    public void OnCatalogSelect(CatalogItem item, int skin)
	    {
	    }

	    public void Enable()
	    {
	    }

	    public void Disable()
	    {
	    }

	    public void OnClicked(MouseButton button)
	    {
	    }
	}
}