using System;
using System.Collections;
using System.Reflection;
using PoneCrafter;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers {

/// <summary>
/// Controller for the load screen that is responsible for activating the load functions of other components and
/// continuing to the property scene when done loading.
/// </summary>
public class GameLoader : MonoBehaviour {
    private IEnumerator Start() {
        yield return LoadGame();
    }

    /// <summary>
    /// Coroutine that loads all game content while the game displays the loading screen.
    /// </summary>
    private IEnumerator LoadGame() {
        LoadBehaviourAssembly();
        yield return null;
        yield return PoneCrafterImporter.Instance.Import();
        yield return ContentController.Instance.LoadContent();
        MusicController.Instance.LoadAllMusic();

        SceneManager.LoadScene("PropertyScene");
    }

    private void LoadBehaviourAssembly() {
#if UNITY_EDITOR
        string assemblyPath = Application.dataPath + "/../Library/ScriptAssemblies/ThePoniesBehaviour.dll";
#else
		string assemblyPath = Application.dataPath + "/Managed/ThePoniesBehaviour.dll";
#endif
        Assembly assembly = Assembly.LoadFile(assemblyPath);
        Type ponyActions = assembly.GetType("ThePoniesBehaviour.BehaviourManager");
        ponyActions.InvokeMember("LoadBehaviour", BindingFlags.InvokeMethod, null, null, new object[0]);
    }
}

}