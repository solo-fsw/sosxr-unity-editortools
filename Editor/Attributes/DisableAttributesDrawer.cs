using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    /// <summary>
    ///     Drawer for DisableEditingAttribute.
    /// </summary>
    /// <remarks>
    /// From: https://gist.github.com/LotteMakesStuff/c0a3b404524be57574ffa5f8270268ea
    /// With additions from ChatGPT and Claude.
    /// Limitations: Does not disable '+' button for lists.
    /// </remarks>
    [CustomPropertyDrawer(typeof(DisableEditingAttribute))]
    public class DisableEditingDrawer : PropertyDrawer
    {
        /// <summary>
        /// Renders the property field in a disabled state so it cannot be edited.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }


        /// <summary>
        /// Returns the height for the disabled property field.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
