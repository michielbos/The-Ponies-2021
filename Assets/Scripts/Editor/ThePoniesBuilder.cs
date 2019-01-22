using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ThePoniesBuilder {
    public static void RunBuild() {
        BuildLinux();
        BuildWindows64();
    }

    private static void BuildLinux() {
        Debug.Log("Starting Linux build...");
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = new[] {"Assets/_Scenes/PropertyScene.unity", "Assets/_Scenes/GameSceneTest.unity"};
        options.target = BuildTarget.StandaloneLinuxUniversal;
        options.locationPathName = "Build/Linux/The Ponies/The Ponies";
        BuildReport result = BuildPipeline.BuildPlayer(options);
        Debug.Log("Result: " + result.summary.result);
    }
    
    private static void BuildWindows64() {
        Debug.Log("Starting Windows64 build...");
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = new[] {"Assets/_Scenes/PropertyScene.unity", "Assets/_Scenes/GameSceneTest.unity"};
        options.target = BuildTarget.StandaloneLinuxUniversal;
        options.locationPathName = "Build/Windows64/The Ponies/The Ponies";
        BuildReport result = BuildPipeline.BuildPlayer(options);
        Debug.Log("Result: " + result.summary.result);
    }
}