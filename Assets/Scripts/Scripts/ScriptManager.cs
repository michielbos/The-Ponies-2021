using System;
using System.IO;
using Controllers.Playmode;
using Model.Ponies;
using Model.Property;
using MoonSharp.Interpreter;
using Scripts.Proxies;
using UnityEngine;
using Util;

namespace Scripts {

public class ScriptManager {
    private static ScriptManager _instance;
    public Hooks hooks;

    public static ScriptManager Instance => _instance ?? (_instance = new ScriptManager());

    public void Init() {
        // TODO: Configure correct scriptloader and test security.
        Script.DefaultOptions.ScriptLoader = new ContentScriptLoader();
        Script.DefaultOptions.DebugPrint = s => Debug.Log("Lua: " + s);
        UserData.RegisterAssembly();
        RegisterProxies();
    }

    public void OnPropertyLoaded() {
        ReloadAllScripts();
    }
    
    private void RegisterProxies() {
        UserData.RegisterProxyType<PropertyProxy, Property>(v => new PropertyProxy(v));
        UserData.RegisterProxyType<HouseholdProxy, Household>(v => new HouseholdProxy(v));
        UserData.RegisterProxyType<PonyProxy, Pony>(v => new PonyProxy(v));
        UserData.RegisterProxyType<PropertyObjectProxy, PropertyObject>(v => new PropertyObjectProxy(v));
        UserData.RegisterProxyType<TerrainTileProxy, TerrainTile>(v => new TerrainTileProxy(v));
        UserData.RegisterProxyType<ScriptPonyActionProxy, ScriptPonyAction>(v => new ScriptPonyActionProxy(v));
        UserData.RegisterProxyType<FurniturePresetProxy, FurniturePreset>(v => new FurniturePresetProxy(v));
    }

    public void ReloadAllScripts() {
        hooks = new Hooks();
        LoadScripts(ContentLoader.ContentFolder + "Scripts/");
        LoadScripts(ContentLoader.ModsFolder + "Scripts/");
    }
    
    private void LoadScripts(string folderPath) {
        if (!Directory.Exists(folderPath)) {
            Directory.CreateDirectory(folderPath);
        }
        foreach (string file in Directory.GetFiles(folderPath)) {
            if (!file.ToLower().EndsWith(".lua"))
                continue;
            string content = File.ReadAllText(file);
            try {
                RunScript(content);
            } catch (SyntaxErrorException e) {
                Debug.LogWarning("Syntax error in " + file + ": " + e.DecoratedMessage);
            } catch (ScriptRuntimeException e) {
                Debug.LogWarning("Lua error in " + file + ": " + e.DecoratedMessage);
            } catch (IOException e) {
                Debug.LogWarning("IO error in " + file + ": " + e.Message);
            }
        }
    }

    private DynValue RunScript(string content) {
        Script script = new Script();
        AddGlobals(script);
        return script.DoString(content);
    }

    public void RunConsoleScript(string content) {
        try {
            Debug.Log("> " + RunScript(content));
        } catch (SyntaxErrorException e) {
            Debug.LogWarning("Syntax error: " + e.DecoratedMessage);
        } catch (ScriptRuntimeException e) {
            Debug.LogWarning("Lua error: " + e.DecoratedMessage);
        } catch (IOException e) {
            Debug.LogWarning("IO error: " + e.Message);
        }
    }

    private void AddGlobals(Script script) {
        // Global variables
        script.Globals["property"] = PropertyController.Instance.property;
        script.Globals["household"] = HouseholdController.Instance.Household;
        script.Globals["hooks"] = hooks;
        
        // Constructor functions
        script.Globals["Vector2"] = (Func<float, float, Vector2Wrapper>) Vector2Wrapper.Create;
        script.Globals["Action"] = (Func<string, Closure, Closure, ScriptAction>) ScriptAction.Create;
    }
}

}