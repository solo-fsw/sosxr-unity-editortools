using System.IO;
using SOSXR.EditorSpice.EditorScripts;
using UnityEditor.Build.Reporting;
using UnityEngine;


namespace SOSXR.BuildHelpers
{
    /// <summary>
    /// Stores persistent build information and configuration used by the BuildInfoManager.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildInfoDetails", menuName = "SOSXR/BuildInfoDetails")]
    public class BuildInfoDetails : ScriptableObject
    {
        /// <summary>
        /// The file path to store the build info CSV file.
        /// </summary>
        [Tooltip("The file path to store the build info CSV file")]
        [SerializeField] private string m_filePath = "Assets/_SOSXR/Resources/build_info.csv";

        /// <summary>
        /// The initial semantic version of the project, used when the current semver is invalid.
        /// </summary>
        [Tooltip("The initial semantic version of the project, set if the semVer is not similar to this format")]
        public string InitialSemVer = "0_0_1";

        /// <summary>
        /// Indicator appended to the semVer for development builds (e.g., 'd').
        /// </summary>
        [Tooltip("The indicator to append to the semVer for development builds")]
        public string DevelopmentBuildIndicator = "d";
        /// <summary>
        /// Indicator appended to the semVer for production builds (e.g., 'p').
        /// </summary>
        [Tooltip("The indicator to append to the semVer for production builds")]
        public string ProductionBuildIndicator = "p";

        /// <summary>
        /// SemVer value prior to the most recent change.
        /// </summary>
        [HideInInspector] public string OldSemVer;
        /// <summary>
        /// Android bundle version code prior to the most recent change.
        /// </summary>
        [HideInInspector] public int OldBundleVersionCode;

        [Header("Current Build")]
        /// <summary>
        /// Current semantic version string for the build (read-only in editor).
        /// </summary>
        [DisableEditing] public string SemVer;
        /// <summary>
        /// Android bundle version code for the current build (Android builds only).
        /// </summary>
        [Tooltip("Android only")]
        [DisableEditing] public int AndroidBundleVersionCode;

        [Header("Previous Builds")]
        /// <summary>
        /// Total number of attempted builds.
        /// </summary>
        [DisableEditing] public int TotalAttemptedBuilds;
        /// <summary>
        /// Total number of successful builds.
        /// </summary>
        [DisableEditing] public int TotalSuccessBuilds;
        /// <summary>
        /// Result of the last build processed by the manager.
        /// </summary>
        [DisableEditing] public BuildResult LastBuildResult = BuildResult.Unknown;

        /// <summary>
        /// Exposes the configured path for the build info CSV file.
        /// Creates the directory if it does not exist.
        /// </summary>
        public string FilePath
        {
            get
            {
                if (!Directory.Exists(Path.GetDirectoryName(m_filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(m_filePath) ?? string.Empty);
                }

                if (!File.Exists(m_filePath))
                {
                    Debug.LogWarningFormat("File does not exist: {0}. It should be created upon your first build.", m_filePath);
                }

                return m_filePath;
            }
        }


        [ContextMenu(nameof(IncreaseAttemptedBuilds))]
        private void IncreaseAttemptedBuilds()
        {
            TotalAttemptedBuilds++;
        }


        [ContextMenu(nameof(IncreaseSuccessfulBuilds))]
        private void IncreaseSuccessfulBuilds()
        {
            TotalSuccessBuilds++;
        }
    }
}
