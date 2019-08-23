using System;
using Controllers.Playmode;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;
using Scripts.Proxies;
using UnityEngine;

namespace Scripts {

public class ScriptManager {
    private static ScriptManager _instance;
    public readonly Hooks hooks = new Hooks();

    public static ScriptManager Instance => _instance ?? (_instance = new ScriptManager());

    public void Init() {
        // TODO: Configure correct scriptloader and test security.
        //Script.DefaultOptions.ScriptLoader = new InvalidScriptLoader();
        Script.DefaultOptions.DebugPrint = s => Debug.Log("Lua: " + s);
        UserData.RegisterAssembly();
        RegisterProxies();
    }

    public void OnPropertyLoaded() {
        hooks.RegisterHookCallbacks();
    }

    private void RegisterProxies() {
        UserData.RegisterProxyType<PropertyProxy, Property>(v => new PropertyProxy(v));
        UserData.RegisterProxyType<HouseholdProxy, Household>(v => new HouseholdProxy(v));
        UserData.RegisterProxyType<PonyProxy, Pony>(v => new PonyProxy(v));
        UserData.RegisterProxyType<PropertyObjectProxy, PropertyObject>(v => new PropertyObjectProxy(v));
        UserData.RegisterProxyType<ScriptPonyActionProxy, ScriptPonyAction>(v => new ScriptPonyActionProxy(v));
    }

    public void RunScript(string content) {
        try {
            Script script = new Script();
            AddGlobals(script);
            Debug.Log("> " + script.DoString(content));
        } catch (SyntaxErrorException e) {
            Debug.LogWarning("Syntax error: " + e.Message);
        } catch (ScriptRuntimeException e) {
            Debug.LogWarning("Lua error: " + e.Message);
        }
    }

    private void AddGlobals(Script script) {
        // Global variables
        script.Globals["property"] = PropertyController.Instance.property;
        script.Globals["household"] = HouseholdController.Instance.Household;
        script.Globals["hooks"] = hooks;
        
        // Constructor functions
        script.Globals["Vector2"] = (Func<float, float, Vector2Wrapper>) Vector2Wrapper.Create;
        script.Globals["Action"] = (Func<string, Closure, ScriptAction>) ScriptAction.Create;
    }
}

}