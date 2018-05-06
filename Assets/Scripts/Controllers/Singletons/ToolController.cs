using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Util;
using UnityEngine;

/// <summary>
/// Controller for buy/build mode that controls the currently selected tool.
/// </summary>
public class ToolController : SingletonMonoBehaviour<ToolController>
{
    [Serializable]
    public class ToolMap
    {
        public ToolType type;
        // This is a ScriptableObject because otherwise we can't assign tools from the editor
        public ScriptableObject tool; 
    }

    public bool HasActiveTool { get { return ActiveTool != null; } }
    public ITool ActiveTool { get; set; }
    public ToolType ActiveToolType { get; set; }

    public ToolMap[] tools;

    /// <summary>
    /// Set the currently active tool.
    /// </summary>
    /// <param name="toolType">The tool type to activate.</param>
    public void SetTool(ToolType toolType)
    {
        bool isNewTool = toolType != ActiveToolType;
        if (isNewTool && HasActiveTool)
        {
            ActiveTool.Disable();
        }
        ActiveToolType = toolType;
        ActiveTool = toolType == ToolType.None ? null : GetTool(toolType);
        if (isNewTool && HasActiveTool)
        {
            // This here complains about a possible null reference exception, but if you look at the
            // implementation of HasActiveTool you can see that it is a null check for this offending
            // variable below. Don't know if this is ReSharper, but whatever. Fuck you.
            ActiveTool.Enable();
        }
    }

    /// <summary>
    /// Sets the selected tool to the type required for the given object category. 
    /// </summary>
    /// <param name="objectCategory">The ObjectCategory to match the tool with.</param>
    public void SetToolForCategory(ObjectCategory objectCategory)
    {
        if (ObjectCategoryUtil.IsFurnitureCategory(objectCategory))
        {
            SetTool(ToolType.Buy);
        }
        else if (objectCategory == ObjectCategory.Floors)
        {
            SetTool(ToolType.Floor);
        }
        else
        {
            // [Ebunix] Replaces DisableTool();
            SetTool(ToolType.None);
        }
    }

    /// <summary>
    /// Get the currently active tool, if there is one.
    /// </summary>
    /// <returns>The tool that is currently active. Null if there is none.</returns>
    public ITool GetTool(ToolType type)
    {
        ToolMap tool = tools.FirstOrDefault(t => t.type == type);
        return tool == null ? null : tool.tool as ITool;
    }

    private void Update()
    {
        if (HasActiveTool)
        {
            bool hit;
            Vector3 tilePosition = GetBuildMarkerPosition(out hit);
            ActiveTool.UpdateTool(tilePosition, Vector2Int.zero); // TODO Fix V2I 
        }
    }

    private Vector3 GetBuildMarkerPosition(out bool hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 1000, 1 << 8 /*Terrain layer*/) && !HUDController.Instance.IsMouseOverGui())
        {
            hit = true;
            float hitX = raycastHit.point.x;
            float hitY = raycastHit.point.z;
            hitX = Mathf.Floor(hitX);
            hitY = Mathf.Floor(hitY);
            return new Vector3(hitX, 0, hitY);
        }
        hit = false;
        return Vector3.zero;
    }

}
