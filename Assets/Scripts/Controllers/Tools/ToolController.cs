using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controller for buy/build mode that controls the currently selected tool.
/// </summary>
public class ToolController : MonoBehaviour {

	[Serializable]
	public class ToolMap
	{
		public ToolType type;
		public Tool tool;
	}

	public ToolMap[] tools;
	private ToolType currentTool;

	/// <summary>
	/// Set the currently active tool.
	/// </summary>
	/// <param name="toolType">The tool type to activate.</param>
	public void SetTool (ToolType toolType) {
		DisableTool();
		currentTool = toolType;
		if (HasActiveTool()) {
			Tool t = GetTool();
			if (t != null)
				t.enabled = true;
		}
	}

	/// <summary>
	/// Sets the selected tool to the type required for the given object category. 
	/// </summary>
	/// <param name="objectCategory">The ObjectCategory to match the tool with.</param>
	public void SetToolForCategory (ObjectCategory objectCategory) {
		if (ObjectCategoryUtil.IsFurnitureCategory(objectCategory)) {
			SetTool(ToolType.Buy);
		} else if (objectCategory == ObjectCategory.Floors) {
			SetTool(ToolType.Floor);
		} else {
			DisableTool();
		}
	}

	/// <summary>
	/// Get the currently active tool, if there is one.
	/// </summary>
	/// <returns>The tool that is currently active. Null if there is none.</returns>
	public Tool GetTool () {
		ToolMap tool = tools.FirstOrDefault(t => t.type == currentTool);
		return tool == null ? null : tool.tool;
	}

	/// <summary>
	/// Disable the currently active tool, if there is one.
	/// </summary>
	public void DisableTool () {
		if (currentTool == ToolType.None)
			return;
		Tool t = GetTool();
		if (t != null)
			t.enabled = false;
		currentTool = ToolType.None;
	}

	/// <summary>
	/// Check if there is an active tool.
	/// </summary>
	/// <returns>Returns true if there is an active tool, false otherwise.</returns>
	public bool HasActiveTool () {
		return currentTool != ToolType.None;
	}
}
