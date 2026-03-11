using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace Tarodev
{
    /// <summary>
    /// Editor utility that wires up the AutoSaveConfig and drives interval-based saving
    /// of open scenes in the Unity Editor.
    /// </summary>
    // Original descriptive header preserved in the class summary above
    [CustomEditor(typeof(AutoSaveConfig))]
    public class TarodevAutoSave : Editor
    {
        private static AutoSaveConfig _config;
        private static CancellationTokenSource _tokenSource;
        private static Task _task;


        // [InitializeOnLoadMethod]
        private static void OnInitialize()
        {
            FetchConfig();
            CancelTask();

            _tokenSource = new CancellationTokenSource();
            _task = SaveInterval(_tokenSource.Token);
        }


        private static void FetchConfig()
        {
            while (true)
            {
                if (_config != null)
                {
                    return;
                }

                var path = GetConfigPath();

                if (path == null)
                {
                    AssetDatabase.CreateAsset(CreateInstance<AutoSaveConfig>(), $"Assets/{nameof(AutoSaveConfig)}.asset");
                    Debug.Log("A config file has been created at the root of your project.<b> You can move this anywhere you'd like.</b>");

                    continue;
                }

                _config = AssetDatabase.LoadAssetAtPath<AutoSaveConfig>(path);

                break;
            }
        }


        private static string GetConfigPath()
        {
            var paths = AssetDatabase.FindAssets(nameof(AutoSaveConfig)).Select(AssetDatabase.GUIDToAssetPath).Where(c => c.EndsWith(".asset")).ToList();

            if (paths.Count > 1)
            {
                Debug.LogWarning("Multiple auto save config assets found. Delete until you have only one.");
            }

            return paths.FirstOrDefault();
        }


        private static void CancelTask()
        {
            if (_task == null)
            {
                return;
            }

            _tokenSource.Cancel();
            _task.Wait();
        }


        private static async Task SaveInterval(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(_config.Frequency * 1000 * 60, token);

                if (_config == null)
                {
                    FetchConfig();
                }

                if (!_config.Enabled || Application.isPlaying || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling)
                {
                    continue;
                }

                if (!InternalEditorUtility.isApplicationActive)
                {
                    continue;
                }

                EditorSceneManager.SaveOpenScenes();

                if (_config.Logging)
                {
                    Debug.Log($"Auto-saved at {DateTime.Now:h:mm:ss tt}");
                }
            }
        }


        [MenuItem("SOSXR/AutoSave/Find config")]
        public static void ShowConfig()
        {
            FetchConfig();

            var path = GetConfigPath();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<AutoSaveConfig>(path).GetInstanceID());
        }


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("You can move this asset where ever you'd like.\nWith ❤, Tarodev.", MessageType.Info);
        }
    }
}
