using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    [InitializeOnLoad]
    public class SetupSettings
    {
        public enum RepositoryType
        {
            Stable,
            Development
        }


        static SetupSettings()
        {
            ProjectSettingsProvider.OnGUIEvent += OnGUI;
        }


        public static string[] StableRepositories
        {
            get
            {
                var stored = EditorPrefs.GetString(_stableKey, "Default");

                return string.IsNullOrEmpty(stored)
                    ? new[]
                    {
                        "com.unity.ide.rider",
                        "com.unity.mobile.android-logcat",
                        "com.unity.nuget.newtonsoft-json",
                        "com.unity.cloud.gltfast",
                        "com.unity.modules.imageconversion",
                        "com.unity.memoryprofiler",
                        "/KyleBanks/scene-ref-attribute.git",
                        "/solo-fsw/sosxr-unity-enhancedlogger.git",
                        "/solo-fsw/sosxr-unity-plet.git",
                        "/solo-fsw/sosxr-unity-editorspice.git",
                        "/solo-fsw/sosxr-unity-scriptableobjectarchitecture.git",
                        "/solo-fsw/sosxr-unity-timelineextensions.git",
                        "/arimger/Unity-Editor-Toolbox.git#upm",
                        "/XCharts-Team/XCharts.git",
                        "com.unity.inputsystem"
                    }
                    : stored.Split(',');
            }
            set => EditorPrefs.SetString(_stableKey, string.Join(",", value));
        }

        public static string[] DevRepositories
        {
            get
            {
                var stored = EditorPrefs.GetString(_devKey, "Default");

                if (!string.IsNullOrEmpty(stored) && stored != "Default")
                {
                    return stored.Split(',');
                }

                return StableRepositories.Select(repo =>
                {
                    return repo.Contains("sosxr-unity", StringComparison.OrdinalIgnoreCase)
                        ? repo + "#dev"
                        : repo;
                }).ToArray();
            }
            set => EditorPrefs.SetString(_devKey, string.Join(",", value));
        }


        public static RepositoryType RepoType
        {
            get => (RepositoryType) EditorPrefs.GetInt(_typeKey, (int) RepositoryType.Stable);
            set => EditorPrefs.SetInt(_typeKey, (int) value);
        }

        private const string _stableKey = "SOSXR.EditorSpice.SetupSettings.StableRepositories";
        private const string _devKey = "SOSXR.EditorSpice.SetupSettings.DevRepositories";

        private const string _typeKey = "SOSXR.EditorSpice.SetupSettings.RepositoryType";


        private static void OnGUI()
        {
            // Space
            GUILayout.Space(10);
            // Draw a horizontal line
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            // Show header
            GUILayout.Label("Manage Repositories", EditorStyles.boldLabel);
            // Space
            GUILayout.Space(10);

            // Repository Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Repository Type:", GUILayout.Width(120));
            var newRepoType = (RepositoryType) EditorGUILayout.EnumPopup(RepoType, GUILayout.Width(200));

            if (newRepoType != RepoType)
            {
                RepoType = newRepoType;
            }

            GUILayout.EndHorizontal();

            var currentValue = RepoType == RepositoryType.Stable ? StableRepositories : DevRepositories;
            var modified = false;

            for (var i = 0; i < currentValue.Length; i++)
            {
                GUILayout.BeginHorizontal();
                var newValue = EditorGUILayout.TextField($"Repository {i + 1}", currentValue[i], GUILayout.Width(500));

                if (newValue != currentValue[i])
                {
                    currentValue[i] = newValue;
                    modified = true;
                }

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    var newList = new string[currentValue.Length - 1];

                    for (int j = 0, k = 0; j < currentValue.Length; j++)
                    {
                        if (j != i)
                        {
                            newList[k++] = currentValue[j];
                        }
                    }

                    currentValue = newList;
                    modified = true;
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Repository", GUILayout.Width(150)))
            {
                var newList = new string[currentValue.Length + 1];

                for (var j = 0; j < currentValue.Length; j++)
                {
                    newList[j] = currentValue[j];
                }

                newList[^1] = "New.Repository";
                currentValue = newList;
                modified = true;
            }


            if (modified)
            {
                if (RepoType == RepositoryType.Stable)
                {
                    StableRepositories = currentValue;
                }
                else
                {
                    DevRepositories = currentValue;
                }
            }

            if (GUILayout.Button("Install Repositories", GUILayout.Width(200)))
            {
                if (RepoType == RepositoryType.Stable)
                {
                    ProjectSetup.InstallStablePackages();
                }
                else
                {
                    ProjectSetup.InstallBetaPackages();
                }
            }
        }
    }
}