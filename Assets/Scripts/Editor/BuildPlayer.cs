using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace RetroFootball76.Editor
{
  public static class BuildPlayer
  {
    public static void BuildMacOS()
    {
      SceneGenerator.GenerateAll();

      var scenes = EditorBuildSettings.scenes
        .Where(s => s.enabled)
        .Select(s => s.path)
        .ToArray();

      var opts = new BuildPlayerOptions
      {
        scenes = scenes,
        locationPathName = "build/RetroFootball76.app",
        target = BuildTarget.StandaloneOSX,
        options = BuildOptions.None
      };

      var report = BuildPipeline.BuildPlayer(opts);
      if (report.summary.result != BuildResult.Succeeded)
        throw new System.Exception($"Build failed: {report.summary.result}");
    }
  }
}
