using System.Threading;
using System.Threading.Tasks;
using SOSXR.EnhancedLogger;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace Tarodev
{
    [InitializeOnLoad]
    public class TarodevAutoSave
    {
        static TarodevAutoSave()
        {
            AssemblyReloadEvents.beforeAssemblyReload += CancelTask;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.update += EnsureRunning;
        }


        private static CancellationTokenSource _tokenSource;
        private static Task _task;


        private static void EnsureRunning()
        {
            EditorApplication.update -= EnsureRunning;

            if (_tokenSource == null || _tokenSource.IsCancellationRequested)
            {
                _tokenSource = new CancellationTokenSource();
                _task = SaveInterval(_tokenSource.Token);
            }
        }


        private static void CancelTask()
        {
            _tokenSource?.Cancel();
            _tokenSource = null;
            _task = null;
        }


        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                CancelTask();
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EnsureRunning();
            }
        }


        private static async Task SaveInterval(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(AutoSaveSettings.Frequency * 60_000, token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                if (!AutoSaveSettings.Enabled
                    || Application.isPlaying
                    || BuildPipeline.isBuildingPlayer
                    || EditorApplication.isCompiling
                    || !InternalEditorUtility.isApplicationActive)
                {
                    continue;
                }

                SaveOpenScenes();
            }
        }


        private static void SaveOpenScenes()
        {
            EditorSceneManager.SaveOpenScenes();
            Log.Static("Auto-saved the open scene(s)", LogLevel.Verbose);
        }
    }
}