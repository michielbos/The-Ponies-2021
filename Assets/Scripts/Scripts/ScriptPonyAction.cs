using Model.Actions;
using Model.Ponies;
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
}

}