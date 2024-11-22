using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     Based on: Warped Imagination
    ///     https://www.youtube.com/watch?v=iqbUbtwiiz0
    /// </summary>
    [CustomEditor(typeof(Button))]
    public class ButtonExtendedEditor : Editor
    {
        private ButtonEditor buttonEditor;


        private void OnEnable()
        {
            buttonEditor = (ButtonEditor) CreateEditor(target, typeof(ButtonEditor)); // Unity's ButtonEditor
        }


        public override void OnInspectorGUI()
        {
            buttonEditor.OnInspectorGUI(); // Draw Unity's default ButtonEditor

            AddHeader();

            AddButton();
        }


        private static void AddHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("SOSXR", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        private void AddButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Click", GUILayout.Width(100f)))
            {
                var button = (Button) target;

                button.onClick?.Invoke();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        private void OnDisable()
        {
            if (buttonEditor == null)
            {
                return;
            }

            DestroyImmediate(buttonEditor);
        }
    }
}