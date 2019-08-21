using Assets.Scripts.Util;
using Controllers.Playmode;
using Controllers.Tools;
using JetBrains.Annotations;
using Model.Property;
using UnityEngine;

namespace Controllers.Singletons {

/// <summary>
/// Controller for buy/build mode that controls the currently selected tool.
/// </summary>
public class ToolController : SingletonMonoBehaviour<ToolController> {
    private const int LAYER_TERRAIN = 8;

    public BuyTool buyTool;
    public FloorTool floorTool;
    public WallTool wallTool;
    public WallCoverTool wallCoverTool;

    private ITool tool;
    private ToolType activeToolType;

    public bool HasActiveTool => ActiveTool != null;

    public ITool ActiveTool { get; private set; }

    /// <summary>
    /// Set the currently active tool.
    /// </summary>
    /// <param name="toolType">The tool type to activate.</param>
    public void SetTool(ToolType toolType) {
        bool isNewTool = toolType != activeToolType;
        if (isNewTool && HasActiveTool) {
            ActiveTool.Disable();
        }

        activeToolType = toolType;
        ActiveTool = GetToolForType(toolType);
        if (isNewTool && HasActiveTool) {
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
    public void SetToolForCategory(ObjectCategory objectCategory) {
        if (ObjectCategoryUtil.IsFurnitureCategory(objectCategory)) {
            SetTool(ToolType.Buy);
        } else if (objectCategory == ObjectCategory.Floors) {
            SetTool(ToolType.Floor);
        } else if (objectCategory == ObjectCategory.Wall) {
            SetTool(ToolType.Wall);
        } else if (objectCategory == ObjectCategory.WallCover) {
            SetTool(ToolType.WallCover);
        } else {
            // [Ebunix] Replaces DisableTool();
            SetTool(ToolType.None);
        }
    }

    [CanBeNull]
    private ITool GetToolForType(ToolType type) {
        switch (type) {
            case ToolType.Floor:
                return floorTool;
            case ToolType.Buy:
                return buyTool;
            case ToolType.Wall:
                return wallTool;
            case ToolType.WallCover:
                return wallCoverTool;
        }

        return null;
    }

    private void Update() {
        if (!HasActiveTool) {
            return;
        }

        bool hit;
        RaycastHit hitInfo = GetMouseTerrainHit(out hit);
        Vector3 tilePosition = hit ? hitInfo.point : Vector3.zero;
        Vector2Int tileIndex = hit ? GetTileIndex(hitInfo) : new Vector2Int(-1, -1);
        ActiveTool.UpdateTool(tilePosition, tileIndex);
    }

    private RaycastHit GetMouseTerrainHit(out bool hit) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        hit = Physics.Raycast(ray, out raycastHit, 1000, 1 << LAYER_TERRAIN) &&
              !HUDController.GetInstance().IsMouseOverGui();
        return raycastHit;
    }

    private Vector2Int GetTileIndex(RaycastHit hitInfo) {
        TerrainTile terrainTile = hitInfo.collider.GetComponent<TerrainTile>();
        return terrainTile.TilePosition;
    }
}

}