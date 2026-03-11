#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SOSXR.BuildHelpers
{
    /// <summary>
    ///     Handles automatic semantic versioning and build information updates.
    ///     Only increments version numbers on successful builds.
    /// </summary>
    public class BuildInfoManager : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private static BuildInfoDetails _buildInfoDetails;

        /// <summary>
        /// Called after a build completes. Triggers post-build processing to record results and update build metadata.
        /// </summary>
        /// <param name="report"></param>
        public void OnPostprocessBuild(BuildReport report)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(PostProcessBuildCR());
        }

        public int callbackOrder => 0;

        /// <summary>
        /// Called before a build starts. Captures pre-build state and increments attempted builds.
        /// </summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            GetBuildInfoDetailsFile();

            _buildInfoDetails.TotalAttemptedBuilds++;

            UndoPreviousIncrementIfPreviousBuildFailed();

            IncrementVersionNumbers();

            _buildInfoDetails.LastBuildResult = BuildResult.Unknown; // Reset the result before the build starts

            WritePreBuildInfoToFile();
        }

        private static void GetBuildInfoDetailsFile()
        {
            if (_buildInfoDetails != null)
            {
                return;
            }

            var path = GetAssetPath();

            if (path == null)
            {
                _buildInfoDetails = ScriptableObject.CreateInstance<BuildInfoDetails>();
                var assetPath = "Assets/_SOSXR/Resources/BuildInfoDetails.asset";
                AssetDatabase.CreateAsset(_buildInfoDetails, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log(
                    $"A config file has been created at {assetPath}. You can move this anywhere you'd like."
                );
            }
            else
            {
                _buildInfoDetails = AssetDatabase.LoadAssetAtPath<BuildInfoDetails>(path);
            }
        }

        /// <summary>
        ///     If the previous build failed, we need to undo the incrementation that was done before the build started.
        ///     This ensures that the version numbers only increment on successful builds.
        ///     We check if the previous build succeeded by storing the latest BuildResult. Since we do this upon completion of the build process, we can use that to determine if the previous build was successful or not.
        ///     Because the OnPostprocessBuild is not called on __all__ failed builds, and not called on Cancelled builds at all, if the value is anything other than Succeeded, we assume the build failed.
        /// </summary>
        private static void UndoPreviousIncrementIfPreviousBuildFailed()
        {
            if (_buildInfoDetails.LastBuildResult == BuildResult.Succeeded)
            {
                return;
            }

            ChangeSemVer(false);

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                ChangeAndroidVersion(false);
            }
        }

        private static void IncrementVersionNumbers()
        {
            ChangeSemVer(true);

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                ChangeAndroidVersion(true);
            }
        }

        private static IEnumerator PostProcessBuildCR()
        {
            var seconds = 1;
            Debug.Log($"Waiting {seconds} seconds to ensure BuildReport is ready...");

            yield return new EditorWaitForSeconds(seconds);

            Debug.Log("Waited enough, fetching latest BuildReport...");

            WritePostBuildInfoToFile();
        }

        [MenuItem("SOSXR/Build Info/Print Build Size")]
        private static void TestGettingSize()
        {
            var buildReport = BuildReport.GetLatestReport();

            var fileInfo = new FileInfo(buildReport.summary.outputPath);
            var fileSizeMB = Math.Round(fileInfo.Length / (1000f * 1000f), 1);

            Debug.Log($"Build size: {fileSizeMB} MB");
        }

        /// <summary>
        /// Writes the post-build information to the CSV and updates the last build result.
        /// </summary>
        public static void WritePostBuildInfoToFile()
        {
            var buildReport = BuildReport.GetLatestReport();

            double fileSizeMB = -1;
            var fileInfo = new FileInfo(buildReport.summary.outputPath);

            if (fileInfo.Exists)
            {
                fileSizeMB = Math.Round(fileInfo.Length / (1000f * 1000f), 1);
            }

            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                _buildInfoDetails.TotalSuccessBuilds++;
            }

            var buildInfo = new[]
            {
                string.Concat(
                    ", ",
                    Math.Round(buildReport.summary.totalTime.TotalSeconds, 0).ToString()
                ),
                fileSizeMB.ToString(),
                _buildInfoDetails.TotalSuccessBuilds.ToString(),
                buildReport.summary.result.ToString(),
            };

            using var sw = File.AppendText(_buildInfoDetails.FilePath);

            sw.Write(string.Join(", ", buildInfo));

            _buildInfoDetails.LastBuildResult = buildReport.summary.result;
        }

        private static string GetAssetPath()
        {
            var paths = AssetDatabase
                .FindAssets(nameof(BuildInfoDetails))
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(c => c.EndsWith(".asset"))
                .ToList();

            if (paths.Count > 1)
            {
                Debug.LogWarning(
                    "Multiple Build Info Detail assets found. Delete until you have only one."
                );
            }

            return paths.FirstOrDefault();
        }

        /// <summary>
        /// Adjusts the semantic version string. If increment is true, increments the patch number; otherwise decrements.
        /// </summary>
        public static void ChangeSemVer(bool increment)
        {
            _buildInfoDetails.OldSemVer = PlayerSettings.bundleVersion;
            _buildInfoDetails.SemVer = GenerateNextVersion(_buildInfoDetails.OldSemVer, increment);

            PlayerSettings.bundleVersion = _buildInfoDetails.SemVer;
            Debug.LogFormat(
                "SemanticVersion: Updated SemVer from {0} to {1}",
                _buildInfoDetails.OldSemVer,
                _buildInfoDetails.SemVer
            );
        }

        /// <summary>
        /// Increments or decrements the patch segment of an underscore-separated semver string (e.g. `1_0_0`).
        /// Appends a build-type indicator suffix. Returns the initial semver from BuildInfoDetails if the format is invalid.
        /// </summary>
        private static string GenerateNextVersion(string version, bool increment)
        {
            var buildIndicator = EditorUserBuildSettings.development
                ? _buildInfoDetails.DevelopmentBuildIndicator
                : _buildInfoDetails.ProductionBuildIndicator;
            version = version.TrimEnd(
                Convert.ToChar(_buildInfoDetails.DevelopmentBuildIndicator),
                Convert.ToChar(_buildInfoDetails.ProductionBuildIndicator)
            ); // Remove build indicator if present

            var parts = version.Split('_');

            if (parts.Length < 2 || !int.TryParse(parts[^1], out var currentNumber))
            {
                var initialVersion = $"{_buildInfoDetails.InitialSemVer}{buildIndicator}";

                Debug.LogWarning(
                    $"[SemanticVersion] Invalid version format \"{version}\". "
                        + $"Expected format like '0_0_0{buildIndicator}' or 'alpha-1_32_2{buildIndicator}'. Resetting to initial version {initialVersion}"
                );

                return initialVersion;
            }

            parts[^1] = (increment ? currentNumber + 1 : Math.Max(0, currentNumber - 1)).ToString();

            return $"{string.Join("_", parts)}{buildIndicator}";
        }

        /// <summary>
        /// Adjusts the Android bundle version code. Increments or decrements based on the flag.
        /// </summary>
        public static void ChangeAndroidVersion(bool increment)
        {
            _buildInfoDetails.OldBundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            _buildInfoDetails.AndroidBundleVersionCode = increment
                ? _buildInfoDetails.OldBundleVersionCode + 1
                : Math.Max(0, _buildInfoDetails.OldBundleVersionCode - 1);

            PlayerSettings.Android.bundleVersionCode = _buildInfoDetails.AndroidBundleVersionCode;
            Debug.LogFormat(
                "SemanticVersion: Updated Android Bundle Version Code from {0} to {1}",
                _buildInfoDetails.OldBundleVersionCode,
                _buildInfoDetails.AndroidBundleVersionCode
            );
        }

        public static void WritePreBuildInfoToFile()
        {
            var directoryPath = Path.GetDirectoryName(_buildInfoDetails.FilePath) ?? string.Empty;

            EnsureDirectoryExists(directoryPath);

            if (!File.Exists(_buildInfoDetails.FilePath))
            {
                WriteHeadersToFile();
            }

            using (var sw = File.AppendText(_buildInfoDetails.FilePath))
            {
                sw.WriteLine();
            }

            AppendBuildInfoToFile();

            Debug.Log(
                $"SemanticVersion: Appended build information to {_buildInfoDetails.FilePath}"
            );
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Writes CSV headers to the build info file if needed.
        /// </summary>
        public static void WriteHeadersToFile()
        {
            using var sw = File.CreateText(_buildInfoDetails.FilePath);
            sw.WriteLine(
                "PackageName, UnityVersion, TotalAttemptedBuilds, BuildTarget, ScriptingBackend, APICompatibility, ProductionBuild, SemVer, AndroidBundleCode, BuildDate, BuildTime, BuildDurationSec, BuildSizeMB, TotalSuccessBuilds, Result"
            );
        }

        private static void AppendBuildInfoToFile()
        {
            var buildInfo = new[]
            {
                PlayerSettings.applicationIdentifier,
                Application.unityVersion,
                _buildInfoDetails.TotalAttemptedBuilds.ToString(),
                EditorUserBuildSettings.activeBuildTarget.ToString(),
                PlayerSettings
                    .GetScriptingBackend(
                        NamedBuildTarget.FromBuildTargetGroup(
                            EditorUserBuildSettings.selectedBuildTargetGroup
                        )
                    )
                    .ToString(),
                PlayerSettings
                    .GetApiCompatibilityLevel(
                        NamedBuildTarget.FromBuildTargetGroup(
                            EditorUserBuildSettings.selectedBuildTargetGroup
                        )
                    )
                    .ToString(),
                (!EditorUserBuildSettings.development).ToString(),
                PlayerSettings.bundleVersion,
                PlayerSettings.Android.bundleVersionCode.ToString(),
                DateTime.Now.ToString("yyyy-MM-dd"),
                DateTime.Now.ToString("HH:mm"),
            };

            using var sw = File.AppendText(_buildInfoDetails.FilePath);
            sw.Write(string.Join(", ", buildInfo));
        }
    }
}
#endif
