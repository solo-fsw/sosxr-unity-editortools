using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     Based on: Warped Imagination
    ///     https://www.youtube.com/watch?v=iqbUbtwiiz0
    /// </summary>
    [CustomEditor(typeof(PlayableDirector))]
    public class PlayableDirectorExtendedEditor : EditorGUIHelpers
    {
        private bool _isPaused;
        private Color _originalColor;
        private bool _isPlaying;


        private void OnEnable()
        {
            _originalColor = GUI.backgroundColor;
            GetInternalEditor("DirectorEditor");
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (PlayableDirector) target;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (DrawPlayButton(targetObject))
            {
                return;
            }

            if (DrawPauseButton(targetObject))
            {
                return;
            }

            if (DrawStopButton(targetObject))
            {
                return;
            }

            // Restore the original GUI color
            GUI.backgroundColor = _originalColor;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        private bool DrawPlayButton(PlayableDirector editorTarget)
        {
            GUI.backgroundColor = _isPlaying && !_isPaused ? Color.green : _originalColor;

            if (GUILayout.Button("Play", GUILayout.Width(ButtonWidth)))
            {
                if (editorTarget.playableAsset == null)
                {
                    Debug.LogWarning("No playable asset assigned to the PlayableDirector.");

                    return true;
                }

                editorTarget.Play();
                 
                _isPaused = false;
                _isPlaying = true;
            }

            return false;
        }


        private bool DrawPauseButton(PlayableDirector editorTarget)
        {
            GUI.backgroundColor = _isPaused && !_isPlaying ? Color.yellow : _originalColor;

            if (GUILayout.Button("Pause", GUILayout.Width(ButtonWidth)))
            {
                editorTarget.Pause();
                
                _isPaused = true;
                _isPlaying = false;
            }

            return false;
        }


        private bool DrawStopButton(PlayableDirector editorTarget)
        {
            GUI.backgroundColor = !_isPlaying && !_isPaused ? _originalColor : Color.red;

            if (GUILayout.Button("Stop", GUILayout.Width(ButtonWidth)))
            {
                editorTarget.Stop();
                
                _isPaused = false;
                _isPlaying = false;
            }

            return false;
        }
    }
}