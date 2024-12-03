using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.EditorTools
{
    [CustomPropertyDrawer(typeof(UnityEvent<float>), true)]
    public class UnityFloatEventWithButtonDrawer : UnityEventButtonDrawerBase
    {
        private const float DefaultValue = 3.14f;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            DrawButtonWithField(position, property, label, $"SOSXR: {property.displayName}", DefaultValue,
                invokeValue =>
                {
                    var targetObject = property.serializedObject.targetObject;
                    var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<float>;
                    unityEvent?.Invoke(invokeValue);
                },
                () => EditorPrefs.GetFloat(property.propertyPath + "_InvokeValue", DefaultValue),
                value => EditorPrefs.SetFloat(property.propertyPath + "_InvokeValue", value)
            );
        }
    }
}