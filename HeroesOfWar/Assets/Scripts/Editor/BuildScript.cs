using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    private static string GetBuildPath(string platform)
    {
        string buildPath = Path.Combine(Application.dataPath, "..", "build", platform);
        Directory.CreateDirectory(buildPath);
        return buildPath;
    }

    [MenuItem("Build/Android/APK")]
    public static void BuildAndroid()
    {
        string buildPath = GetBuildPath("android");
        string apkPath = Path.Combine(buildPath, "HeroesOfWar.apk");
        
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = apkPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Android build succeeded! Built to: {apkPath}");
        }
        else
        {
            Debug.LogError($"Android build failed! Error: {summary.totalErrors} errors");
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/Android/AAB")]
    public static void BuildAndroidAAB()
    {
        string buildPath = GetBuildPath("android");
        string aabPath = Path.Combine(buildPath, "HeroesOfWar.aab");
        
        PlayerSettings.Android.buildAppBundle = true;
        
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = aabPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Android AAB build succeeded! Built to: {aabPath}");
        }
        else
        {
            Debug.LogError($"Android AAB build failed! Error: {summary.totalErrors} errors");
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/iOS")]
    public static void BuildiOS()
    {
        string buildPath = GetBuildPath("ios");
        
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = buildPath,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"iOS build succeeded! Built to: {buildPath}");
        }
        else
        {
            Debug.LogError($"iOS build failed! Error: {summary.totalErrors} errors");
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/Windows")]
    public static void BuildWindows()
    {
        string buildPath = GetBuildPath("windows");
        string exePath = Path.Combine(buildPath, "HeroesOfWar.exe");
        
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Windows build succeeded! Built to: {exePath}");
        }
        else
        {
            Debug.LogError($"Windows build failed! Error: {summary.totalErrors} errors");
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/macOS")]
    public static void BuildMacOS()
    {
        string buildPath = GetBuildPath("macos");
        string appPath = Path.Combine(buildPath, "HeroesOfWar.app");
        
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = appPath,
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"macOS build succeeded! Built to: {appPath}");
        }
        else
        {
            Debug.LogError($"macOS build failed! Error: {summary.totalErrors} errors");
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Build/WebGL")]
    public static void BuildWebGL()
    {
        string buildPath = GetBuildPath("webgl");
        
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = buildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"WebGL build succeeded! Built to: {buildPath}");
        }
        else
        {
            Debug.LogError($"WebGL build failed! Error: {summary.totalErrors} errors");
            EditorApplication.Exit(1);
        }
    }

    private static string[] GetScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }
}
