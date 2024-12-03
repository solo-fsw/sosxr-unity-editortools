using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.EditorTools
{
    [CustomPropertyDrawer(typeof(UnityEvent<bool>), true)]
    public class UnityBoolEventWithButtonDrawer : UnityEventButtonDrawerBase
    {
        private const bool DefaultValue = true;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            DrawButtonWithField(position, property, label, $"SOSXR: {property.displayName}", DefaultValue,
                invokeValue =>
                {
                    var targetObject = property.serializedObject.targetObject;
                    var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<bool>;
                    unityEvent?.Invoke(invokeValue);
                },
                () => EditorPrefs.GetBool(property.propertyPath + "_InvokeValue", DefaultValue),
                value => EditorPrefs.SetBool(property.propertyPath + "_InvokeValue", value)
            );
        }
    }
}