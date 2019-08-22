using MoonSharp.Interpreter;
using UnityEngine;

namespace Scripts {

public class ScriptManager {
    private static ScriptManager _instance;

    public static ScriptManager Instance => _instance ?? (_instance = new ScriptManager());

    public void Init() {
        //Script.DefaultOptions.ScriptLoader = new InvalidScriptLoader();
        Script.DefaultOptions.DebugPrint = s => Debug.Log("Lua: " + s);
    }

    public void RunScript(string content) {
        try {
            Script script = new Script();
            Debug.Log("> " + script.DoString(content));
        } catch (SyntaxErrorException e) {
            Debug.LogWarning("Syntax error: " + e.Message);
        } catch (ScriptRuntimeException e) {
            Debug.LogWarning("Lua error: " + e.Message);
        }
    }
}

}