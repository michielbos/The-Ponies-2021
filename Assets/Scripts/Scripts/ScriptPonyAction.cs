using System;
using Model.Actions;
using Model.Data;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Scripts.Proxies {

public class ScriptPonyAction : PonyAction {
    private readonly string identifier;
    public readonly IActionProvider target;
    
    public ScriptPonyAction(string name, string identifier, Pony pony, IActionProvider target) : base(pony, name) {
        this.identifier = identifier;
        this.target = target;
    }

    public ScriptPonyAction(string name, string identifier, Pony pony, IActionProvider target, int tickCount,
        bool canceled) : base(pony, name, tickCount, canceled) {
        this.identifier = identifier;
        this.target = target;
    }

    public static ScriptPonyAction FromData(Pony pony, PonyActionData data) {
        // TODO: Fix load order to load action names. Properties and ScriptManager have a two-way dependency. 
        // ScriptAction scriptAction = ScriptManager.Instance.hooks.GetScriptAction(data.identifier);
        string name = data.identifier;
        return new ScriptPonyAction(name, data.identifier, pony, GetTargetFromData(data), data.tickCount, data.canceled);
    }
    
    private static IActionProvider GetTargetFromData(PonyActionData data) {
        switch (data.GetTargetType()) {
            case PonyActionData.TargetType.Object:
                return PropertyController.Instance.property.GetPropertyObject(data.targetObjectid);
            case PonyActionData.TargetType.Pony:
                return PropertyController.Instance.property.GetPony(new Guid(data.targetPonyUuid));
            case PonyActionData.TargetType.Tile:
                return PropertyController.Instance.property.GetTerrainTile(data.targetTileX, data.targetTileY);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Tick() {
        try {
            // Fetching the action each tick is less performant than storing it, but this will help us detect save bugs.
            ScriptAction scriptAction = ScriptManager.Instance.hooks.GetScriptAction(identifier);
            if (scriptAction == null) {
                Debug.LogWarning("No registered script action for identifier '" + identifier + "'.");
                Finish();
                return;
            }
            DynValue result = scriptAction.actionFunction.Call(this);
            if (result.Type != DataType.Boolean || result.Boolean)
                Finish();
        } catch (ScriptRuntimeException e) {
            Debug.LogWarning("Lua error on action: " + e.DecoratedMessage);
            Finish();
        }
    }

    public override PonyActionData GetData() {
        if (target is PropertyObject propertyObject)
            return new PonyActionData(identifier, tickCount, canceled, propertyObject.id);
        if (target is Pony pony)
            return new PonyActionData(identifier, tickCount, canceled, pony.uuid.ToString());
        if (target is TerrainTile terrainTile)
            return new PonyActionData(identifier, tickCount, canceled, terrainTile.TilePosition.x,
                terrainTile.TilePosition.y);
        throw new Exception("Invalid target type?");
    }
}

}