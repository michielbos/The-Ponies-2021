using Model.Ponies;
using MoonSharp.Interpreter.Interop;

namespace Scripts.Proxies {

public class ScriptPonyActionProxy {
    private readonly ScriptPonyAction scriptPonyAction;

    [MoonSharpVisible(false)]
    public ScriptPonyActionProxy(ScriptPonyAction scriptPonyAction) {
        this.scriptPonyAction = scriptPonyAction;
    }

    public Pony pony => scriptPonyAction.pony;
    
    public object target => scriptPonyAction.target;

    public int tickCount => scriptPonyAction.tickCount;

    public bool canceled => scriptPonyAction.canceled;

    public void cancel() => scriptPonyAction.Cancel();

}

}