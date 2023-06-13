#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;

internal static class BatchBuild
{
    [MenuItem("Build Tool/Build Server")]
    public static void BuildServer()
    {
        string[] scenes = GetIncludedScenePaths();
        string dir = "Builds/Server";

        Directory.CreateDirectory(dir);

        BuildPlayerOptions option = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = dir + "/UnitySimpleServer.x86_64",
            target = BuildTarget.StandaloneLinux64,
            subtarget = (int) StandaloneBuildSubtarget.Server,
            options = BuildOptions.EnableHeadlessMode
        };
        BuildPipeline.BuildPlayer(option);
    }

    private static string[] GetIncludedScenePaths()
    {
        List<string> scenePaths = new List<string>();
        var scenes = EditorBuildSettings.scenes;

        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                scenePaths.Add(scene.path);
            }
        }

        return scenePaths.ToArray();
    }
}
#endif