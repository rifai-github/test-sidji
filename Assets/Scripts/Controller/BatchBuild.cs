﻿#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;

internal static class BatchBuild
{
    [MenuItem("Build Tool/Build Server")]
    public static void BuildServer()
    {
        string[] scenes = GetIncludedScenePaths();
        string dir = "buildserver";

        Directory.CreateDirectory(dir);

        BuildPlayerOptions option = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = dir + "/UnityServer",
            target = BuildTarget.StandaloneLinux64,
            subtarget = (int) StandaloneBuildSubtarget.Server,
            extraScriptingDefines = new string[] {"UNITY_SERVER"},
            options = BuildOptions.EnableHeadlessMode,
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