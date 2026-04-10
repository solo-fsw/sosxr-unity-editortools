using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    /// <summary>
    /// Custom inspector extension for Canvas that adds world-space canvas utilities.
    /// 
    /// Purpose:
    /// Exposes a small helper to automatically bind the Main Camera as the World Camera for world-space canvases during development.
    /// 
    /// Features:
    /// - Automatic main camera assignment for world-space canvases
    /// 
    /// How to Extend:
    /// To add this extension to a new component type:
    /// 1. Create a new class inheriting from EditorGUIHelpers
    /// 2. Add [CustomEditor(typeof(YourComponentType))] attribute
    /// 3. Override CustomInspectorContent() to add your UI
    /// 4. Use existing helper methods to implement editor behavior
    /// 
    /// Attribution:
    /// Based on Warped Imagination: https://www.youtube.com/watch?v=iqbUbtwiiz0
    /// </summary>
    [CustomEditor(typeof(Canvas))]
    public class CanvasExtendedEditor : EditorGUIHelpers
    {
        private static bool _setMainCamera = true;


        private void OnEnable()
        {
            // Delay initialization to avoid null reference during domain reload
            EditorApplication.delayCall += () =>
            {
                if (this == null || target == null)
                {
                    return;
                }

                GetInternalEditor("CanvasEditor");
            };
        }


        protected override void CustomInspectorContent()
        {
            // Null check to prevent errors during domain reload or object destruction
            if (target == null)
            {
                return;
            }

            var canvas = (Canvas) target;

            if (canvas == null || canvas.renderMode != RenderMode.WorldSpace)
            {
                return;
            }

            CreateHeader("Warning: This only works when the MainCam is in the scene during development", LabelStyle);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            _setMainCamera = GUILayout.Toggle(_setMainCamera, "Set MainCam as WorldCam", GUILayout.Width(ButtonWidth * 1.25f));

            if (_setMainCamera && canvas.worldCamera == null)
            {
                var mainCamera = Camera.main;

                if (mainCamera != null)
                {
                    canvas.worldCamera = mainCamera;
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
