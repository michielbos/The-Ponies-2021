using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Scripts.Proxies {

[MoonSharpUserData]
public class ScriptAction {
    public readonly string name;
    public readonly Closure conditionFunction;
    public readonly Closure actionFunction;

    private ScriptAction(string name, Closure conditionFunction, Closure actionFunction) {
        this.name = name;
        this.conditionFunction = conditionFunction;
        this.actionFunction = actionFunction;
    }

    [MoonSharpVisible(false)]
    public static ScriptAction Create(string name, Closure conditionFunction, Closure actionFunction) {
        return new ScriptAction(name, conditionFunction, actionFunction);
    }
}

}