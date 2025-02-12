#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;


namespace SOSXR.BuildHelpers
{
    /// <summary>
    ///     Handles automatic semantic versioning and build information updates
    /// </summary>
    public class BuildInfoManager : IPreprocessBuildWithReport
    {
        private static BuildInfoDetails _buildInfoDetails;
        public int callbackOrder => 0;


        public void OnPreprocessBuild(BuildReport report)
        {
            GetBuildInfoDetails();
            ChangeVersion(true);

            if (report.summary.platform == BuildTarget.Android)
            {
                ChangeAndroidVersion(true);
                Debug.Log("SemanticVersion: Incremented Android Bundle Version Code");
            }

            WriteBuildInfoToFile();
        }


        private static void GetBuildInfoDetails()
        {
            if (_buildInfoDetails != null)
            {
                return;
            }

            var path = GetAssetPath();

            if (path == null)
            {
                _buildInfoDetails = ScriptableObject.CreateInstance<BuildInfoDetails>();
                var assetPath = "Assets/_SOSXR/BuildInfoDetails.asset";
                AssetDatabase.CreateAsset(_buildInfoDetails, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"A config file has been created at {assetPath}. You can move this anywhere you'd like.");
            }
            else
            {
                _buildInfoDetails = AssetDatabase.LoadAssetAtPath<BuildInfoDetails>(path);
            }
        }


        private static string GetAssetPath()
        {
            var paths = AssetDatabase.FindAssets(nameof(BuildInfoDetails))
                                     .Select(AssetDatabase.GUIDToAssetPath)
                                     .Where(c => c.EndsWith(".asset"))
                                     .ToList();

            if (paths.Count > 1)
            {
                Debug.LogWarning("Multiple Build Info Detail assets found. Delete until you have only one.");
            }

            return paths.FirstOrDefault();
        }


        public static void ChangeVersion(bool increment)
        {
            _buildInfoDetails.OldSemVer = PlayerSettings.bundleVersion;
            _buildInfoDetails.NewSemVer = GenerateNextVersion(_buildInfoDetails.OldSemVer, increment);

            PlayerSettings.bundleVersion = _buildInfoDetails.NewSemVer;
            Debug.LogFormat("SemanticVersion: Updated SemVer from {0} to {1}", _buildInfoDetails.OldSemVer, _buildInfoDetails.NewSemVer);
        }


        private static string GenerateNextVersion(string version, bool increment)
        {
            var buildIndicator = EditorUserBuildSettings.development ? _buildInfoDetails.DevelopmentBuildIndicator : _buildInfoDetails.ProductionBuildIndicator;
            version = version.TrimEnd(Convert.ToChar(_buildInfoDetails.DevelopmentBuildIndicator), Convert.ToChar(_buildInfoDetails.ProductionBuildIndicator)); // Remove build indicator if present

            var parts = version.Split('_');

            if (parts.Length < 2 || !int.TryParse(parts[^1], out var currentNumber))
            {
                var initialVersion = $"{_buildInfoDetails.InitialSemVer}{buildIndicator}";

                Debug.LogWarning($"[SemanticVersion] Invalid version format \"{version}\". " +
                                 $"Expected format like '0_0_0{buildIndicator}' or 'alpha-1_32_2{buildIndicator}'. Resetting to initial version {initialVersion}");

                return initialVersion;
            }

            parts[^1] = (increment ? currentNumber + 1 : Math.Max(0, currentNumber - 1)).ToString();

            return $"{string.Join("_", parts)}{buildIndicator}";
        }


        public static void ChangeAndroidVersion(bool increment)
        {
            _buildInfoDetails.OldBundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            _buildInfoDetails.NewBundleVersionCode = increment ? _buildInfoDetails.OldBundleVersionCode + 1 : Math.Max(0, _buildInfoDetails.OldBundleVersionCode - 1);

            PlayerSettings.Android.bundleVersionCode = _buildInfoDetails.NewBundleVersionCode;
            Debug.LogFormat("SemanticVersion: Updated Android Bundle Version Code from {0} to {1}", _buildInfoDetails.OldBundleVersionCode, _buildInfoDetails.NewBundleVersionCode);
        }


        public static void WriteBuildInfoToFile()
        {
            var directoryPath = Path.GetDirectoryName(_buildInfoDetails.FilePath) ?? string.Empty;

            EnsureDirectoryExists(directoryPath);

            if (!File.Exists(_buildInfoDetails.FilePath))
            {
                WriteHeadersToFile();
            }

            AppendBuildInfoToFile();

            Debug.Log($"SemanticVersion: Appended build information to {_buildInfoDetails.FilePath}");
        }


        private static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }


        public static void WriteHeadersToFile()
        {
            using var sw = File.CreateText(_buildInfoDetails.FilePath);
            sw.WriteLine("PackageName, UnityVersion, ProductionBuild, SemVer, BundleCode, BuildDate, BuildTime");
        }


        private static void AppendBuildInfoToFile()
        {
            var buildInfo = new[]
            {
                PlayerSettings.applicationIdentifier,
                Application.unityVersion,
                (!EditorUserBuildSettings.development).ToString(),
                PlayerSettings.bundleVersion,
                PlayerSettings.Android.bundleVersionCode.ToString(),
                DateTime.Now.ToString("yyyy-MM-dd"),
                DateTime.Now.ToString("HH:mm")
            };

            using var sw = File.AppendText(_buildInfoDetails.FilePath);
            sw.WriteLine(string.Join(", ", buildInfo));
        }
    }
}
#endif