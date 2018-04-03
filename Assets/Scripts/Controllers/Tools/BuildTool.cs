using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts.Controls
{
    [CreateAssetMenu(fileName = "BuildTool", menuName = "Tools/Build Tool", order = 10)]
    public class BuildTool : ScriptableObject, ITool
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
