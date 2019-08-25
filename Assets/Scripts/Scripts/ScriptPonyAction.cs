using Model.Actions;
using Model.Ponies;
using MoonSharp.Interpreter;
using UnityEngine;

namespace Scripts.Proxies {

public class ScriptPonyAction : PonyAction {
    private readonly Closure function;
    public readonly IActionProvider target;
    
    public ScriptPonyAction(string name, Closure function, Pony pony, IActionProvider target) : base(pony, name) {
        this.function = function;
        this.target = target;
    }
    public override void Tick() {
        try {
            DynValue result = function.Call(this);
            if (result.Type != DataType.Boolean || result.Boolean)
                Finish();
        } catch (ScriptRuntimeException e) {
            Debug.LogWarning("Lua error on action: " + e.Message);
        }
    }
}

}