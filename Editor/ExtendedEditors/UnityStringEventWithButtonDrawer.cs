using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.EditorTools
{
    [CustomPropertyDrawer(typeof(UnityEvent<string>), true)]
    public class UnityStringEventWithButtonDrawer : UnityEventButtonDrawerBase
    {
        private const string DefaultValue = "Hello, World!";


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            DrawButtonWithField(position, property, label, $"SOSXR: {property.displayName}", DefaultValue,
                invokeValue =>
                {
                    var targetObject = property.serializedObject.targetObject;
                    var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<string>;
                    unityEvent?.Invoke(invokeValue);
                },
                () => EditorPrefs.GetString(property.propertyPath + "_InvokeValue", DefaultValue),
                value => EditorPrefs.SetString(property.propertyPath + "_InvokeValue", value)
            );
        }
    }
}