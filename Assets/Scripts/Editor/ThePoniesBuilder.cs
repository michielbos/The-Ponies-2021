using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ThePoniesBuilder {
    
    [MenuItem("Build/Build all")]
    public static void RunBuild() {
        BuildLinux();
        BuildWindows64();
    }
    
    [MenuItem("Build/Build releases")]
    public static void RunReleaseBuild() {
        BuildLinux(false);
        BuildWindows64(false);
    }

    [MenuItem("Build/Build Linux64")]
    private static void BuildLinux(bool development = true) {
        Build("Linux64", BuildTarget.StandaloneLinux64, development);
    }

    [MenuItem("Build/Build Windows64")]
    private static void BuildWindows64(bool development = true) {
        Build("Windows64", BuildTarget.StandaloneWindows64, development);
    }

    private static void Build(string name, BuildTarget buildTarget, bool development) {
        Debug.Log("Starting " + name + " build...");
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.scenes = new[] {"Assets/_Scenes/PropertyScene.unity", "Assets/_Scenes/GameSceneTest.unity"};
        options.target = buildTarget;
        if (buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64) {
            // Windows is a special snowflake.
            options.locationPathName = "Build/" + name + "/The Ponies/The Ponies.exe";
        } else {
            options.locationPathName = "Build/" + name + "/The Ponies/The Ponies";
        }
        options.options = development ? BuildOptions.Development : BuildOptions.None;
        BuildReport result = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = result.summary;
        Debug.Log(name + " build result: " + summary.result + " (took " + summary.totalTime + ")");
        if (summary.result != BuildResult.Succeeded) {
            throw new BuildException(name + " build was not successful. Result: " + summary.result + " with " +
                                     summary.totalErrors + " errors and " + summary.totalWarnings + " warnings.");
        }
        PostBuild(name);
    }

    private static void PostBuild(string name) {
        Directory.CreateDirectory("Build/" + name + "/The Ponies/The Ponies_Data/Content");
        Directory.CreateDirectory("Build/" + name + "/The Ponies/Mods");
        Debug.Log("Post-build actions done.");
    }
}