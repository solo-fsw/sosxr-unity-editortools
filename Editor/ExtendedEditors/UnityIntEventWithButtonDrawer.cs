using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.EditorTools
{
    [CustomPropertyDrawer(typeof(UnityEvent<int>), true)]
    public class UnityIntEventWithButtonDrawer : UnityEventButtonDrawerBase
    {
        private const int DefaultValue = 42;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            DrawButtonWithField(position, property, label, $"SOSXR: {property.displayName}", DefaultValue,
                invokeValue =>
                {
                    var targetObject = property.serializedObject.targetObject;
                    var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<int>;
                    unityEvent?.Invoke(invokeValue);
                },
                () => EditorPrefs.GetInt(property.propertyPath + "_InvokeValue", DefaultValue),
                value => EditorPrefs.SetInt(property.propertyPath + "_InvokeValue", value)
            );
        }
    }
}