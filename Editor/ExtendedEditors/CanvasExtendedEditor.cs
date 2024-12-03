using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     Based on: Warped Imagination
    ///     https://www.youtube.com/watch?v=iqbUbtwiiz0
    /// </summary>
    [CustomEditor(typeof(Canvas))]
    public class CanvasExtendedEditor : EditorGUIHelpers
    {
        private static bool _setMainCamera = true;


        private void OnEnable()
        {
            GetInternalEditor("CanvasEditor");
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (Canvas) target;

            if (targetObject.renderMode != RenderMode.WorldSpace)
            {
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            CreateHeader("Warning: This only works when the MainCam is in the scene during development", LabelStyle);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _setMainCamera = GUILayout.Toggle(_setMainCamera, "Set MainCam as WorldCam", GUILayout.Width(ButtonWidth * 1.25f));

            if (_setMainCamera && targetObject.worldCamera == null)
            {
                var mainCamera = Camera.main;

                if (mainCamera != null)
                {
                    targetObject.worldCamera = mainCamera;
                }
                else
                {
                    Debug.LogWarning("No main camera found in the scene.");
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}