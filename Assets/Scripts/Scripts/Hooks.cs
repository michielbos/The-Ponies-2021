using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Controllers.Playmode;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Scripts {

[MoonSharpUserData]
public class Hooks : ITimeTickListener {
    private readonly List<Closure> onTickFunctions = new List<Closure>();

    [MoonSharpVisible(false)]
    public void RegisterHookCallbacks() {
        TimeController.Instance.AddTickListener(this);
    }
    
    [MoonSharpVisible(false)]
    public void OnTick() {
        onTickFunctions.ForEach(function => function.Call());
    }
    
    public void onTick(Closure function) {
        if (function != null)
            onTickFunctions.Add(function);
    }
}

}