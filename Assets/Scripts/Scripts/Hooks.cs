using System.Collections.Generic;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Scripts.Proxies;
using UnityEngine;
using Util;

namespace Scripts {

[MoonSharpUserData]
public class Hooks {
    private readonly List<Closure> onTickFunctions = new List<Closure>();
    private readonly Dictionary<string, ScriptAction> objectActions = new Dictionary<string, ScriptAction>();
    private readonly Dictionary<string, ScriptAction> ponyActions = new Dictionary<string, ScriptAction>();
    private readonly Dictionary<string, ScriptAction> tileActions = new Dictionary<string, ScriptAction>();

    [MoonSharpVisible(false)]
    public void OnTickCallback() {
        onTickFunctions.ForEach(function => {
            try {
                function.Call();
            } catch (ScriptRuntimeException e) {
                HookError("onTick", e);
            }
        });
    }
    
    [MoonSharpVisible(false)]
    public List<PonyAction> RequestObjectActions(Pony pony, PropertyObject targetObject) {
        return RequestActions(objectActions, pony, targetObject, "object action");
    }
    
    [MoonSharpVisible(false)]
    public List<PonyAction> RequestPonyActions(Pony pony, Pony targetPony) {
        return RequestActions(ponyActions, pony, targetPony, "pony action");
    }
    
    [MoonSharpVisible(false)]
    public List<PonyAction> RequestTileActions(Pony pony, TerrainTile targetTile) {
        return RequestActions(tileActions, pony, targetTile, "tile action");
    }

    private List<PonyAction> RequestActions(Dictionary<string, ScriptAction> scriptActions, Pony pony, IActionProvider target, string hookName) {
        List<PonyAction> actions = new List<PonyAction>();
        foreach (KeyValuePair<string, ScriptAction> actionPair in scriptActions) {
            try {
                string identifier = actionPair.Key;
                ScriptAction scriptAction = actionPair.Value;
                DynValue conditionResult = scriptAction.conditionFunction.Call(pony, target);
                if (conditionResult.Type == DataType.Boolean && conditionResult.Boolean)
                    actions.Add(new ScriptPonyAction(scriptAction.name, identifier, pony, target));
            } catch (ScriptRuntimeException e) {
                HookError(hookName, e);
            }
        }
        return actions;
    }

    private void HookError(string hookName, ScriptRuntimeException exception) {
        Debug.LogWarning("Lua " + hookName + " error: " + exception.DecoratedMessage);
    }
    
    [MoonSharpVisible(false)]
    public ScriptAction GetScriptAction(string identifier) {
        return objectActions.Get(identifier) ?? ponyActions.Get(identifier) ?? tileActions.Get(identifier);
    }

    public void onTick(Closure function) {
        if (function != null)
            onTickFunctions.Add(function);
    }

    public void registerObjectAction(string identifier, ScriptAction scriptAction) {
        if (scriptAction != null)
            objectActions.Add(identifier, scriptAction);
    }
    
    public void registerPonyAction(string identifier, ScriptAction scriptAction) {
        if (scriptAction != null)
            ponyActions.Add(identifier, scriptAction);
    }
    
    public void registerTileAction(string identifier, ScriptAction scriptAction) {
        if (scriptAction != null)
            tileActions.Add(identifier, scriptAction);
    }
}

}