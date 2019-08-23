using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Controllers.Playmode;
using Model.Actions;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Scripts.Proxies;
using UnityEngine;

namespace Scripts {

[MoonSharpUserData]
public class Hooks : ITimeTickListener {
    private readonly List<Closure> onTickFunctions = new List<Closure>();
    private readonly List<Closure> onObjectActionFunctions = new List<Closure>();

    [MoonSharpVisible(false)]
    public void RegisterHookCallbacks() {
        TimeController.Instance.AddTickListener(this);
    }

    [MoonSharpVisible(false)]
    public void OnTick() {
        onTickFunctions.ForEach(function => function.Call());
    }
    
    [MoonSharpVisible(false)]
    public List<PonyAction> RequestObjectActions(Pony pony, PropertyObject propertyObject) {
        List<PonyAction> actions = new List<PonyAction>();
        onObjectActionFunctions.ForEach(function => {
            DynValue result = function.Call(pony, propertyObject);
            if (result.Type == DataType.Table) {
                foreach (DynValue value in result.Table.Values) {
                    AddActionToList(actions, value, pony, propertyObject);
                }
            } else {
                AddActionToList(actions, result, pony, propertyObject);
            }
        });
        return actions;
    }

    private void AddActionToList(List<PonyAction> actions, DynValue value, Pony pony, PropertyObject propertyObject) {
        if (value.Type == DataType.UserData && value.UserData.Object is ScriptAction) {
            ScriptAction scriptAction = (ScriptAction) value.UserData.Object;
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
}

}