namespace SOSXR.EditorTools
{
    // No longer needed, as it is now superseded by the UnityEventWithButtonDrawer 
    /*

    /// <summary>
    ///     Based on: Warped Imagination
    ///     https://www.youtube.com/watch?v=iqbUbtwiiz0
    /// </summary>
    [CustomEditor(typeof(Button))]
    public class ButtonExtendedEditor : EditorGUIHelpers
    {
        private void OnEnable()
        {
            GetInternalEditor(typeof(ButtonEditor));
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (Button) target;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Click Button", GUILayout.Width(ButtonWidth)))
            {
                targetObject.onClick?.Invoke();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    */
}