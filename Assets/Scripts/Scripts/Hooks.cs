using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Controllers.Playmode;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Scripts.Proxies;

namespace Scripts {

[MoonSharpUserData]
public class Hooks : ITimeTickListener {
    private readonly List<Closure> onTickFunctions = new List<Closure>();
    private readonly List<Closure> onObjectActionFunctions = new List<Closure>();
    private readonly List<Closure> onPonyActionFunctions = new List<Closure>();
    private readonly List<Closure> onTileActionFunctions = new List<Closure>();

    [MoonSharpVisible(false)]
    public void RegisterHookCallbacks() {
        TimeController.Instance.AddTickListener(this);
    }

    [MoonSharpVisible(false)]
    public void OnTick() {
        onTickFunctions.ForEach(function => function.Call());
    }
    
    [MoonSharpVisible(false)]
    public List<PonyAction> RequestObjectActions(Pony pony, PropertyObject targetObject) {
        return RequestActions(onObjectActionFunctions, pony, targetObject);
    }
    
    [MoonSharpVisible(false)]
    public List<PonyAction> RequestPonyActions(Pony pony, Pony targetPony) {
        return RequestActions(onPonyActionFunctions, pony, targetPony);
    }
    
    [MoonSharpVisible(false)]
    public List<PonyAction> RequestTileActions(Pony pony, TerrainTile targetTile) {
        return RequestActions(onTileActionFunctions, pony, targetTile);
    }

    private List<PonyAction> RequestActions(List<Closure> functions, Pony pony, IActionProvider target) {
        List<PonyAction> actions = new List<PonyAction>();
        functions.ForEach(function => {
            DynValue result = function.Call(pony, target);
            if (result.Type == DataType.Table) {
                foreach (DynValue value in result.Table.Values) {
                    AddActionToList(actions, value, pony, target);
                }
            } else {
                AddActionToList(actions, result, pony, target);
            }
        });
        return actions;
    }

    private void AddActionToList(List<PonyAction> actions, DynValue value, Pony pony, IActionProvider propertyObject) {
        if (value.Type == DataType.UserData && value.UserData.Object is ScriptAction scriptAction) {
            actions.Add(new ScriptPonyAction(scriptAction.name, scriptAction.function, pony, propertyObject));
        }
    }

    public void onTick(Closure function) {
        if (function != null)
            onTickFunctions.Add(function);
    }

    public void onObjectActionMenu(Closure function) {
        if (function != null)
            onObjectActionFunctions.Add(function);
    }
    
    public void onPonyActionMenu(Closure function) {
        if (function != null)
            onPonyActionFunctions.Add(function);
    }
    
    public void onTileActionMenu(Closure function) {
        if (function != null)
            onTileActionFunctions.Add(function);
    }
}

}