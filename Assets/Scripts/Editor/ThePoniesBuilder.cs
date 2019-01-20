using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ThePoniesBuilder {
    public static void RunBuild() {
        Debug.Log("Starting build...");
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = new[] {"Assets/_Scenes/PropertyScene.unity", "Assets/_Scenes/GameSceneTest.unity"};
        options.target = BuildTarget.StandaloneLinuxUniversal;
        options.locationPathName = "Build/Linux/ThePonies";
        BuildReport result = BuildPipeline.BuildPlayer(options);
        Debug.Log("Result: " + result.summary.result);
    }
}