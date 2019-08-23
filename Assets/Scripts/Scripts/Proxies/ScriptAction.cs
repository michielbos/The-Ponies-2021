using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Scripts.Proxies {

[MoonSharpUserData]
public class ScriptAction {
    public readonly string name;
    public readonly Closure function;

    [MoonSharpVisible(false)]
    public ScriptAction(string name, Closure function) {
        this.name = name;
        this.function = function;
    }

    [MoonSharpVisible(false)]
    public static ScriptAction Create(string name, Closure function) {
        return new ScriptAction(name, function);
    }
}

}