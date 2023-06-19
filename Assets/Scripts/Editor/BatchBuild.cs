// Copyright 2019 Google LLC
// All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace AgonesExample.Editor
{
    public class BatchBuild
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
                locationPathName = dir + "/UnityServer.x86_64",
                target = BuildTarget.StandaloneLinux64,
                subtarget = (int) StandaloneBuildSubtarget.Server,
                options = BuildOptions.EnableHeadlessMode
            };
            BuildPipeline.BuildPlayer(option);
        }

        [MenuItem("Build Tool/Build Client")]
        public static void BuildClient()
        {
            string[] scenes = GetIncludedScenePaths();
            string dir = "Builds/Client";

            Directory.CreateDirectory(dir);

            var target = BuildTarget.StandaloneWindows64;
#if UNITY_EDITOR_OSX
            target = BuildTarget.StandaloneOSX;
#elif !UNITY_EDITOR_WIN && !UNITY_EDITOR_OSX
            target = BuildTarget.StandaloneLinux64;
#endif
            BuildPlayerOptions option = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = dir + "/test_agones.exe",
                target = target,
                options = BuildOptions.None
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
}

