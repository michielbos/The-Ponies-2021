using Controllers.Playmode;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;
using Scripts.Proxies;
using UnityEngine;

namespace Scripts {

public class ScriptManager {
    private static ScriptManager _instance;

    public static ScriptManager Instance => _instance ?? (_instance = new ScriptManager());

    public void Init() {
        //Script.DefaultOptions.ScriptLoader = new InvalidScriptLoader();
        Script.DefaultOptions.DebugPrint = s => Debug.Log("Lua: " + s);
        RegisterProxies();
    }

    private void RegisterProxies() {
        UserData.RegisterProxyType<PropertyProxy, Property>(v => new PropertyProxy(v));
        UserData.RegisterProxyType<HouseholdProxy, Household>(v => new HouseholdProxy(v));
        UserData.RegisterProxyType<PonyProxy, Pony>(v => new PonyProxy(v));
        UserData.RegisterProxyType<PropertyObjectProxy, PropertyObject>(v => new PropertyObjectProxy(v));
    }

    public void RunScript(string content) {
        try {
            Script script = new Script();
            AddGlobalVariables(script);
            Debug.Log("> " + script.DoString(content));
        } catch (SyntaxErrorException e) {
            Debug.LogWarning("Syntax error: " + e.Message);
        } catch (ScriptRuntimeException e) {
            Debug.LogWarning("Lua error: " + e.Message);
        }
    }

    private void AddGlobalVariables(Script script) {
        script.Globals["property"] = PropertyController.Instance.property;
        script.Globals["household"] = HouseholdController.Instance.Household;
    }
}

}