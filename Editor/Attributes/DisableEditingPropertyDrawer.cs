using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools.Attributes
{
    /// <summary>
    ///     From: https://gist.github.com/LotteMakesStuff/c0a3b404524be57574ffa5f8270268ea
    ///     With additions from ChatGPT
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableEditingAttribute))]
    public class DisableEditingPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var wasEnabled = GUI.enabled;
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalHeight = EditorGUI.GetPropertyHeight(property, label, true);

            if (property.isArray && property.isExpanded)
            {
                for (var i = 0; i < property.arraySize; i++)
                {
                    var element = property.GetArrayElementAtIndex(i);
                    totalHeight += EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            return totalHeight;
        }
    }
}