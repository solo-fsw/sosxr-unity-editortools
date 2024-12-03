using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.EditorTools
{
    public class UnityEventButtonDrawerBase : UnityEventDrawer
    {
        protected const float ButtonWidthOffset = 68;
        protected static readonly float ButtonHeight = EditorGUIUtility.singleLineHeight;
        protected static readonly float ButtonSpacing = EditorGUIUtility.standardVerticalSpacing;


        // Handles drawing the button and the input field for any UnityEvent type.
        protected void DrawButtonWithField<T>(Rect position, SerializedProperty property, GUIContent label, string buttonText, T defaultValue, Action<T> invokeAction, Func<T> getValue, Action<T> setValue)
        {
            var unityEventHeight = GetPropertyHeight(property, label);
            var key = property.propertyPath + "_InvokeValue"; // Use property path for unique key

            // Get the current value, with a default fallback if not set
            var invokeValue = getValue.Invoke();

            // Determine the width of the button and the field
            var buttonWidth = position.width / 2 >= ButtonWidthOffset ? position.width / 2 : position.width - ButtonWidthOffset;
            var buttonRect = new Rect(position.x, position.y + unityEventHeight + ButtonSpacing - 22, buttonWidth, ButtonHeight);

            // Draw the button
            if (GUI.Button(buttonRect, buttonText))
            {
                var targetObject = property.serializedObject.targetObject;
                var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<T>;
                unityEvent?.Invoke(invokeValue); // Invoke the UnityEvent with the current value
            }

            // Draw the input field next to the button
            var fieldWidth = position.width / 2 >= ButtonWidthOffset ? position.width / 2 - ButtonWidthOffset - 1 : 0;
            var fieldRect = new Rect(position.x + buttonRect.width + 1, position.y + unityEventHeight + ButtonSpacing - 22, fieldWidth, ButtonHeight);

            // Handle the type-specific input field (int, string, float, bool)
            invokeValue = DrawInputField(fieldRect, invokeValue);

            setValue.Invoke(invokeValue); // Save the updated value
        }


        // Helper method to draw different types of input fields
        private T DrawInputField<T>(Rect fieldRect, T currentValue)
        {
            if (typeof(T) == typeof(int))
            {
                return (T) (object) EditorGUI.IntField(fieldRect, (int) (object) currentValue);
            }

            if (typeof(T) == typeof(string))
            {
                return (T) (object) EditorGUI.TextField(fieldRect, (string) (object) currentValue);
            }

            if (typeof(T) == typeof(float))
            {
                return (T) (object) EditorGUI.FloatField(fieldRect, (float) (object) currentValue);
            }

            if (typeof(T) == typeof(bool))
            {
                return (T) (object) EditorGUI.Toggle(fieldRect, (bool) (object) currentValue);
            }

            return currentValue;
        }
    }
}