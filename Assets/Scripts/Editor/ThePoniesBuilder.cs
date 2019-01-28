using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class ThePoniesBuilder {
    
    [MenuItem("Build/Build all")]
    public static void RunBuild() {
        BuildLinux();
        BuildWindows64();
    }

    [MenuItem("Build/Build Linux")]
    private static void BuildLinux() {
        Build("Linux", BuildTarget.StandaloneLinuxUniversal);
    }

    [MenuItem("Build/Build Windows64")]
    private static void BuildWindows64() {
        Build("Windows64", BuildTarget.StandaloneWindows64);
    }

    private static void Build(string name, BuildTarget buildTarget) {
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
        BuildReport result = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = result.summary;
        Debug.Log(name + " build result: " + summary.result + " (took " + summary.totalTime + ")");
        if (summary.result != BuildResult.Succeeded) {
            throw new BuildException(name + " build was not successful. Result: " + summary.result + " with " +
                                     summary.totalErrors + " errors and " + summary.totalWarnings + " warnings.");
        }
    }
}