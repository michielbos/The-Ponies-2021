using System.IO;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

namespace Scripts {

/// <summary>
/// Modified version of FileSystemScriptLoader.
/// </summary>
public class ContentScriptLoader : ScriptLoaderBase {
    public override bool ScriptFileExists(string name) {
        return File.Exists(GetPath(name));
    }

    public override object LoadFile(string file, Table globalContext) {
        return new FileStream(GetPath(file), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    private string GetPath(string name) {
        // Note that we currently don't prevent scripts from loading files outside the mod/content directories.
        if (name.StartsWith("mods/")) {
            return Application.dataPath + "../Mods/Scripts/" + name.Substring(5);
        }
        return Application.dataPath + "/Content/Scripts/" + name;
    }
}

}